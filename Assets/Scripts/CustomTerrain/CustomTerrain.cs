using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour
{
  public Vector2 randomHeightRange = new Vector2(0, 0.1f);
  public Texture2D heightMapImage;
  public Vector3 heightMapScale = new Vector3(1, 1, 1);

  // Perlin Noise
  public float perlinXScale = 0.01f;
  public float perlinYScale = 0.01f;
  public int perlinOffsetX = 0;
  public int perlinOffsetY = 0;
  public int perlinOctaves = 3;
  public float perlinPersistance = 8f;
  public float perlinHeightScale = 0.09f;

  public Terrain terrain;
  public TerrainData terrainData;

  private void OnEnable() 
  {
    Debug.Log("Initialising Terrain Data");
    terrain = this.GetComponent<Terrain>();
    terrainData = Terrain.activeTerrain.terrainData;
  }
  private void Reset() 
  {
    SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
    SerializedProperty tagsProp = tagManager.FindProperty("tags");
    
    AddTag(tagsProp, "Terrain");
    AddTag(tagsProp, "Cloud");
    AddTag(tagsProp, "Shore");

    tagManager.ApplyModifiedProperties();

    this.gameObject.tag = "Terrain";
  }

  private void AddTag(SerializedProperty tagsProp, string newTag)
  {
    bool found = false;
    for (int i = 0; i < tagsProp.arraySize; i++)
    {
      SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
      if(t.stringValue.Equals(newTag))
      {
        found = true;
        break;
      }      
    }

    if (!found)
    {
      tagsProp.InsertArrayElementAtIndex(0);
      SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
      newTagProp.stringValue = newTag;
    }
  }

  public void RandomTerrain()
  {
    float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    for (int x = 0; x < terrainData.heightmapResolution; x++)
    {
      for (int z = 0; z < terrainData.heightmapResolution; z++)
      {
        heightMap[x, z] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void LoadTexture()
  {
    float[,] heightMap;
    heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
    for (int x = 0; x < terrainData.heightmapResolution; x++)
    {
      for (int z = 0; z < terrainData.heightmapResolution; z++)
      {
        heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
      }
    }

    terrainData.SetHeights(0,0, heightMap);
  }

  public void Perlin()
  {
    float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    for (int y = 0; y < terrainData.heightmapResolution; y++)
    {
      for (int x = 0; x < terrainData.heightmapResolution; x++)
      {
        heightMap[x, y] = Mathf.PerlinNoise((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale);
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void FBM()
  {
    float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    for (int y = 0; y < terrainData.heightmapResolution; y++)
    {
      for (int x = 0; x < terrainData.heightmapResolution; x++)
      {
        heightMap[x, y] = Utils.fBM((x + perlinOffsetX) * perlinXScale,
                                    (y + perlinOffsetY) * perlinYScale,
                                    perlinOctaves,
                                    perlinPersistance) * perlinHeightScale;
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void ResetTerrain()
  {
    float[,] heightMap;
    heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
    for (int x = 0; x < terrainData.heightmapResolution; x++)
    {
      for (int z = 0; z < terrainData.heightmapResolution; z++)
      {
        heightMap[x, z] = 0;
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }  
}
