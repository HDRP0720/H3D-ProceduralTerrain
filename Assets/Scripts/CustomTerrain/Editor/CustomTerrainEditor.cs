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
  SerializedProperty heightMapImage;
  SerializedProperty heightMapScale;

  SerializedProperty perlinXScale;
  SerializedProperty perlinYScale;
  SerializedProperty perlinOffsetX;
  SerializedProperty perlinOffsetY;
  SerializedProperty perlinOctaves;
  SerializedProperty perlinPersistance;
  SerializedProperty perlinHeightScale;

  bool showRandom = false;
  bool showLoadHeights = false;
  bool showPerlinNoise = false;
  bool showfBM = false;

  private void OnEnable() 
  {
    randomHeightRange = serializedObject.FindProperty("randomHeightRange");
    heightMapImage = serializedObject.FindProperty("heightMapImage");
    heightMapScale = serializedObject.FindProperty("heightMapScale");

    perlinXScale = serializedObject.FindProperty("perlinXScale");
    perlinYScale = serializedObject.FindProperty("perlinYScale");
    perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
    perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
    perlinOctaves = serializedObject.FindProperty("perlinOctaves");
    perlinPersistance = serializedObject.FindProperty("perlinPersistance");
    perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
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

    showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
    if (showLoadHeights)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Load Heights From Texture", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(heightMapImage);
      EditorGUILayout.PropertyField(heightMapScale);
      if (GUILayout.Button("Load Texture"))
      {
        terrain.LoadTexture();
      }
    }

    showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
    if (showPerlinNoise)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
      EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
      EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
      EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
      EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
      if (GUILayout.Button("Generate Perlin Noise"))
      {
        terrain.Perlin();
      }
    }

    showfBM = EditorGUILayout.Foldout(showfBM, "Perlin Noise-fBM");
    if (showfBM)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("fBM(Fractal Brownian Motion)", EditorStyles.boldLabel);
      EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
      EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
      EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
      EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
      EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
      EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
      EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
      if (GUILayout.Button("Generate fBM Perlin Noise"))
      {
        terrain.FBM();
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
