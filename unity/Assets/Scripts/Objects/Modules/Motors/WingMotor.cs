using System.Collections;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(Wing))]
public class WingMotor : Motor
{
    [RequiredGet][SerializeField] private Wing wing;

    public override void OnUpdate(ControlledBody body)
    {
        base.OnUpdate(body);

        wing.frequency = RPM / 60;
    }
}