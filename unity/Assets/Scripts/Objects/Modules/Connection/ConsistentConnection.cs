using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class ConsistentConnection : ControlledBodyModule, IConnection
{
    public override Type type => Type.Сonverter;

    [ShowInInspector][ReadOnly] private ControlledBodyModule[] modules;

    public override float maxAmperage
    {
        get
        {
            return maxVoltage / resistance;
        }
    }
    [HideInInspector] public override float maxVoltage
    {
        get
        {
            float v = 0;
            for(int i = 0; i < modules.Length; i++) 
                v += modules[i].maxVoltage;
            return v;
        }
    }
    private float m_resistance;
    public override float resistance
    {
        get
        {
            return m_resistance;
        }
        protected set
        {
            m_resistance = value;
        }
    }
    public override float currentAmperage 
    {   
        get 
        {
            float I = 0;
            for (int i = 0; i < modules.Length; i++)
                I += modules[i].currentAmperage;
            return I;
        }
        set
        {
            var c = 0f;
            if (needAmperage == 0)
                c = 0f;
            else
                c = value / needAmperage;
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].currentAmperage = c * modules[i].needAmperage;
            }
        } 
    }
    public override float currentVoltage
    {
        get
        {
            var v = 0f;
            for (int i = 0; i < modules.Length; i++)
                v += modules[i].currentVoltage;
            return v / modules.Length;
        }
        set
        {
            var v = value;
            var rv = 0f;
            for(int i = 0; i < modules.Length;i++)
            {
                rv += modules[i].maxVoltage;
            }
            float c = Mathf.Clamp01(rv / v);
            for(int i = 0;i < modules.Length;i++)
            {
                modules[i].currentVoltage = c * modules[i].needVoltage;
            }
        }
    }
    public override float needAmperage
    {
        get
        {
            float I = 0;
            for (int i = 0; i < modules.Length; i++)
                I += modules[i].needAmperage;
            return I;
        }
        set
        {
            for (int i = 0; i < modules.Length; i++)
                modules[i].needAmperage = value;
        }
    }
    public override float needVoltage
    {
        get
        {
            var v = 0f;
            for (int i = 0; i < modules.Length; i++)
                v += modules[i].needVoltage;
            return v / modules.Length;
        }
    }
    public override void OnStart(ControlledBody body)
    {
        base.OnStart(body);
        modules = GetModules(transform).ToArray();

        for (int i = 0; i < modules.Length; i++)
            maxVoltage += modules[i].maxVoltage;
        for (int i = 0; i < modules.Length; i++)
            resistance += modules[i].resistance;
        maxAmperage = maxVoltage / resistance;

        for(int i = 0; i< modules.Length;i++)
            modules[i].OnStart(body);
    }
    public override void OnShutdown(ControlledBody body)
    {
        base.OnShutdown(body);
        for (int i = 0; i < modules.Length; i++)
            modules[i].OnShutdown(body);
    }
    public override void OnUpdate(ControlledBody body)
    {
        base.OnUpdate(body);
        for (int i = 0; i < modules.Length; i++)
            modules[i].OnUpdate(body);
    }


    private List<ControlledBodyModule> GetModules(Transform root)
    {
        var result = new List<ControlledBodyModule>();
        if (root == null)
            return result;

        // Используем очередь для обхода в ширину (итеративный подход, чтобы избежать глубокой рекурсии)
        var queue = new Queue<Transform>();
        for(int i = 0; i < root.childCount;i++)
            queue.Enqueue(root.GetChild(i));

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            ControlledBodyModule component = current.GetComponent<ControlledBodyModule>();
            if (component != null)
                result.Add(component);

            if (current.GetComponent<IConnection>() == null)
            {
                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }
        }

        return result;
    }
}