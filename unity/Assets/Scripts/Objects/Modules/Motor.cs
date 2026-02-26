using System.Collections;
using TriInspector;
using UnityEngine;

/// <summary>
/// Абстрактная реализация любого мотора для проекта
/// </summary>
public class Motor : ControlledBodyModule
{
    [SerializeField] private float MaxRPM;
    [SerializeField] private float currentRPM;
    [ReadOnly][SerializeField] private float localMaxAmperage;
    
    /// <summary>
    /// Величина от 0 до 1, где 0 - не работает, 1 - максимальная возможная нагрузка
    /// </summary>
    public float Workload
    {
        get
        {
            return RPM / MaxRPM;
        }
        set
        {
            RPM = Mathf.Lerp(0, MaxRPM,Mathf.Clamp01(value));
        }
    }
    public float RPM
    {
        get
        {
            float currentMaxRPM = MaxRPM * (currentAmperage / maxAmperage);
            if (currentRPM > currentMaxRPM)
            {
                currentRPM = currentMaxRPM;
            }
            return currentRPM;
        }
        set
        {
            currentRPM = value;
        }
    }
    public override float currentAmperage
    {
        get
        {
            return localMaxAmperage * (currentRPM / MaxRPM);
        }
        set
        {
            localMaxAmperage = value;
        }
    }
    /// <summary>
    /// Ручной способ установки RPM через силу тока
    /// </summary>
    public void SetAmperage(float amperage)
    {
        Workload = amperage / maxAmperage;
    }
    public override Type type => Type.Сonverter;
}