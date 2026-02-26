using System.Collections;
using UnityEngine;


public class Battery : ControlledBodyModule
{
    /// <summary>
    /// Минимальный вольтаж при котором батарея считается разраженной
    /// </summary>
    [SerializeField] private float minVoltage;
    /// <summary>
    /// ёмкость батарейки в А/ч
    /// </summary>
    [SerializeField] private float maxCapacity;
    /// <summary>
    /// Внутреннее сопротивление батарейки
    /// </summary>
    [field:SerializeField] public override float resistance { get; protected set; }
    public override float voltage
    {
        get 
        {
            return minVoltage + ((maxVoltage - minVoltage) * (currentCapacity / maxCapacity));     
        }
    }
    [field: SerializeField] public float currentCapacity { get; protected set; }
    public override void OnStart(ControlledBody body)
    {
        base.OnStart(body);
        currentCapacity = maxCapacity;
    }

    public override Type type => Type.Producer;
}