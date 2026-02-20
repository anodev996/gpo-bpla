using System;
using UnityEngine;

public class EnviromentSettings : Singleton<EnviromentSettings>
{
    public const float R = 8.314462618f;

    public const float P0 = 101325.0f;

    public float temperture = 15;
    public static float Temperture => Instance.temperture;

    public const float MolarMassAir = 0.0289647f;

    public Vector3 gravity = new Vector3(0, -9.80665f, 0);
    public static Vector3 Gravity => Instance.gravity;

    public static float ScaleHeight => 8000;

    public Quaternion windDirection;
    public static Quaternion WindDirection => Instance.windDirection;
    public float windSpeed;
    public static float WindSpeed => Instance.windSpeed;

    public static float GetPressure(float height)
    {
        return P0 * Mathf.Exp(-height / ScaleHeight);
    }

    public static float GetDensity(float height)
    {
        return (GetPressure(height) * MolarMassAir) / (R * (Temperture + 273f));
    }
}
