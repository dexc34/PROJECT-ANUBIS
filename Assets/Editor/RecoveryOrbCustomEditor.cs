using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (RecoveryOrb)), CanEditMultipleObjects]
public class RecoveryOrbCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RecoveryOrb script = (RecoveryOrb) target;

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Can Respawn");
        script.canRespawn = EditorGUILayout.Toggle(script.canRespawn);
        GUILayout.EndHorizontal();

        if(script.canRespawn)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cooldown");
            GenerateTooltip("How long it takes for the orb to reappear");
            script.orbCooldown = EditorGUILayout.FloatField(script.orbCooldown);
            GUILayout.EndHorizontal();
        }

        //Draw recovery options
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Recovery Types");
        GenerateTooltip("What does the orb recover?");

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Health", GUILayout.MaxWidth(40));
        script.recoverHealth = EditorGUILayout.Toggle(script.recoverHealth);

        EditorGUILayout.LabelField("Stamina", GUILayout.MaxWidth(50));
        script.recoverStamina = EditorGUILayout.Toggle(script.recoverStamina);

        EditorGUILayout.LabelField("Ability", GUILayout.MaxWidth(40));
        script.recoverAbility = EditorGUILayout.Toggle(script.recoverAbility);

        EditorGUILayout.LabelField("Ammo", GUILayout.MaxWidth(40));
        script.recoverAmmo = EditorGUILayout.Toggle(script.recoverAmmo);

        GUILayout.EndHorizontal();

        //Draw health recovery options
        if(script.recoverHealth)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Health Recovery Settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health to Recover");
            script.healthToRecover = EditorGUILayout.FloatField(script.healthToRecover);
            GUILayout.EndHorizontal();
        }

        //Draw stamina recovery options
        if(script.recoverStamina)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stamina Recovery Settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stamina Bars to Recover");
            script.staminaToRecover = EditorGUILayout.IntField(script.staminaToRecover);
            GUILayout.EndHorizontal();
        }

        if(script.recoverAbility)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("No Ability Recovery Settings", EditorStyles.boldLabel);
        }

        if(script.recoverAmmo)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ammo Recovery Settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ammo % to Recover");
            script.ammoToRecoverPercent = EditorGUILayout.FloatField(script.ammoToRecoverPercent);
            script.ammoToRecoverPercent = Mathf.Clamp(script.ammoToRecoverPercent, 0, 100);
            GUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(target);
    }

    private void GenerateTooltip(string text)
    {
        var propRect = GUILayoutUtility.GetLastRect();
        GUI.Label(propRect, new GUIContent("", text));
    }
}