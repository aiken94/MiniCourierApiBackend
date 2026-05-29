namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class PackageCollection
    {
        public string? Status { get; set; }
        public int Count { get; set; }
        public List<PackageResource>? Data { get; set; }

        public static PackageCollection FromCollection(IEnumerable<Package> packages)
        {
            var list = packages.Select(PackageResource.FromModel).ToList();

            return new PackageCollection
            {
                Status = "success",
                Count = list.Count,
                Data = list
            };
        }
    }
}