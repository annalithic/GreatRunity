using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SoulsFormats;

[CustomEditor(typeof(SoulsLightComponent))]
[CanEditMultipleObjects]
public class SoulsLightComponentEditor : Editor
{
    static string[] propertyNames = {
        "l.<Type>k__BackingField",
        "l.<Unk1C>k__BackingField",
        "diffuseColor",
        "l.<DiffusePower>k__BackingField",
        "specularColor",
        "l.<CastShadows>k__BackingField",
        "l.<SpecularPower>k__BackingField",
        "l.<ConeAngle>k__BackingField",
        "l.<Unk30>k__BackingField",
        "l.<Unk34>k__BackingField",
        "l.<Unk50>k__BackingField",
        "l.<Unk54>k__BackingField",
        "l.<Radius>k__BackingField",
        "l.<Unk5C>k__BackingField",
        "l.<Unk68>k__BackingField",
        "shadowColor",
        "l.<Unk70>k__BackingField",
        "l.<FlickerIntervalMin>k__BackingField",
        "l.<FlickerIntervalMax>k__BackingField",
        "l.<FlickerBrightnessMult>k__BackingField",
        "l.<Unk80>k__BackingField",
        "l.<Unk88>k__BackingField",
        "l.<Unk90>k__BackingField",
        "l.<Unk98>k__BackingField",
        "l.<NearClip>k__BackingField",
        "unkA00",
        "unkA01",
        "unkA02",
        "unkA03",
        "l.<Sharpness>k__BackingField",
        "l.<UnkAC>k__BackingField",
        "l.<Width>k__BackingField",
        "l.<UnkBC>k__BackingField",
        "disableSomethingA",
        "disableSomethingB",
        "disableSomethingC",
        "disableSomethingD",
        "l.<UnkC4>k__BackingField",
        "l.<UnkC8>k__BackingField",
        "l.<UnkCC>k__BackingField",
        "l.<UnkD0>k__BackingField",
        "l.<UnkD4>k__BackingField",
        "l.<UnkD8>k__BackingField",
        "l.<UnkDC>k__BackingField",
        "l.<UnkE0>k__BackingField",
        "l.<UnkE4>k__BackingField",     
    };

    SerializedProperty[] properties;

    private void OnEnable() {
        properties = new SerializedProperty[propertyNames.Length];
        for(int i = 0; i < properties.Length; i++) {
            properties[i] = serializedObject.FindProperty(propertyNames[i]);
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        for (int i = 0; i < properties.Length; i++) EditorGUILayout.PropertyField(properties[i]);
        serializedObject.ApplyModifiedProperties();
        for(int i = 0; i < targets.Length; i++) {
            ((SoulsLightComponent)targets[i]).UpdateLight();
        }
    }
}
