using UnityEngine;
using UnityEditor;

// position point refers to the red handle used to change the position of a control point
// tangent point refers to the blue handle used to change the two tangent points for a position point
// control point refers to all three of these linked together and moving together
[CustomEditor(typeof(NPCPath))]
public class NPCPathEditor : Editor
{
    Path path;
    SerializedProperty property;
    Vector3 snap = Vector3.one * 0.5f;
    Quaternion rotation = Quaternion.identity;
    readonly Handles.CapFunction sphereCap = Handles.SphereHandleCap;
    const float positionHandleSize = 0.6f;
    const float tangentHandleSize = 0.4f;

    void OnSceneGUI()
    {
        NPCPath npcPath = (NPCPath)target;
        Event e = Event.current;
        
        if (npcPath.closeLoop)
        {
            path.SetLoop(true);
        }
        else
        {
            path.SetLoop(false);
        }

        // iterate through control points
        for (int i = 0; i < path.GetNumOfControlPoints(); i++)
        {
            Vector3 controlPointPosition = path.points[i].GetPosition();
            // sets y value of controlPointPosition to 0 if the user has selected top down mode
            if (npcPath.topDown)
            {
                controlPointPosition = new Vector3(controlPointPosition.x, 0, controlPointPosition.z);
            }
            Handles.color = Color.red;
            // calculates the new position point handle position based on user input
            Vector3 newPosition = Handles.FreeMoveHandle(controlPointPosition, rotation, positionHandleSize, snap, sphereCap);

            // functionality for adding control points, hold shift and left click on an existing control point to add a new control point at that point in the path
            if (e.shift)
            {
                if (Handles.Button(controlPointPosition, rotation, positionHandleSize, positionHandleSize, sphereCap))
                {
                    path.InsertPoint(i, controlPointPosition + new Vector3(positionHandleSize, 0, 0));
                }
            }

            // functionality for deleting control points, hold ctrl and left click on a control point to delete that specific control point
            if (e.control)
            {
                if (Handles.Button(controlPointPosition, rotation, positionHandleSize, positionHandleSize, sphereCap))
                {
                    // don't allow there to be less than 2 control points, otherwise it's not a path
                    if (path.GetNumOfControlPoints() <= 2)
                    {
                        Debug.Log("Cannot delete any more control points, there must be at least 2.");
                    }
                    else
                    {
                        path.RemovePoint(i);
                    }
                }
            }

            // if the position of the control point has not changed, we don't want to do unecessary operations
            if (controlPointPosition != newPosition)
            {
                // updates the position point in the path as well
                path.MovePositionPoint(i, newPosition);
                // required for the tangent point calculations below
                controlPointPosition = newPosition;
            }

            // ----- handles new position of the back tangent handle -----

            // ensuring that the back tangent point will be moved with its linked control point
            Vector3 backTangent = controlPointPosition + path.points[i].GetBackTangent();

            // sets y value of backTangent to 0 if the user has selected top down mode
            if (npcPath.topDown)
            {
                backTangent = new Vector3(backTangent.x, 0, backTangent.z);
            }
            
            // the lines between the position point and the tangent points are black
            Handles.color = Color.black;
            // draws the line between the position point and the tangent point
            Handles.DrawLine(controlPointPosition, backTangent);
            // tangent handles are blue
            Handles.color = Color.blue;
            // calculates the new back tangent handle position based on user input
            Vector3 newBackTangent = Handles.FreeMoveHandle(backTangent, rotation, tangentHandleSize, snap, sphereCap);

            // if the position of the control point has not changed, we don't want to do unecessary operations
            if (backTangent != newBackTangent)
            {
                path.MoveBackTangent(i, newBackTangent - controlPointPosition);
            }

            // ----- handles new position of the front tangent handle -----
            
            // NOTE: this section is repeated code, should be put into a function called HandleNewTangentPosition() or something similar

            Vector3 frontTangent = controlPointPosition + path.points[i].GetFrontTangent();

            if (npcPath.topDown)
            {
                frontTangent = new Vector3(frontTangent.x, 0, frontTangent.z);
            }

            Handles.color = Color.black; 
            Handles.DrawLine(controlPointPosition, frontTangent);
            Handles.color = Color.blue;
            Vector3 newFrontTangent = Handles.FreeMoveHandle(frontTangent, rotation, tangentHandleSize, snap, sphereCap);

            if (frontTangent != newFrontTangent)
            {
                path.MoveFrontTangent(i, newFrontTangent - controlPointPosition);
            }
        }

        // draw the bezier curves
        for (int i = 0; i < path.GetNumOfSegments(); i++)
        {
            Vector3[] points = path.GetBezierPointsInSegment(i);

            if (npcPath.topDown)
            {
                points[0] = new Vector3(points[0].x, 0, points[0].z);
                points[1] = new Vector3(points[1].x, 0, points[1].z);
                points[2] = new Vector3(points[2].x, 0, points[2].z);
                points[3] = new Vector3(points[3].x, 0, points[3].z);
            }

            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }
    }

    private void OnEnable()
    {
        NPCPath npcPath = (NPCPath)target;
        path = npcPath.path ?? npcPath.CreatePath();
        // serialise the path so it is retained after deselection
        property = serializedObject.FindProperty("path");
    }
}
