namespace CourierBackend.Data.Resource
{
    using CourierBackend.Models;

    public class AdminCollection
    {
        public string? Status { get; set; }
        public int Count { get; set; }
        public List<AdminResource>? Data { get; set; }

        public static AdminCollection FromCollection(IEnumerable<Admin> admins)
        {
            var list = admins.Select(AdminResource.FromModel).ToList();

            return new AdminCollection
            {
                Status = "success",
                Count = list.Count,
                Data = list
            };
        }
    }
}