using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
    [ShowInInspector][ReadOnly] private ControlledBodyModule[] СonvertersModules;
    [ShowInInspector][ReadOnly] private ControlledBodyModule[] ProducersModules;

    public void CollectModules()
    {
        List<ControlledBodyModule> converters = new List<ControlledBodyModule>();
        List<ControlledBodyModule> producers = new List<ControlledBodyModule>();

        var modules = GetModules(transform);

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
    private List<ControlledBodyModule> GetModules(Transform root)
    {
        var result = new List<ControlledBodyModule>();
        if (root == null)
            return result;

        // Используем очередь для обхода в ширину (итеративный подход, чтобы избежать глубокой рекурсии)
        var queue = new Queue<Transform>();
        queue.Enqueue(root);

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
        var Vbat = 0f;
        var Ibat = 0f;
        var Rbat = 0f;
        ///Все батарейки считаются парарельно соединенными
        for (int i = 0; i < ProducersModules.Length; i++)
        {
            Vbat += ProducersModules[i].maxVoltage;
            Ibat += ProducersModules[i].maxAmperage;
            Rbat += (1 / ProducersModules[i].resistance);
        }
        Rbat = 1 / Rbat;

        var Ic = 0f;
        var Rc = 0f;
        ///Все элементы цепи так же парарельно соединены
        ///
        ///Correction
        for (int i = 0; i < СonvertersModules.Length; i++)
        {
            ///если внутренние сопротивление слишком мало срабатывает защита и ток на цепь не подается 
            if (СonvertersModules[i].resistance > 0.5f)
            {
                if (СonvertersModules[i].currentVoltage <= Vbat)
                {
                    СonvertersModules[i].currentAmperage = СonvertersModules[i].maxAmperage;
                    СonvertersModules[i].currentVoltage = СonvertersModules[i].maxVoltage;
                    Rc += (1 / СonvertersModules[i].resistance);
                    Ic += СonvertersModules[i].currentAmperage;
                }
                else
                {
                    СonvertersModules[i].currentVoltage = Vbat;
                    СonvertersModules[i].currentAmperage = Vbat / СonvertersModules[i].resistance;
                    Rc += (1 / СonvertersModules[i].resistance);
                }
            }
        }
        Rc = 1 / Rc;

        Debug.Log($"Vb:{Vbat} Ib:{Ibat} Rb:{Rbat} Ic{Ic} Rc{Rc}");
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