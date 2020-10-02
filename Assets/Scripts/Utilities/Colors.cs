using UnityEngine;

public static class Colors
{
    public static Color LerpColor(this Color c, Color ca, Color cb)
    {
        return new Color(Mathf.Sqrt((ca.r * 255 * ca.r * 255) + (cb.r * 255 * cb.r * 255) / 2),
                         Mathf.Sqrt((ca.g * 255 * ca.g * 255) + (cb.g * 255 * cb.g * 255) / 2),
                         Mathf.Sqrt((ca.b * 255 * ca.b * 255) + (cb.b * 255 * cb.b * 255) / 2));
    }
    public static Color LerpColor(this Color c, Color ca, Color cb, float t)
    {
        return new Color(Mathf.Sqrt((ca.r * 255 * ca.r * 255) + (cb.r * 255 * cb.r * 255) / (1 / t)),
                         Mathf.Sqrt((ca.g * 255 * ca.g * 255) + (cb.g * 255 * cb.g * 255) / (1 / t)),
                         Mathf.Sqrt((ca.b * 255 * ca.b * 255) + (cb.b * 255 * cb.b * 255) / (1 / t)));
    }
}
