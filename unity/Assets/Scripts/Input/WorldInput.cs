using System.Collections;
using UnityEngine;

/// <summary>
/// Класс обработчик всего инпута
/// </summary>
public class WorldInput : MonoBehaviour
{


    public void Update()
    {
        ///Обязательный инпут
        StandartInput();
    }

    public void StandartInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Time.timeScale = Time.timeScale == 0? 1 : 0;
    }


    [System.Serializable]
    public enum InputType
    {
        WASD, Mission
    }
}