using System;
using System.Collections;
using TriInspector;
using UnityEngine;

[Serializable]
[DeclareTabGroup("Main")]
public class PIDVector
{
    [SerializeField, Group("Main"), Tab("X")] private PIDController x;
    [SerializeField, Group("Main"), Tab("Y")] private PIDController y;
    [SerializeField, Group("Main"), Tab("Z")] private PIDController z;
    [ShowInInspector, ReadOnly] public Vector3 integral => new Vector3(x.integral, y.integral, z.integral);
    [ShowInInspector, ReadOnly] public Vector3 error => new Vector3(x.prevError, y.prevError, z.prevError);



    public PIDVector(PIDController x, PIDController y, PIDController z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 Update(Vector3 error, float dt)
    {
        return new Vector3(x.Update(error.x, dt), y.Update(error.y, dt), z.Update(error.z, dt));
    }
    public void Reset()
    {
        x.Reset(); y.Reset(); z.Reset();
    }
}