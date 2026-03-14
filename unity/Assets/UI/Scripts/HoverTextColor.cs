using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI textToChange;
    
    [Header("Активное состояние (Interactable = True)")]
    public Color normalColor = Color.white;        // Обычный цвет текста
    public Color hoverColor = Color.black;         // Цвет при наведении

    [Header("Неактивное состояние (Interactable = False)")]
    public Color disabledColor = Color.gray;       // Цвет выключенной кнопки
    public Color disabledHoverColor = Color.red;   // Цвет при наведении на выключенную кнопку

    private Button _button;
    private bool _isHovered = false;

    void Start()
    {
        _button = GetComponent<Button>();
        UpdateTextColor(); // Устанавливаем правильный цвет при старте
    }

    void Update()
    {
        // Постоянно проверяем состояние, так как друг может выключить кнопку из своего скрипта в любой момент
        UpdateTextColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        UpdateTextColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        UpdateTextColor();
    }

    // Главная логика распределения цветов
    private void UpdateTextColor()
    {
        if (_button == null || textToChange == null) return;

        if (_button.interactable)
        {
            // Если кнопка включена
            textToChange.color = _isHovered ? hoverColor : normalColor;
        }
        else
        {
            // Если кнопка выключена
            textToChange.color = _isHovered ? disabledHoverColor : disabledColor;
        }
    }
}