using System.Collections;
using TriInspector;
using UnityEngine;

/// <summary>
/// Абстрактная реализация любого мотора для проекта
/// </summary>
public class Motor : ControlledBodyModule
{
    [SerializeField] private float MaxRPM;
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
            needAmperage = Mathf.Lerp(0, maxAmperage, value);
        }
    }
    public float RPM
    {
        get
        {
            if (Mathf.Abs(currentAmperage - needAmperage) < (maxAmperage * 0.2f))
                return MaxRPM * (needAmperage / maxAmperage);
            return MaxRPM * (currentAmperage / maxAmperage);
        }
    }
    public override float needAmperage { get; set; }
    public override Type type => Type.Сonverter;
}