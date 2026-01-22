using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<ControlPoint> points;

    bool loop;

    // a path should always have at least two control points so that it can be defined as a path
    public Path(Vector3 position) 
    {
        points = new List<ControlPoint>
        {
            new ControlPoint(position),
            new ControlPoint(position + Vector3.right)
        };
    }

    public void SetLoop(bool newLoop)
    {
        loop = newLoop;
    }

    public int GetNumOfControlPoints()
    {
        return points.Count;
    }

    public int GetNumOfSegments()
    {
        if (loop)
        {
            return points.Count;
        }
        else
        {
            return points.Count - 1;
        }
    }
    public void MovePositionPoint(int i, Vector3 position)
    {
        points[i].SetPosition(position);
    }

    public void MoveBackTangent(int i, Vector3 position)
    {
        points[i].SetBackTangent(position);
    }

    public void MoveFrontTangent(int i, Vector3 position)
    {
        points[i].SetFrontTangent(position);
    }

    public void InsertPoint(int index, Vector3 position)
    {
        points.Insert(index, new ControlPoint(position));
    }

    public void RemovePoint(int index)
    {
        points.RemoveAt(index);
    }

    public Vector3[] GetBezierPointsInSegment(int i)
    {
        ControlPoint backControlPoint = points[i];
        ControlPoint frontControlPoint;
        if (loop && i == points.Count - 1)
        {
            frontControlPoint = points[0];
        }
        else
        {
            frontControlPoint = points[i + 1];
        }

        Vector3[] bezierPoints = new Vector3[4]
        {
            backControlPoint.GetPosition(),
            backControlPoint.GetPosition() + backControlPoint.GetFrontTangent(),
            frontControlPoint.GetPosition() + frontControlPoint.GetBackTangent(),
            frontControlPoint.GetPosition()
        };
        return bezierPoints;
    }
}
