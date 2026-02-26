using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Управляемое тело, это любое тело которым может управлять игрок,
/// В нашей симуляции под такими телами подразумеваются обычно квадрокоптеры и (возможно) самолеты,
/// Так же контроллируемое тело само является контроллером включающих его модулей
/// </summary>
public abstract class ControlledBody : Body
{
    ///<summary>
    /// Радиус от которого будет действовать WorldInput при неполном наведении
    ///</summary>
    [field: SerializeField] public float inputRadius { get; protected set; } = 2;
    public static List<ControlledBody> bodies { get; protected set; } = new List<ControlledBody>();

    protected override void OnEnable()
    {
        base.OnEnable();
        bodies.Add(this);
        CollectModules();
        StartModules();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        bodies.Remove(this);
        ShutdownModules();
    }

    protected override void Update()
    {
        base.Update();
        UpdateModules();
    }

    #region Modules

    private ControlledBodyModule[] СonvertersModules;
    private ControlledBodyModule[] ProducersModules;

    public float amperage { get; protected set; }
    public float voltage {  get; protected set; }
    public float resistance { get; protected set; }

    public void CollectModules()
    {
        List<ControlledBodyModule> converters = new List<ControlledBodyModule>();
        List<ControlledBodyModule> producers = new List<ControlledBodyModule>();

        var modules = GetComponents<ControlledBodyModule>().ToList();
        modules.AddRange(GetComponentsInChildren<ControlledBodyModule>());

        for (int i = 0; i < modules.Count;i++)
        {
            switch(modules[i].type)
            {
                case ControlledBodyModule.Type.Сonverter:
                    converters.Add(modules[i]);
                    break;
                case ControlledBodyModule.Type.Producer:
                    producers.Add(modules[i]);
                    break;
            }
        }

        СonvertersModules = converters.ToArray();
        ProducersModules = producers.ToArray();
    }

    public void StartModules()
    {
        for(int i = 0;i < СonvertersModules.Length;i++)
        {
            СonvertersModules[i].OnStart(this);
        }
        for (int i = 0; i < ProducersModules.Length; i++)
        {
            ProducersModules[i].OnStart(this);
        }
    }

    public void ShutdownModules()
    {
        for (int i = 0; i < СonvertersModules.Length; i++)
        {
            СonvertersModules[i].OnShutdown(this);
        }
        for (int i = 0; i < ProducersModules.Length; i++)
        {
            ProducersModules[i].OnShutdown(this);
        }
    }

    public void UpdateModules()
    {
        //Clear
        voltage = 0;
        amperage = 0;
        resistance = 0;
        ///Все батарейки считаются парарельно соединенными
        for (int i = 0; i < ProducersModules.Length; i++)
        {
            voltage += ProducersModules[i].voltage;
            amperage += ProducersModules[i].amperage;
            resistance += (1 / ProducersModules[i].resistance);
        }
        resistance = 1 / resistance;

        for (int i = 0; i < СonvertersModules.Length; i++)
        {
            resistance += СonvertersModules[i].resistance;
        }
        var needAmperage = voltage / resistance;

        Debug.Log($"V:{voltage} I:{amperage} R:{resistance} In{needAmperage}");
        for (int i = 0; i < СonvertersModules.Length; i++)
        {
            СonvertersModules[i].OnUpdate(this);
        }
        for (int i = 0; i < ProducersModules.Length; i++)
        {
            ProducersModules[i].OnUpdate(this);
        }
    }

    #endregion

    #region Input
    public virtual void OnClick(WorldInput input)
    {

    }

    public virtual void OnDoubleClick(WorldInput input)
    {
        var cTransform = input.cameraTransform;
        var dist = Vector3.Distance(cTransform.position, transform.position);
        if (dist < 2 || dist > 5)
        {
            if (cTransform.TryGetComponent(out IMoveTo moveTo))
            {
                moveTo.MoveTo((cTransform.position - transform.position).normalized * Mathf.Min(Mathf.Max(2, Vector3.Distance(cTransform.position, transform.position)), 5));
            }
        }
    }

    public virtual void OnDownAim(WorldInput input)
    {
        gameObject.layer = 3;
    }

    public virtual void OnUpAim(WorldInput input)
    {
        gameObject.layer = 0;
    }
    #endregion
}