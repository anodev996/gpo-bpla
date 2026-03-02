using System;
using System.Collections;
using TriInspector;
using UnityEngine;

[Serializable]
[DeclareTabGroup("Main")]
public class PIDController
{
    [field: SerializeField, Group("Main"), Tab("Const")] public float kp { get;private set; }
    [field: SerializeField, Group("Main"), Tab("Const")] public float ki { get; private set; }
    [field: SerializeField, Group("Main"), Tab("Const")] public float kd { get; private set; }
    [field: SerializeField, Group("Main"), Tab("Realtime"), ReadOnly] public float integral { get; private set; }
    [field: SerializeField, Group("Main"), Tab("Realtime"), ReadOnly] public float prevError { get; private set; }
    public PIDController(float kp, float ki, float kd)
    {
        this.kp = kp;
        this.ki = ki;
        this.kd = kd;
        integral = 0f;
        prevError = 0f;
    }
    public float output { get; private set; }
    public float Update(float error, float dt)
    {
        if (dt == 0)
            return 0;
        integral += error * dt;
        float derivative = (error - prevError) / dt;
        output = kp * error + ki * integral + kd * derivative;
        prevError = error;
        return output;
    }

    public float Calc(float current, float target) => Update(target - current, Time.deltaTime);

    public void Reset()
    {
        integral = 0f;
        prevError = 0f;
    }
}