using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SoulsFormats;

[CustomEditor(typeof(SoulsLightComponent))]
public class SoulsLightComponentEditor : Editor
{

    private void OnEnable() {
        SoulsLightComponent l = (SoulsLightComponent)target;
    }

    public override void OnInspectorGUI() {

        //DrawDefaultInspector();

        serializedObject.Update();

        //Unk00 = br.ReadBytes(16);
        //Name = br.GetUTF16(namesStart + br.ReadVarint());
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Type>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk1C>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("diffuseColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<DiffusePower>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("specularColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<CastShadows>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<SpecularPower>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<ConeAngle>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk30>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk34>k__BackingField"));
        //Position = br.ReadVector3();
        //Rotation = br.ReadVector3();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk50>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk54>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Radius>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk5C>k__BackingField"));
        //Unk64 = br.ReadBytes(4); ALWAYS 00 00 00 01
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk68>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk70>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<FlickerIntervalMin>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<FlickerIntervalMax>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<FlickerBrightnessMult>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk80>k__BackingField"));
        //Unk84 = br.ReadBytes(4); ALWAYS 00 00 00 00
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk88>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk90>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Unk98>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<NearClip>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unkA00"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unkA01"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unkA02"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unkA03"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Sharpness>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkAC>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<Width>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkBC>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSomethingA"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSomethingB"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSomethingC"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSomethingD"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkC4>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkC8>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkCC>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkD0>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkD4>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkD8>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkDC>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkE0>k__BackingField"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("l.<UnkE4>k__BackingField"));

        serializedObject.ApplyModifiedProperties();
        ((SoulsLightComponent)target).UpdateLight();
     }
}
