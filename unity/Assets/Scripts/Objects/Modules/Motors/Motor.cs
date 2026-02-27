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
            return MaxRPM * (needAmperage / maxAmperage);
        }
    }
    public override float needAmperage { get; set; }
    public override Type type => Type.Сonverter;
}