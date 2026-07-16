namespace CourierBackend.Data.Enums
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PackageStatus
    {
        Pending,
        InTransit,
        Failed,
        Delivered
    }
}