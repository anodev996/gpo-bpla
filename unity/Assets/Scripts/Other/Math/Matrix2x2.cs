using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TriInspector;
using System;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public struct Matrix2x2
{
    [Header("Matrix")]
    [HideLabel][SerializeField] private Vector2 a;
    [HideLabel][SerializeField] private Vector2 b;

    public float this[int x,int y]
    {
        get
        {
            switch (y)
            {
                default:
                case 0:
                    return a[x];
                case 1:
                    return b[x];
            }
        }
        set
        {
            switch(y)
            {
                case 0:
                    a[x] = value;
                    break;
                case 1:
                    b[x] = value;
                    break;
            }
        }
    }

    public Matrix2x2(float a, float b, float c, float d)
    {
        this.a = new Vector2(a, b);
        this.b = new Vector2(c, d);
    }
}