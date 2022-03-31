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
        SoulsLightComponent light = (SoulsLightComponent)target;
        GUILayout.Label("LIGHT");

        light.l.Unk1C = EditorGUILayout.Toggle("Unk1C", light.l.Unk1C);
        light.specularColor = EditorGUILayout.ColorField("Specular Color", light.specularColor);
        light.l.CastShadows = EditorGUILayout.Toggle("Cast Shadows", light.l.CastShadows);
        light.l.SpecularPower = EditorGUILayout.FloatField("Specular Power", light.l.SpecularPower);
        light.l.Unk30 = EditorGUILayout.FloatField("Unk30", light.l.Unk30);
        light.l.Unk34 = EditorGUILayout.FloatField("Unk34", light.l.Unk34);


    }
}
