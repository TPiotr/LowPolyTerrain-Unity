using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LTerrainTools))]
public class LTerrainToolsGUI : Editor
{
    private void OnSceneGUI()
    {
        LTerrainTools tools = (LTerrainTools)target;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            tools.LastRaycastHit = hit;
        }

        //input
        Event e = Event.current;
        
        int editor_hash = 12312;
        int controlID = GUIUtility.GetControlID(editor_hash, FocusType.Passive);
        if (e.type == EventType.Layout)
            HandleUtility.AddDefaultControl(controlID);
        
        tools.ProcessEvent(e);

        //update
        tools.Tick();
    }

    public override void OnInspectorGUI()
    {
        LTerrainTools terrain = (LTerrainTools)target;

        //base.DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();
        terrain.CurrentTool = GUILayout.SelectionGrid(terrain.CurrentTool, LTerrainTools.ALL_TOOLS, LTerrainTools.ALL_TOOLS.Length);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        if(GUILayout.Button("Connect chunk edges!"))
        {
            terrain.ConnectChunksEdges();
        }

        terrain.CurrentRadius = EditorGUILayout.Slider("Radius", terrain.CurrentRadius, .10f, 10f);
        EditorGUILayout.EndVertical();

        if (terrain.CurrentTool == LTerrainTools.SCULPT_TOOL)
        {
            EditorGUILayout.BeginVertical();
            terrain.SculptStrenght = EditorGUILayout.Slider("Strenght", terrain.SculptStrenght, 0f, 1f);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            terrain.UpDirection = (GUILayout.SelectionGrid((terrain.UpDirection) ? 0 : 1, new string[] { "Up", "Down" }, 2) == 0) ? true : false;
            EditorGUILayout.EndHorizontal();
        }
        else if(terrain.CurrentTool == LTerrainTools.SMOOTH_TOOL)
        {
            EditorGUILayout.BeginVertical();
            terrain.SmoothStrenght = EditorGUILayout.Slider("Strenght", terrain.SmoothStrenght, 0f, 1f);
            EditorGUILayout.EndVertical();
            
        }
        else if(terrain.CurrentTool == LTerrainTools.COLOR_TOOL)
        {
            EditorGUILayout.BeginVertical();
            terrain.PaintingStrenght = EditorGUILayout.Slider("Strenght", terrain.PaintingStrenght, 0f, 1f);
            terrain.PaintingColor = EditorGUILayout.ColorField("Color", terrain.PaintingColor);
            EditorGUILayout.EndVertical();
        }
    }
}
