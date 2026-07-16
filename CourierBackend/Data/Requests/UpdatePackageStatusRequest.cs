using CourierBackend.Data.Enums;

namespace CourierBackend.Data.Requests
{
    public class UpdatePackageStatusRequest
    {
        public PackageStatus  PackageStatus { get; set; }
    }
}