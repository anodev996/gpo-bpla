using System.Collections;
using TriInspector;
using UnityEngine;


public class GPS : DataModule<Vector2>
{
    public override Type type => Type.Сonverter;
    protected override Vector2 getData(ControlledBody body)
    {
        return new Vector2(body.transform.position.x, body.transform.position.y);
    }
}