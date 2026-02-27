using System.Collections;
using UnityEngine;


public class Quadcopter : ControlledBody
{
    [SerializeField] protected Motor[] motors;
    public Matrix2x2 movement;
    
    public Motor GetMotor(int x, int y)
    {
        return motors[x + (y * 2)];
    }

    protected override void Update()
    {
        base.Update();

        for(int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                GetMotor(x,y).Workload = movement[x, y];
            }
        }
    }
}