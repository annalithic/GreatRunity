using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BTLComponent))]
public class BTLComponentEditor : Editor {
    public override void OnInspectorGUI() {
        BTLComponent b = (BTLComponent)target;
        GUILayout.Label($"{GreatRunity.MapName(b.m1, b.m2, b.m3, b.m4)} - version {b.version}");
        if (GUILayout.Button("Export Lights")) b.Export();
    }
}
