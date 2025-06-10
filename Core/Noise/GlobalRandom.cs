namespace Core.Noise;

public static class GlobalRandom
{
    public static int Seed { get; set; }
    private static Random? _random;
    public static Random Instance => _random ??= new Random(Seed);
}