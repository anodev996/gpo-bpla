using System.Collections;
using UnityEngine;

/// <summary>
/// Класс обработчик всего инпута
/// </summary>
public class WorldInput : MonoBehaviour
{

    [System.Serializable]
    public enum InputType
    {
        WASD, Mission
    }
}