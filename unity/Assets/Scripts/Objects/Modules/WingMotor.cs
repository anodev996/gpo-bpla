using System.Collections;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(Wing))]
public class WingMotor : ControlledBodyModule
{
    [RequiredGet][SerializeField] private Wing wing;
    [SerializeField] private float MaxRPM;
    [field:SerializeField] public float RPM { get; private set; }
    public override float currentAmperage => Mathf.Max(maxAmperage * (RPM / MaxRPM), 0.1f);
    public override Type type => Type.Сonverter;

    public override void OnUpdate(ControlledBody body)
    {
        base.OnUpdate(body);

        wing.frequency = RPM / 60;
    }
}