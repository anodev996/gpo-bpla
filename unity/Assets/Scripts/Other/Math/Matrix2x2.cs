using System;
using UnityEngine;
using TriInspector;
using Unity.VisualScripting;

[Serializable]
[DeclareHorizontalGroup("Matrix")]                     // Основная горизонтальная группа
[DeclareVerticalGroup("Matrix/Column0")]               // Первая вертикальная группа (столбец 0)
[DeclareVerticalGroup("Matrix/Column1")]               // Вторая вертикальная группа (столбец 1)
public struct Matrix2x2
{
    [Group("Matrix/Column0"), HideLabel]
    [SerializeField] private float m00; // элемент (0,0)

    [Group("Matrix/Column0"), HideLabel]
    [SerializeField] private float m10; // элемент (1,0)

    [Group("Matrix/Column1"), HideLabel]
    [SerializeField] private float m01; // элемент (0,1)

    [Group("Matrix/Column1"), HideLabel]
    [SerializeField] private float m11; // элемент (1,1)
    public float this[int x]
    {
        get
        {
            switch(x)
            {
                default:
                case 0: return m00; 
                case 1: return m10;
                case 2: return m01; 
                case 3: return m11;
            }
        }
        set
        {
            switch (x)
            {
                case 0: m00 = value; break;
                case 1: m10 = value; break;
                case 2: m01 = value; break;
                case 3: m11 = value; break;
            }
        }
    }
    public float this[int row, int col]
    {
        get
        {
            if (row == 0)
                return col == 0 ? m00 : m01;
            else
                return col == 0 ? m10 : m11;
        }
        set
        {
            if (row == 0)
            {
                if (col == 0) m00 = value;
                else m01 = value;
            }
            else
            {
                if (col == 0) m10 = value;
                else m11 = value;
            }
        }
    }

    public static implicit operator Matrix2x2(float a)
    {
        return new Matrix2x2(a, a, a, a);
    }

    public static Matrix2x2 operator *(Matrix2x2 lhz, Matrix2x2 rhz)
    {
        for(int i = 0; i < 4; i++)
        {
            lhz[i] *= rhz[i];
        }
        return lhz;
    }

    public static Matrix2x2 operator +(Matrix2x2 lhz, Matrix2x2 rhz)
    {
        for (int i = 0; i < 4; i++)
        {
            lhz[i] += rhz[i];
        }
        return lhz;
    }

    public void Clamp01() => Clamp(0, 1);

    public void Clamp(float min, float max)
    {
        for(int i = 0; i < 4; i++)
        {
            this[i] = Mathf.Clamp(this[i], min, max);
        }
    }

    public float Max()
    {
        float v = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            if(this[i] > v)
                v = this[i];
        }
        return v;
    }

    public void Norm()
    {
        float max = Mathf.Abs(Max());
        if (max == 0)
            return;
        for(int i = 0;i < 4; i++)
        {
            this[i] = this[i] / max;
        }
    }

    public Matrix2x2(float a, float b, float c, float d)
    {
        m00 = a;
        m01 = b;
        m10 = c;
        m11 = d;
    }
}