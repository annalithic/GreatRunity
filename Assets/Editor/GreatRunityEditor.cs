using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GreatRunity))]
public class GreatRunityEditor : Editor {
    public override void OnInspectorGUI() {

        GreatRunity g = (GreatRunity)target;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Game Path: ", GUILayout.ExpandWidth(false));
        g.gamePath = EditorGUILayout.TextField(g.gamePath);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Map:");
        GUILayout.FlexibleSpace();

        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 12;

        g.map1 = EditorGUILayout.IntField("m", g.map1, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        EditorGUIUtility.labelWidth = 8;
        g.map2 = EditorGUILayout.IntField("_", g.map2, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        g.map3 = EditorGUILayout.IntField("_", g.map3, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        g.map4 = EditorGUILayout.IntField("_", g.map4, GUILayout.Width(42));

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        EditorGUILayout.EndHorizontal();

        
        using (new EditorGUI.DisabledScope(!File.Exists(g.GetPath()))) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import models")) g.ImportMapModels();
            if (GUILayout.Button("Import map")) g.ImportMap();
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(g.map1 != 60 || g.map4 != 0)) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Import parent models")) ;
                if (GUILayout.Button("Import parent maps"));
                EditorGUILayout.EndHorizontal();
            }
        }


        
        

    }
}