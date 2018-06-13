using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LTerrain))]
public class LTerrainGUI : Editor {

    public override void OnInspectorGUI()
    {
        LTerrain terrain = (LTerrain) target;

        base.DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Create"))
        {
            HeightsGenerator generator = terrain.gameObject.GetComponent<HeightsGenerator>();

            if (generator == null)
                Debug.Log("Heights generator object not found, just add generator as mono behaviour to object where LTerrain script is added!");

            terrain.CreateTerrain(generator);//new TestWorldheightsGenerator());
        }
        else if(GUILayout.Button("Delete terrain"))
        {
            terrain.Delete();
        }

        EditorGUILayout.EndHorizontal();
    }
}
