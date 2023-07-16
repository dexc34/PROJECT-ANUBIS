using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeleeAttacks)), CanEditMultipleObjects]
public class MeleeCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeleeAttacks script = (MeleeAttacks)target;

        base.OnInspectorGUI();

        //Add parry settings
        //Spacing
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();

        //Header
        EditorGUILayout.LabelField("Parry", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Can Parry?");
        script.canParry = EditorGUILayout.Toggle(script.canParry);
        EditorGUILayout.EndHorizontal();


        if (script.canParry)
        {
            //Parry range
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parry Range");
            script.parryRange = EditorGUILayout.FloatField(script.parryRange);
            EditorGUILayout.EndHorizontal();

            //Parry window
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parry Window");
            GenerateTooltip("How long of a window the player has to parry in seconds");
            script.parryWindow = EditorGUILayout.FloatField(script.parryWindow);
            EditorGUILayout.EndHorizontal();

            //Parry active time
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parry Active Time");
            GenerateTooltip("How long does the parry stay active for after reflecting its first projectile");
            script.parryActiveTime = EditorGUILayout.FloatField(script.parryActiveTime);
            EditorGUILayout.EndHorizontal();

            //Parry multiplier
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parry Multiplier");
            GenerateTooltip("How much a bullet's damage will be multiplied by after getting reflected");
            script.parryMultiplier = EditorGUILayout.FloatField(script.parryMultiplier);
            EditorGUILayout.EndHorizontal();
        }

    }
    private void GenerateTooltip(string text)
    {
        var propRect = GUILayoutUtility.GetLastRect();
        GUI.Label(propRect, new GUIContent("", text));
    }
}
