using System.Collections;
using UnityEngine;


public class Wing : ComponentBody
{
    [Header("Основные параметры")]
    public int count = 3;
    /// <summary>
    /// Модификация размера крыла от изначального
    /// </summary>
    public float scale = 1;
    /// <summary>
    /// Радиус крыла
    /// </summary>
    public float radius = 0.2529f;
    /// <summary>
    /// максимальная частота
    /// </summary>
    public int maxFrequency = 4000;
    public float frequency = 0;
    /// <summary>
    /// Коэффициент подъемной силы профиля лопасти
    /// </summary>
    public BladeProfile profile;
    /// <summary>
    /// Геометрический шаг винта (м). 
    /// Важный параметр, показывающий, насколько винт "закручен". 
    /// В простейшем случае для симуляции его можно принять пропорциональным диаметру: 
    /// H = (шаг/диаметр) × D
    /// </summary>
    public float screwPitch = 0.5f;
    [Header("Коэфициенты")]
    /// <summary>
    /// Интегральный коэффициент (≈0.75). Учитывает, что тяга распределена по лопасти неравномерно
    /// </summary>
    public float integralCoefficient = 0.75f;
    /// <summary>
    /// Коэффициент крутки (≈0.95). Учитывает, что угол установки лопасти меняется по радиусу
    /// </summary>
    public float twistRatioCoefficient = 0.95f;
    /// <summary>
    /// Коэффициент заполнения(≈0.85). Учитывает затенение втулкой и комлями лопастей
    /// </summary>
    public float fillFactorCoefficient = 0.85f;
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject[] visualizeWinds;
    /// <summary>
    /// Ометаемая площадь
    /// </summary>
    /// <returns></returns>
    public float GetSwerptSquare()
    {
        return (0.1f * Mathf.PI * Mathf.Pow(GetDiameter(), 2)) / (4 * count);
    }
    public float GetDiameter()
    {
        return scale * radius * 2;
    }
    /// <summary>
    /// Коэфициент зависимости от количества лопастей
    /// </summary>
    /// <returns></returns>
    public float GetBladeCountCoef()
    {
        return Mathf.Pow(count, 0.677f);
    }
    /// <summary>
    /// Коэфициент зависимости от профиля лопастей
    /// </summary>
    /// <returns></returns>
    public float GetBladeProfileCoef()
    {
        switch(profile)
        {
            default:
            case BladeProfile.Biconvex: return 0.9f;
            case BladeProfile.FlatConvex: return 1.1f;
            case BladeProfile.ConcaveСonvex: return 1.4f;
        }
    }

    [ContextMenu("VisualizeUpdate")]
    public void VisualizeUpdate()
    {
        VisualizeClear();

        var angle = 360 / count;
        visualizeWinds = new GameObject[count];
        for(int i = 0; i < count; i++)
            visualizeWinds[i] = CreateVisualizeWing(i * angle);
    }
    public void VisualizeClear()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private GameObject CreateVisualizeWing(float angle)
    {
        var obj = Instantiate(prefab);
        obj.transform.parent = transform;
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localRotation = Quaternion.Euler(0, angle, 0);
        obj.transform.localScale = new Vector3(scale, scale, scale);
        return obj;
    }    
    public override void OnUpdate(Body body)
    {
        for(int i = 0; i < count;i++)
        {
            visualizeWinds[i].transform.Rotate(0, 2 * Mathf.PI * frequency, 0);
        }
        body.AddForceAtPosition(3f * GetBladeCountCoef() * GetBladeProfileCoef() * screwPitch * (Mathf.Pow(GetDiameter(), 4) / count) * integralCoefficient * twistRatioCoefficient * fillFactorCoefficient * frequency * transform.up
                                ,transform.position);
    }

    [System.Serializable]
    public enum BladeProfile
    {
        /// <summary>
        /// Вогнуто-выпуклый
        /// </summary>
        ConcaveСonvex,
        /// <summary>
        /// Плоско-выпуклый
        /// </summary>
        FlatConvex,
        /// <summary>
        /// Двувыпуклый
        /// </summary>
        Biconvex
    }
}