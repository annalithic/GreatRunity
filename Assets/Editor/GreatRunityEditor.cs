using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GreatRunity))]
public class GreatRunityEditor : Editor {

    GUIStyle boxStyle;

    public override void OnInspectorGUI() {

        float defaultLabelWidth = EditorGUIUtility.labelWidth;


        if (boxStyle == null) {
            boxStyle = new GUIStyle(GUI.skin.window);
            boxStyle.alignment = TextAnchor.UpperLeft;
        }

        GreatRunity g = (GreatRunity)target;

        EditorGUIUtility.labelWidth = 70;


        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Game Path: ", GUILayout.ExpandWidth(false));
        g.gamePath = EditorGUILayout.TextField("Game Path:", g.gamePath);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label(" ModPath:    ", GUILayout.ExpandWidth(false));
        g.modPath = EditorGUILayout.TextField("Mod Path:" ,g.modPath);
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Map:");
        GUILayout.FlexibleSpace();

        EditorGUIUtility.labelWidth = 12;

        g.map1 = (byte) EditorGUILayout.IntField("m", g.map1, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        EditorGUIUtility.labelWidth = 8;
        g.map2 = (byte)EditorGUILayout.IntField("_", g.map2, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        g.map3 = (byte)EditorGUILayout.IntField("_", g.map3, GUILayout.Width(42));
        //GUILayout.Label("_", GUILayout.ExpandWidth(false));
        g.map4 = (byte)EditorGUILayout.IntField("_", g.map4, GUILayout.Width(42));

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        //GUI.enabled = false;
        GUILayout.Label(g.GetMapName());
        //GUI.enabled = true;
        //EditorGUILayout.EndHorizontal();

        GUILayout.EndVertical();

        EditorGUILayout.Space();


        if (g.gamePath == null) return;

        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledScope(!File.Exists(g.GetPath()))) {
            
            if (GUILayout.Button("Import models", GUILayout.Width(100))) g.ImportMapModels();
            GUILayout.Label("Reimport", GUILayout.ExpandWidth(false));
            g.reimportModels = EditorGUILayout.Toggle(g.reimportModels, GUILayout.MaxWidth(16));
            GUILayout.Label("Combine Meshes", GUILayout.ExpandWidth(false));
            g.combineMeshes = EditorGUILayout.Toggle(g.combineMeshes, GUILayout.MaxWidth(16));

            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import msb", GUILayout.Width(100))) g.ImportMap();
        }
        using (new EditorGUI.DisabledScope(!File.Exists(g.GetLightPath()))) {
            if (GUILayout.Button("Import btl", GUILayout.Width(100))) g.ImportLights();
        }
        //EditorGUILayout.EndHorizontal();

        /*
        using (new EditorGUI.DisabledScope(g.map1 != 60 || g.map4 != 0)) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import parent models")) ;
            if (GUILayout.Button("Import parent maps"));
            EditorGUILayout.EndHorizontal();
        }
        */


        EditorGUIUtility.labelWidth = defaultLabelWidth;
    }
}
