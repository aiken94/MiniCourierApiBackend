namespace CourierBackend.Services
{
    using System.Reflection;

    public static class FieldSelectionService
    {

        public static List<DynamicDto> Apply<T>(List<T> data, string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return data.Select(item =>
                {
                    var dict = new DynamicDto();

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        dict[prop.Name] = prop.GetValue(item);
                    }

                    return dict;
                }).ToList();
            }

            var selectedFields = fields
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return data.Select(item =>
            {
                var dict = new DynamicDto();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (selectedFields.Contains(prop.Name))
                    {
                        dict[prop.Name] = prop.GetValue(item);
                    }
                }

                return dict;
            }).ToList();
        }
    }
}