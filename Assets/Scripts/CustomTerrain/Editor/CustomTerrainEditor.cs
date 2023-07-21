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

  SerializedProperty addPrevTerrainHeight;

  // Perlin Noise Parameters
  SerializedProperty perlinXScale;
  SerializedProperty perlinYScale;
  SerializedProperty perlinOffsetX;
  SerializedProperty perlinOffsetY;
  SerializedProperty perlinOctaves;
  SerializedProperty perlinPersistance;
  SerializedProperty perlinHeightScale;

  GUITableState perlinParameterTable;
  SerializedProperty perlinParameters;

  // Voronoi Parameters
  SerializedProperty voronoiPeakCount;
  SerializedProperty voronoiFallOff;
  SerializedProperty voronoiDropOff;
  SerializedProperty voronoiMinHeight;
  SerializedProperty voronoiMaxHeight;
  SerializedProperty voronoiType;

  bool showRandom = false;
  bool showLoadHeights = false;
  bool showPerlinNoise = false;
  bool showfBM = false;
  bool showMultiplePerlin = false;
  bool showVoronoi = false;

  private void OnEnable() 
  {
    randomHeightRange = serializedObject.FindProperty("randomHeightRange");
    heightMapImage = serializedObject.FindProperty("heightMapImage");
    heightMapScale = serializedObject.FindProperty("heightMapScale");

    addPrevTerrainHeight = serializedObject.FindProperty("addPrevTerrainHeight");

    // Perlin Noise Parameters
    perlinXScale = serializedObject.FindProperty("perlinXScale");
    perlinYScale = serializedObject.FindProperty("perlinYScale");
    perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
    perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
    perlinOctaves = serializedObject.FindProperty("perlinOctaves");
    perlinPersistance = serializedObject.FindProperty("perlinPersistance");
    perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");

    perlinParameterTable = new GUITableState("perlinParameterTable");
    perlinParameters = serializedObject.FindProperty("perlinParameters");

    // Voronoi Parameters
    voronoiPeakCount = serializedObject.FindProperty("peakCount");
    voronoiFallOff = serializedObject.FindProperty("fallOff");
    voronoiDropOff = serializedObject.FindProperty("dropOff");
    voronoiMinHeight = serializedObject.FindProperty("minHeight");
    voronoiMaxHeight = serializedObject.FindProperty("maxHeight");
    voronoiType = serializedObject.FindProperty("voronoiType");
  }

  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    CustomTerrain terrain = (CustomTerrain)target;

    EditorGUILayout.PropertyField(addPrevTerrainHeight);

    showRandom = EditorGUILayout.Foldout(showRandom, "Random");
    if(showRandom)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(randomHeightRange);
      if (GUILayout.Button("Random Heights"))
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

    showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise with fBM");
    if (showMultiplePerlin)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Multiple Perlin Noise with fBM", EditorStyles.boldLabel);

      perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, serializedObject.FindProperty("perlinParameters"));
      
      GUILayout.Space(20);

      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("+"))
      {
        terrain.AddNewPerlin();
      }
      if (GUILayout.Button("-"))
      {
        terrain.RemovePerlin();
      }
      EditorGUILayout.EndHorizontal();

      if (GUILayout.Button("Apply Multiple Perlin Noise"))
      {
        terrain.MultiplePerlin();
      }
    }

    showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
    if (showVoronoi)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Voronoi", EditorStyles.boldLabel);
      EditorGUILayout.IntSlider(voronoiPeakCount, 1, 10, new GUIContent("Peak Count"));
      EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("Fall Off"));
      EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("Drop Off"));
      EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
      EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
      EditorGUILayout.PropertyField(voronoiType);
      if (GUILayout.Button("Voronoi"))
      {        
        terrain.VoronoiTessellation();
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