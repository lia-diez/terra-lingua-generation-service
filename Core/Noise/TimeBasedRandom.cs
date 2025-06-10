using System.Security.Cryptography;
using System.Text;

namespace Cli.Helpers;

public class TimeBasedRandom
{
    public static int Generate()
    {
        // Get current time as a string
        string timeString = DateTime.UtcNow.Ticks.ToString();

        // Hash it using SHA256
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(timeString));

        // Convert part of the hash to an integer
        int value = BitConverter.ToInt32(hash, 0);
        // Ensure it's positive
        return Math.Abs(value);
    }
}