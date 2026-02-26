using System.Collections;
using UnityEngine;


public class WingMotor : ControlledBodyModule
{
    [SerializeField] private float MaxRPM;
    [field:SerializeField] public float RPM { get; private set; }
    public override float amperage => Mathf.Max(maxAmperage * (RPM / MaxRPM), 0.1f);
    public override Type type => Type.Сonverter;
}