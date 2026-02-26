using System.Collections;
using UnityEngine;

/// <summary>
/// Модуль контролированного тела,
/// Этот обьект может обладать достаточно разнообразной функциональностью,
/// Все модули потребляют либо предоставляют энегрию
/// </summary>
public abstract class ControlledBodyModule : MonoBehaviour
{
    /// <summary>
    /// Максимальная cила тока
    /// </summary>
    [field: SerializeField] public float maxAmperage {  get; private set; }
    /// <summary>
    /// Максимальное напряжение
    /// </summary>
    [field: SerializeField] public float maxVoltage { get; private set; }

    public abstract Type type { get; }

    public virtual float amperage => maxAmperage;
    public virtual float voltage => maxVoltage;
    public virtual float resistance
    {
        get
        {
            return voltage / amperage;
        }
        protected set
        {

        }
    }
    public virtual void OnStart(ControlledBody body) { }
    public virtual void OnUpdate(ControlledBody body) { }
    public virtual void OnShutdown(ControlledBody body) { }

    public enum Type
    {
        /// <summary>
        /// Все что потребляет ток
        /// </summary>
        Сonverter,
        /// <summary>
        /// Все что выводит ток в сеть
        /// </summary>
        Producer
    }
}