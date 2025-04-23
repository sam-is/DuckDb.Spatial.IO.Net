namespace DuckDb.Spatial.IO.Net.Extension;

public static class FloatConverter
{
    public static float DoubleToFloatDown(double d)
    {
        if (d > float.MaxValue) return float.MaxValue;
        if (d <= float.MinValue) return float.MinValue;

        float converted = (float)d;
        return (double)converted <= d
            ? converted
            : NextAfter(converted, float.MinValue);
    }

    public static float DoubleToFloatUp(double d)
    {
        if (d >= float.MaxValue) return float.MaxValue;
        if (d < float.MinValue) return float.MinValue;

        float converted = (float)d;
        return (double)converted >= d
            ? converted
            : NextAfter(converted, float.MaxValue);
    }

    public static float NextAfter(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            return x + y;

        if (x == y)
            return y;

        var bits = BitConverter.SingleToUInt32Bits(x);
        var direction = Math.Sign(y - x);
        var delta = direction == 0 ? 0 : (direction > 0 ? 1u : uint.MaxValue);

        if (bits == 0 && direction < 0)
            return BitConverter.UInt32BitsToSingle(0x80000001);

        return BitConverter.UInt32BitsToSingle(bits + delta);
    }
}