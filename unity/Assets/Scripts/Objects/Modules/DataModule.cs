using System.Collections;
using TriInspector;
using UnityEngine;


public abstract class DataModule<T> : ControlledBodyModule where T : struct
{
    [ReadOnly][ShowInInspector] public T data;
    public override Type type => Type.Сonverter;
    public T GetData()
    {
        return data;
    }
    public override void OnUpdate(ControlledBody body)
    {
        base.OnUpdate(body);
        data = getData(body);
    }
    protected abstract T getData(ControlledBody body);
}