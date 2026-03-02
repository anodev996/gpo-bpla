using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[Serializable]
[DeclareHorizontalGroup("Matrix")]
[DeclareVerticalGroup("Matrix/Column0")]
[DeclareVerticalGroup("Matrix/Column1")]
public class CubeMatrix<T>
{
    [Group("Matrix/Column0"), HideLabel]
    [SerializeField] private T m00; // (0,0)

    [Group("Matrix/Column0"), HideLabel]
    [SerializeField] private T m10; // (1,0)

    [Group("Matrix/Column1"), HideLabel]
    [SerializeField] private T m01; // (0,1)

    [Group("Matrix/Column1"), HideLabel]
    [SerializeField] private T m11; // (1,1)

    // Индексатор по плоскому индексу (0..3)
    public T this[int index]
    {
        get
        {
            switch (index)
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
            switch (index)
            {
                case 0: m00 = value; break;
                case 1: m10 = value; break;
                case 2: m01 = value; break;
                case 3: m11 = value; break;
            }
        }
    }

    // Индексатор по строке и столбцу
    public T this[int row, int col]
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

    public CubeMatrix(T a, T b, T c, T d)
    {
        m00 = a;
        m01 = b;
        m10 = c;
        m11 = d;
    }
}