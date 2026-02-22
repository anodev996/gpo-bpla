using System.Collections;
using UnityEngine;

/// <summary>
/// Компонент тела, это любой компонент 
/// оказывающий постоянное физическое воздействие на Тело
/// </summary>
public abstract class ComponentBody : MonoBehaviour
{
    /// <summary>
    /// Вызывается при добавлении в тело
    /// </summary>
    public virtual void OnInit(Body body) { }
    /// <summary>
    /// Вызывается при уничтожении тела
    /// </summary>
    /// <param name="body"></param>
    public virtual void OnShutdown(Body body) { }
    /// <summary>
    /// Вызывается во время обновления физического тела
    /// </summary>
    /// <param name="body"></param>
    public abstract void OnUpdate(Body body);
}