using System.Collections;
using TriInspector;
using UnityEngine;


public class Battery : ControlledBodyModule
{
    [InfoBox("Минимальный вольтаж при котором батарея считается разраженной")]
    [SerializeField] private float minVoltage;
    [InfoBox("ёмкость батарейки в А/ч")]
    [SerializeField] private float maxCapacity;
    [InfoBox("Внутреннее сопротивление батарейки")]
    [field:SerializeField] public override float resistance { get; protected set; }
    public override float currentVoltage
    {
        get 
        {
            if (currentCapacity > 0)
            {
                return minVoltage + ((maxVoltage - minVoltage) * (currentCapacity / maxCapacity));
            }   
            else
            {
                return 0;
            }
        }
    }
    [field: SerializeField] public float currentCapacity { get; protected set; }
    public override void OnStart(ControlledBody body)
    {
        base.OnStart(body);
        currentCapacity = maxCapacity;
    }
    public override void OnUpdate(ControlledBody body)
    {
        base.OnUpdate(body);

        currentCapacity -= currentAmperage * (Time.deltaTime/3600);
    }

    public override Type type => Type.Producer;
}