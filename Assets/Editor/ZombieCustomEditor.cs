
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Zombie))]
public class ZombieCustomEditor : Editor
{
    private Zombie script;

    private void Awake()
    {
        script = (Zombie)target;
    }

    public override void OnInspectorGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("AI Test", labelStyle);

        GUILayout.BeginHorizontal();
        bool isPatroling = GUILayout.Button("Patrol");
        bool isChasing = GUILayout.Button("Chase");
        GUILayout.EndHorizontal();

        if (Application.isPlaying)
        {
            if (isPatroling)
                script.EditorTestPatrol();

            if (isChasing)
                script.EditorTestChase();
        }

        GUILayout.Space(5);
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        // Draw vision cone
        DrawCone(Color.yellow, script.transform.position, script.transform.eulerAngles.y, script.VisionAngle, script.VisionDistance);

        // Draw attack trigger
        Color color = Color.red;
        color.a = 0.2f;
        Handles.color = color;
        Handles.DrawSolidDisc(script.transform.position, Vector3.up, script.DistanceToAttack);
    }

    private void DrawCone(Color color, Vector3 originPos, float originEulerY, float angle, float range)
    {
        Vector3 viewAngle01 = DirectionFromAngle(originEulerY, -angle / 2);
        Vector3 arcStartPoint = (originPos + viewAngle01) - originPos;

        color.a = 0.2f;
        Handles.color = color;
        Handles.DrawSolidArc(originPos, Vector3.up, arcStartPoint, angle, range);
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleDegrees)
    {
        angleDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
}
