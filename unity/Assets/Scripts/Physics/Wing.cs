using System.Collections;
using TriInspector;
using UnityEngine;

[DeclareTabGroup("Main")]
public class Wing : ComponentBody
{
    [Header("Основные параметры")]
    [Group("Main"), Tab("Основные параметры")] public int count = 3;
    [InfoBox("Модификация размера крыла от изначального")]
    [Group("Main"), Tab("Основные параметры")] public float scale = 1;
    [InfoBox("Радиус крыльев")]
    [Group("Main"), Tab("Основные параметры")] public float radius = 0.2529f;
    [Group("Main"), Tab("Основные параметры")] public float square = 0.0108f;
    [Group("Main"), Tab("Основные параметры")] public float frequency = 0;
    [Group("Main"), Tab("Основные параметры")] public BladeProfile profile;
    [Group("Main"), Tab("Основные параметры")] public Direction direction;
    [InfoBox("Геометрический шаг винта (м). \r\n Важный параметр, показывающий, насколько винт \"закручен\". \r\n В простейшем случае для симуляции его можно принять пропорциональным диаметру: \r\nH = (шаг/диаметр) × D")]
    [Group("Main"), Tab("Основные параметры")] public float screwPitch = 0.5f;
    [Header("Коэфициенты")]
    [InfoBox("Интегральный коэффициент. Учитывает, что тяга распределена по лопасти неравномерно")]
    [Group("Main"), Tab("Коэфициенты")] public float integralCoefficient = 0.75f;
    [InfoBox("Коэффициент крутки. Учитывает, что угол установки лопасти меняется по радиусу")]
    [Group("Main"), Tab("Коэфициенты")] public float twistRatioCoefficient = 0.95f;
    [InfoBox("Коэффициент заполнения. Учитывает затенение втулкой и комлями лопастей")]
    [Group("Main"), Tab("Коэфициенты")] public float fillFactorCoefficient = 0.85f;
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject[] visualizeWinds;
    [Header("Runtime")]
    [Group("Main"), Tab("Runtime")][ShowInInspector][ReadOnly] public float angularVelocity { get; private set; }
    [Group("Main"), Tab("Runtime")][ShowInInspector][ReadOnly] public float Fmagnitude { get; private set; }
    [Group("Main"), Tab("Runtime")][ShowInInspector][ReadOnly] public float angleStep {  get; private set; }
    /// <summary>
    /// Фактор заполненая диск 
    /// </summary>
    /// <returns></returns>
    public float DiskFillFactor()
    {
        return (4 * count * square) / (Mathf.PI * Mathf.Pow(GetDiameter(), 2));
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
        obj.transform.localScale = new Vector3(GetDirection() * scale, scale,GetDirection() * scale);
        return obj;
    }  
    /// <summary>
    /// Безразмерная величина зависящая от формы лопасти
    /// </summary>
    /// <returns></returns>
    public float GetCT()
    {
        return GetBladeCountCoef() * GetBladeProfileCoef() * integralCoefficient * twistRatioCoefficient * fillFactorCoefficient;
    }
    public int GetDirection()
    {
        switch(direction)
        {
            default:
            case Direction.Clockwise:
                return 1;
            case Direction.ReverseClockwise: 
                return -1;
        }
    }
    public override void OnUpdate(Body body)
    {
        float rotationSpeed = 2 * Mathf.PI * frequency; // рад/с
        angleStep = rotationSpeed * Time.fixedDeltaTime * Mathf.Rad2Deg;
        for (int i = 0; i < count; i++)
        {
            visualizeWinds[i].transform.Rotate(0, angleStep, 0);
        }

        float density = EnviromentSettings.GetDensity(body.height);
        float D = GetDiameter();
        float Ct = GetCT(); // комбинация коэффициентов
        float sigma = DiskFillFactor(); // (4*count*square)/(π*D²)
        float pitchRatio = screwPitch / D;
        Fmagnitude = 0.5f * Ct * sigma * density * pitchRatio * Mathf.Pow(D, 4) * Mathf.Pow(frequency, 2);
        Vector3 Ft = Fmagnitude * transform.up; 
        body.AddForceAtPosition(Ft, transform.position);

        float reactiveFactor = 0.08f;
        float M_reac_mag = reactiveFactor * Fmagnitude * D;
        Vector3 M_reac = GetDirection() * transform.up * M_reac_mag;
        body.AddTorque(M_reac);
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
    public enum Direction
    {
        Clockwise, ReverseClockwise
    }
}