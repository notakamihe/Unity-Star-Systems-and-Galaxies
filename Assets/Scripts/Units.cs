using UnityEngine;

public static class Units {
    public const float G = 6.67f;
    public const float AU = 1500.0f;
    public const float LIGHT_YEAR = 948615.0f;
    public const float KPC = 3092489.0f;
    public const float MPC = 3092489000.0f;
    public const float SOLAR_RADIUS = 2500.0f;
    public const float SOLAR_MASS = 198801000.0f;
    public const float SOLAR_LUMINOSITY = 5.0f;
    public const float SPEED_OF_LIGHT = 299792458.0f;
    public const float EARTH_MASS = 597.2f;
    public const float EARTH_RADIUS = 5.0f;
    public const float LUNAR_MASS = 7.37f;
    public const float LUNAR_RADIUS = 1.25f;
    public const float JUPITER_MASS = 189790.0f;
    public const float JUPITER_RADIUS = 55.0f;

    public static float ToAU(float distance)
    {
        return distance / AU;
    }

    public static string ToDistanceUnit(float distance)
    {
        if (distance >= LIGHT_YEAR)
        {
            return ToLightYear(distance).ToString("0.##") + " LY";
        }
        else if (distance < AU)
        {
            return (distance * 10000.0f).ToString("0") + " KM";
        }
        else
        {
            return ToAU(distance).ToString("0.#") + " AU";
        }
    }

    public static float ToEarthMass(float mass)
    {
        return mass / EARTH_MASS;
    }

    public static float ToEarthRadius(float radius)
    {
        return radius / EARTH_RADIUS;
    }

    public static float ToHertz(float rotationSpeed)
    {
        return rotationSpeed / 100489.0f;
    }

    public static float ToHour(float days)
    {
        return days * 24.0f;
    }

    public static float ToJupiterMass(float mass)
    {
        return mass / JUPITER_MASS;
    }

    public static float ToJupiterRadius(float radius)
    {
        return radius / JUPITER_RADIUS;
    }

    public static float ToKPC(float distance)
    {
        return distance / KPC;
    }

    public static float ToLightYear(float distance)
    {
        return distance / LIGHT_YEAR;
    }

    public static float ToLunarMass(float mass)
    {
        return mass / LUNAR_MASS;
    }

    public static float ToLunarRadius(float radius)
    {
        return radius / LUNAR_RADIUS;
    }

    public static float ToMPC(float distance)
    {
        return distance / MPC;
    }

    public static string ToPC(float distance)
    {
        if (distance >= MPC)
        {
            return ToMPC(distance).ToString("0.##") + " MPC";
        }
        else
        {
            return ToKPC(distance).ToString("0.##") + " KPC";
        }
    }

    public static string ToPlanetMassUnit(float mass)
    {
        if (mass >= JUPITER_MASS)
        {
            return ToJupiterMass(mass).ToString("0.####") + " M♃";
        }
        else
        {
            return ToEarthMass(mass).ToString("0.####") + " M⊕";
        }
    }

    public static string ToPlanetRadiusUnit(float radius)
    {
        if (radius >= JUPITER_RADIUS)
        {
            return ToJupiterRadius(radius).ToString("0.#") + " R♃";
        }
        else
        {
            return ToEarthRadius(radius).ToString("0.#") + " R⊕";
        }
    }

    public static float ToSolarLuminosity(float luminosity)
    {
        return luminosity / SOLAR_LUMINOSITY;
    }

    public static float ToSolarMass(float mass)
    {
        return mass / SOLAR_MASS;
    }

    public static float ToSolarRadius(float radius)
    {
        return radius / SOLAR_RADIUS;
    }

    public static string ToTimeUnit(float days)
    {
        if (days <= 3.0f)
        {
            return ToHour(days).ToString("#.#") + "H";
        }
        else if (days >= 500)
        {
            return ToYear(days).ToString("#.#") + "Y";
        }
        else
        {
            return days.ToString("#.#") + "D";
        }
    }

    public static float ToYear(float days)
    {
        return days / 365.0f;
    }
}

public static class SpectralType
{
    public static Color M = new Color(1.0f, 0.0f, 0.2f);
    public static Color K = new Color(1.0f, 0.4f, 0.0f);
    public static Color G = Color.yellow;
    public static Color F = Color.white;
    public static Color A = new Color(0.8f, 0.8f, 1.0f);
    public static Color B = new Color(0.3f, 0.3f, 1.0f);
    public static Color O = Color.blue;

    public static Color Interpolate (Color start, Color end)
    {
        return Color.Lerp(start, end, Utils.NextFloat(0.0f, 1.0f));
    }
} 