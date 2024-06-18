namespace Digital_Rain;

using System;

public struct RGB_Col : IEquatable<RGB_Col>
{
    public byte R;
    public byte G;
    public byte B;

    public RGB_Col(byte R, byte G, byte B)
    {
        this.R = R;
        this.G = G;
        this.B = B;
    }

    public RGB_Col(int PackedRGB)
    {
        R = (byte)((PackedRGB >> 16) & 0xff);
        G = (byte)((PackedRGB >> 8) & 0xff);
        B = (byte)(PackedRGB & 0xff);
    }

    public bool Equals(RGB_Col Other) => R == Other.R && G == Other.G && B == Other.B;

    public override bool Equals(object? Obj)
    {
        if (Obj == null || GetType() != Obj.GetType())
            return false;

        return Equals((RGB_Col)Obj);
    }

    public static RGB_Col operator +(RGB_Col Left, RGB_Col Right) => new() { R = (byte)Math.Clamp(Left.R + Right.R, byte.MinValue, byte.MaxValue), G = (byte)Math.Clamp(Left.G + Right.G, byte.MinValue, byte.MaxValue), B = (byte)Math.Clamp(Left.B + Right.B, byte.MinValue, byte.MaxValue) };
    public static RGB_Col operator -(RGB_Col Left, RGB_Col Right) => new() { R = (byte)Math.Clamp(Left.R - Right.R, byte.MinValue, byte.MaxValue), G = (byte)Math.Clamp(Left.G - Right.G, byte.MinValue, byte.MaxValue), B = (byte)Math.Clamp(Left.B - Right.B, byte.MinValue, byte.MaxValue) };

    public static bool operator ==(RGB_Col Left, RGB_Col Right) => Left.Equals(Right);

    public static bool operator !=(RGB_Col Left, RGB_Col Right) => !(Left == Right);

    public override int GetHashCode() => (R << 16) | (G << 8) | B;

}

public struct HSL_Col(double Hue, double Saturation, double Lightness)
{
    public double Hue = Hue;               // 0 - 360
    public double Saturation = Saturation; // 0 - 1
    public double Lightness = Lightness;   // 0 - 1

    public RGB_Col ToRGB_Col()
    {
        return HSL_Util.HSLtoRGB(this);
    }
};

public struct RGB_Char
{
    public RGB_Col FG_Col;
    public RGB_Col BG_Col;
    public char Character;
}

class HSL_Util
{
    // Weird name taken straight from the textbook this came out of. 
    static double Value(double n1, double n2, double hue)
    {
        if (hue > 360.0)
            hue -= 360.0;
        else if (hue < 0.0)
            hue += 360.0;

        if (hue < 60.0)
            return n1 + (((n2 - n1) * hue) / 60.0);
        else if (hue < 180)
            return n2;
        else if (hue < 240.0)
            return n1 + (((n2 - n1) * (240.0 - hue)) / 60.0);
        else
            return n1;
    }

    // Logic from page 596, fig 13.37 of "Computer Graphics: Principles and Practices" by James D. Foley
    public static RGB_Col HSLtoRGB(HSL_Col Color)
    {
        double m1, m2;

        m2 = (Color.Lightness <= 0.5) ? (Color.Lightness * (Color.Lightness + Color.Saturation)) : ((Color.Lightness + Color.Saturation) - (Color.Lightness * Color.Saturation));
        m1 = (2.0 * Color.Lightness) - m2;
        if (Color.Saturation == 0.0) // no hue
        {
            return new RGB_Col(
                (byte)Math.Round(Color.Lightness * 255.0),
                (byte)Math.Round(Color.Lightness * 255.0),
                (byte)Math.Round(Color.Lightness * 255.0));
        }
        else // has hue
        {
            return new RGB_Col(
                (byte)Math.Round(Value(m1, m2, Color.Hue + 120.0) * 255.0),
                (byte)Math.Round(Value(m1, m2, Color.Hue) * 255.0),
                (byte)Math.Round(Value(m1, m2, Color.Hue - 120.0) * 255.0));
        }
    }
}