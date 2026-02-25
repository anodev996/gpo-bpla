using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Позволяет рисовать линии через GL-API.
/// </summary>
public class VisualizeUtils : Singleton<VisualizeUtils>
{
    public Material lineMaterial;

    private struct Line
    {
        public Vector3 start;
        public Vector3 end;
        public Color color;
    }

    private List<Line> lines = new List<Line>();

    private readonly object linesLock = new object();

    void Start()
    {
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color) => Instance.drawLine(start, end, color);
    /// <summary>
    /// Добавляет линию для отрисовки в текущем кадре.
    /// </summary>
    /// <param name="start">Точка начала (мировые координаты)</param>
    /// <param name="end">Точка конца (мировые координаты)</param>
    /// <param name="color">Цвет линии</param>
    public void drawLine(Vector3 start, Vector3 end, Color color)
    {
        lock (linesLock)
        {
            lines.Add(new Line { start = start, end = end, color = color });
        }
    }


    void OnPostRender()
    {
        if (lineMaterial == null) return;

        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        lock (linesLock)
        {
            foreach (var line in lines)
            {
                GL.Color(line.color);
                GL.Vertex3(line.start.x, line.start.y, line.start.z);
                GL.Vertex3(line.end.x, line.end.y, line.end.z);
            }
        }

        GL.End();

        lock (linesLock)
        {
            lines.Clear();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}