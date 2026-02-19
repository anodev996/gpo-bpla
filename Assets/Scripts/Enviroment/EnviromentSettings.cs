using System;
using UnityEngine;

public class EnviromentSettings : Singleton<EnviromentSettings>
{
    /// <summary>
    /// Универсальная газовая постоянная, Дж/(моль·К).
    /// </summary>
    public const float R = 8.314462618f;

    /// <summary>
    /// Стандартное давление на уровне моря (н.у.), Па.
    /// </summary>
    public const float P0 = 101325.0f;

    public float temperture = 15;
    public static float Temperture => Instance.temperture;

    /// <summary>
    /// Молярная масса сухого воздуха, кг/моль.
    /// </summary>
    public const float MolarMassAir = 0.0289647f;

    /// <summary>
    /// Ускорение свободного падения на уровне моря, м/с².
    /// </summary>
    public Vector3 g = new Vector3(0, 9.80665f, 0);
    public static Vector3 G => Instance.g;
    /// <summary>
    /// Шкала высоты для изотермической атмосферы, м.
    /// Вычисляется как H = R * T0 / (MolarMassAir * g).
    /// </summary>
    public static readonly float ScaleHeight = R * Temperture / (MolarMassAir * G.magnitude);

    /// <summary>
    /// Возвращает атмосферное давление на заданной высоте в предположении изотермической атмосферы.
    /// Используется барометрическая формула: P = P0 * exp(-h / H).
    /// </summary>
    /// <param name="altitude">Высота над уровнем моря, м.</param>
    /// <returns>Давление, Па.</returns>
    public static float GetPressure(float altitude)
    {
        return P0 * Mathf.Exp(-altitude / ScaleHeight);
    }

    /// <summary>
    /// Возвращает плотность воздуха при заданном давлении и молярной массе,
    /// используя стандартную температуру T0.
    /// Формула: ρ = (P * M) / (R * T0).
    /// </summary>
    /// <param name="pressure">Давление, Па.</param>
    /// <returns>Плотность, кг/м³.</returns>
    public static float GetDensity(float pressure)
    {
        return (pressure * MolarMassAir) / (R * Temperture);
    }
}
