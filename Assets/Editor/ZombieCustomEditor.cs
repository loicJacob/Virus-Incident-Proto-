
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Zombie))]
public class ZombieCustomEditor : Editor
{
    private Zombie script;

    private bool isChasing = false;
    private bool isToggled = false;
    private bool isUnToggled = false;

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

        if (Application.isPlaying)
        {
            isChasing = GUILayout.Toggle(isChasing, "Chase Target", GUI.skin.button);

            if (isChasing && !isToggled)
            {
                isToggled = true;
                isUnToggled = false;
                script.EditorTestChase(true);
            }
            else if (!isChasing && !isUnToggled)
            {
                isUnToggled = true;
                isToggled = false;
                script.EditorTestChase(false);
            }
        }

        GUILayout.Space(5);
        base.OnInspectorGUI();
    }
}
