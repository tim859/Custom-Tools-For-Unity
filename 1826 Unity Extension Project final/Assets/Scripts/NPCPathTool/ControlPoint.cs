using System;
using UnityEngine;

[Serializable]
public class ControlPoint
{
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 backTangent;
    [SerializeField] Vector3 frontTangent;

    // control points always require a position point, a front tangent point and a back tangent point
    public ControlPoint(Vector3 initialPosition)
    {
        position = initialPosition;
        frontTangent = Vector3.one;
        backTangent = -Vector3.one;
    }
    
    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 newPos)
    {
        position = newPos;
    }

    public Vector3 GetBackTangent()
    {
        return backTangent;
    }

    public void SetBackTangent(Vector3 newBackTangent)
    {
        backTangent = newBackTangent;
    }

    public Vector3 GetFrontTangent()
    {
        return frontTangent;
    }

    public void SetFrontTangent(Vector3 newFrontTangent)
    {
        frontTangent = newFrontTangent;
    }
}
