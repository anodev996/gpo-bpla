using UnityEngine;
using UnityEngine.UI;

public class TabHighlighter : MonoBehaviour
{
    [Header("Настройки кнопок")]
    public Button[] tabButtons; 
    
    [Header("Цвета")]
    public Color activeColor = Color.white; 
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Start()
    {
        if (tabButtons.Length > 0) SetHighlight(tabButtons[0]);
    }

    public void SetHighlight(Button clickedButton)
    {
        foreach (Button btn in tabButtons)
        {
            // Получаем доступ к блоку Color Tint нашей кнопки
            ColorBlock cb = btn.colors;

            if (btn == clickedButton)
            {
                // Активная вкладка: задаем цвет для выключенного состояния
                cb.disabledColor = activeColor; 
                btn.colors = cb; // Применяем изменения
                btn.interactable = false; // Выключаем кнопку
            }
            else
            {
                // Неактивные вкладки: задаем цвет для обычного состояния
                cb.normalColor = inactiveColor;
                cb.selectedColor = inactiveColor; // Защита от системного "залипания" фокуса Unity
                btn.colors = cb; // Применяем изменения
                btn.interactable = true; // Включаем кнопку
            }
        }
    }
}
