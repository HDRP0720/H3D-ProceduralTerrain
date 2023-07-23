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

  // Midpoint Displacement
  SerializedProperty minHeightForMPD;
  SerializedProperty maxHeightForMPD;
  SerializedProperty heightDampenerForMPD;
  SerializedProperty roughnessForMPD;

  // Smooth
  SerializedProperty smoothAmount;

  // Splat Maps
  GUITableState splatMapTable;
  SerializedProperty splatHeights;

  // Extract Height Map
  Texture2D hmTexture;

  // Vegetation
  SerializedProperty vegetation;
  SerializedProperty maxTrees;
  SerializedProperty treeSpacing;
  GUITableState vegetationTable;

  bool showRandom = false;
  bool showLoadHeights = false;
  bool showPerlinNoise = false;
  bool showfBM = false;
  bool showMultiplePerlin = false;
  bool showVoronoi = false;
  bool showMPD = false;
  bool showSmooth = false;
  bool showSplatMaps = false;
  bool showHeightMap = false;
  bool showVegetation = false;

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

    // Midpoint Displacement
    minHeightForMPD = serializedObject.FindProperty("minHeightForMPD");
    maxHeightForMPD = serializedObject.FindProperty("maxHeightForMPD");
    heightDampenerForMPD = serializedObject.FindProperty("heightDampenerForMPD");
    roughnessForMPD = serializedObject.FindProperty("roughnessForMPD");

    // Smooth
    smoothAmount = serializedObject.FindProperty("smoothAmount");

    // Splat Maps
    splatMapTable = new GUITableState("splatMapsTable");
    splatHeights = serializedObject.FindProperty("splatHeights");

    // Extract Height Map
    hmTexture = new Texture2D(513, 513, TextureFormat.ARGB32, false);

    // Vegetation
    vegetation = serializedObject.FindProperty("vegetation");
    maxTrees = serializedObject.FindProperty("maxTrees");
    treeSpacing = serializedObject.FindProperty("treeSpacing");
    vegetationTable = new GUITableState("vegetationTable");
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

    showMPD = EditorGUILayout.Foldout(showMPD, "Midpoint Displacement");
    if (showMPD)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField(new GUIContent(string.Format("Min Height: {0}", terrain.minHeightForMPD)));
      EditorGUILayout.LabelField(new GUIContent(string.Format("Min Height: {0}", terrain.maxHeightForMPD)));
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.MinMaxSlider(ref terrain.minHeightForMPD, ref terrain.maxHeightForMPD, -10, 10);
      EditorGUILayout.Space();
      // EditorGUILayout.PropertyField(minHeightForMPD);
      // EditorGUILayout.PropertyField(maxHeightForMPD);
      EditorGUILayout.PropertyField(heightDampenerForMPD);
      EditorGUILayout.PropertyField(roughnessForMPD);
      EditorGUILayout.Space();
      if (GUILayout.Button("MPD"))
      {
        terrain.MidPointDisplacement();
      }
    }

    showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrain");
    if(showSmooth)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("Smooth Amount"));
      if(GUILayout.Button("Smooth"))
      {
        terrain.SmoothAdvanced();
      }
    }

    showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
    if(showSplatMaps)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Splat Maps", EditorStyles.boldLabel);

      // EditorGUILayout.Slider(offsetForBlending, 0, 0.1f, new GUIContent("Blending Offset"));
      // EditorGUILayout.Slider(noiseXScaleForBlending, 0.001f, 1, new GUIContent("Noise X Scale"));
      // EditorGUILayout.Slider(noiseYScaleForBlending, 0.001f, 1, new GUIContent("Noise Y Scale"));
      // EditorGUILayout.Slider(noiseScalerForBlending, 0, 1, new GUIContent("Noise Scaler"));

      splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));
      GUILayout.Space(20);

      EditorGUILayout.BeginHorizontal();
      if(GUILayout.Button("+"))
      {
        terrain.AddNewSplatHeight();
      }
      if (GUILayout.Button("-"))
      {
        terrain.RemoveSplatHeight();
      }
      EditorGUILayout.EndHorizontal();
      if (GUILayout.Button("Apply SplatMaps"))
      {
        terrain.SplatMaps();
      }
    }

    showHeightMap = EditorGUILayout.Foldout(showHeightMap, "Height Map");
    if (showHeightMap)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      int wSize = (int)(EditorGUIUtility.currentViewWidth - 100);

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label(hmTexture, GUILayout.Width(wSize), GUILayout.Height(wSize));
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Refresh", GUILayout.Width(wSize)))
      {
        float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        for (int y = 0; y < terrain.terrainData.heightmapResolution; y++)
        {
          for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
          {
            hmTexture.SetPixel(x, y, new Color(heightMap[x, y], heightMap[x, y], heightMap[x, y], 1));
          }
        }
        hmTexture.Apply();
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
    }

    showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
    if (showVegetation)
    {
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Label("Vegetation", EditorStyles.boldLabel);
      EditorGUILayout.IntSlider(maxTrees, 1, 10000, new GUIContent("Maximum Trees"));
      EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Trees Spacing"));
      vegetationTable = GUITableLayout.DrawTable(vegetationTable, serializedObject.FindProperty("vegetation"));
      GUILayout.Space(20);

      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("+"))
      {
        terrain.AddNewVegetation();
      }
      if (GUILayout.Button("-"))
      {
        terrain.RemoveVegetation();
      }
      EditorGUILayout.EndHorizontal();
      if (GUILayout.Button("Apply Vegetation"))
      {
        terrain.PlantVegetation();
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