using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))] 
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
  SerializedProperty randomHeightRange;

  bool showRandom = false;
  private void OnEnable() 
  {
    randomHeightRange = serializedObject.FindProperty("randomHeightRange");
  }

  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    CustomTerrain terrain = (CustomTerrain)target;

    showRandom = EditorGUILayout.Foldout(showRandom, "Random");
    if(showRandom)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(randomHeightRange);
      if(GUILayout.Button("Random Heights"))
      {
        terrain.RandomTerrain();
      }
    }

    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    if (GUILayout.Button("Reset Terrain"))
    {
      terrain.ResetTerrain();
    }

    serializedObject.ApplyModifiedProperties();
  }
}
