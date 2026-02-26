using System.Collections;
using TriInspector;
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
    [field: SerializeField] public virtual float maxAmperage {  get; protected set; }
    /// <summary>
    /// Максимальное напряжение
    /// </summary>
    [field: SerializeField] public virtual float maxVoltage { get; protected set; }
    public abstract Type type { get; }

    public virtual float currentAmperage { get; set; }
    public virtual float currentVoltage { get; set; }
    /// <summary>
    /// Сопротивление это стандартная величина, 
    /// на нее влияют только максимальные параметры, 
    /// т.к сопротивление в цепи у прибора никогда не меняется
    /// </summary>
    public virtual float resistance
    {
        get
        {
            return maxVoltage / maxAmperage;
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