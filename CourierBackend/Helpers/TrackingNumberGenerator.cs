namespace CourierBackend.Helpers;

using System.Security.Cryptography;

public static class TrackingNumberGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string Generate(int length = 10)
    {
        return new string(Enumerable.Range(0, length)
            .Select(_ => Chars[RandomNumberGenerator.GetInt32(Chars.Length)])
            .ToArray());
    }
}