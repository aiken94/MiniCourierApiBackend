namespace CourierBackend.Data.Repositories
{
    using System.Threading.Tasks;
    using CourierBackend.Models;
    using Microsoft.EntityFrameworkCore;
    using CourierBackend.Data;
    using CourierBackend.Data.Repositories.Interfaces;
    using CourierBackend.Data.Requests;
    using CourierBackend.Helpers;
    using CourierBackend.Services;
    using CourierBackend.Services.Auth.Interfaces;
    public class PackageRepository : IPackageRepository
    {
        private readonly CourierContext _context;

        private readonly IFileService _fileService;

        private readonly ICurrentAdminService _currentUser;

        public PackageRepository(CourierContext context, IFileService fileService, ICurrentAdminService currentUser)
        {
            _context = context;
            _fileService = fileService;
            _currentUser = currentUser;
        }

        public IQueryable<Package> GetPackagesAsync()
        {
            return _context.Packages
                .Include(x => x.Admin)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .AsQueryable();
        }

        public async Task<Package?> GetByIdAsync(int id)
        {
            return await _context.Packages
                .Include(x => x.Admin)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Package> CreateAsync(PackageRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // save image
                var imagePath = await _fileService
                    .SavePackageImageAsync(request.PackageImage);

                // generate unique tracking number
                string trackingNumber;

                do
                {
                    trackingNumber = TrackingNumberGenerator.Generate();
                }
                while (await _context.Packages
                    .AnyAsync(p => p.TrackingNumber == trackingNumber));

                // create package
                var package = new Package
                {
                    AdminId = _currentUser.GetId(), // replace with authenticated admin id
                    Value = request.PackageValue,
                    Weight = request.PackageWeight,
                    Cost = request.PackageCost,
                    ImageUrl = imagePath,
                    Description = request.PackageDescription,
                    TrackingNumber = trackingNumber,
                    DeliveryDate = request.EstimatedDeliveryDate,
                    UpdatedAt = DateTime.UtcNow
                };

                // save package first
                await _context.Packages.AddAsync(package);
                await _context.SaveChangesAsync();

                // create sender
                var sender = new Sender
                {
                    PackageId = package.Id,
                    Name = request.SenderName,
                    Email = request.SenderEmail,
                    PhoneNumber = request.SenderPhoneNumber,
                    Country = request.SenderCountry,
                    Address = request.SenderAddress
                };

                // create receiver
                var receiver = new Receiver
                {
                    PackageId = package.Id,
                    Name = request.ReceiverName,
                    Email = request.ReceiverEmail,
                    PhoneNumber = request.ReceiverPhoneNumber,
                    Country = request.ReceiverCountry,
                    Address = request.ReceiverAddress
                };

                // save sender and receiver
                await _context.Senders.AddAsync(sender);
                await _context.Receivers.AddAsync(receiver);

                await _context.SaveChangesAsync();

                // commit transaction
                await transaction.CommitAsync();

                return await GetByIdAsync(package.Id) ?? throw new Exception("Failed to retrieve created package");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Package> UpdateAsync(PackageRequest request, Package package)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Update package fields
                package.Value = request.PackageValue;
                package.Weight = request.PackageWeight;
                package.Cost = request.PackageCost;
                package.Description = request.PackageDescription;
                package.DeliveryDate = request.EstimatedDeliveryDate;
                package.UpdatedAt = DateTime.UtcNow;

                // optional: update image only if provided
                if (request.PackageImage != null)
                {
                    // update image and delete old one
                    package.ImageUrl = await _fileService.UpdatePackageImageAsync(request.PackageImage, package.ImageUrl);
                }

                // 2. Update Sender (1:1)
                if (package.Sender != null)
                {
                    package.Sender.Name = request.SenderName;
                    package.Sender.Email = request.SenderEmail;
                    package.Sender.PhoneNumber = request.SenderPhoneNumber;
                    package.Sender.Country = request.SenderCountry;
                    package.Sender.Address = request.SenderAddress;
                }
                else
                {
                    package.Sender = new Sender
                    {
                        PackageId = package.Id,
                        Name = request.SenderName,
                        Email = request.SenderEmail,
                        PhoneNumber = request.SenderPhoneNumber,
                        Country = request.SenderCountry,
                        Address = request.SenderAddress
                    };
                }

                // 3. Update Receiver (1:1)
                if (package.Receiver != null)
                {
                    package.Receiver.Name = request.ReceiverName;
                    package.Receiver.Email = request.ReceiverEmail;
                    package.Receiver.PhoneNumber = request.ReceiverPhoneNumber;
                    package.Receiver.Country = request.ReceiverCountry;
                    package.Receiver.Address = request.ReceiverAddress;
                }
                else
                {
                    package.Receiver = new Receiver
                    {
                        PackageId = package.Id,
                        Name = request.ReceiverName,
                        Email = request.ReceiverEmail,
                        PhoneNumber = request.ReceiverPhoneNumber,
                        Country = request.ReceiverCountry,
                        Address = request.ReceiverAddress
                    };
                }

                // 4. Save changes
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return package;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Package package)
        {
            // delete image file
            if (!string.IsNullOrEmpty(package.ImageUrl))
            {
                await _fileService.DeleteFileAsync(package.ImageUrl);
            }

            _context.Packages.Remove(package);

            await _context.SaveChangesAsync();
        }

        public async Task<Package?> GetByTrackingNumberAsync(string trackingNumber)
        {
            var package = await _context.Packages
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Include(x => x.Histories)
                .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

            // since this is package tracking request, we need to update the number of tracking column by increasing by 1
            if (package != null)
            {
                package.NoOfTracking++;
                await _context.SaveChangesAsync();
            }

            return package;
        }
    }
}