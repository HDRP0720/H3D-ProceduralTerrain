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

  [Tooltip("이전에 적용된 알고리즘과 합쳐서 터레인에 적용할지 여부를 확인 합니다.")]
  public bool addPrevTerrainHeight = true;

  // Perlin Noise
  public float perlinXScale = 0.01f;
  public float perlinYScale = 0.01f;
  public int perlinOffsetX = 0;
  public int perlinOffsetY = 0;
  public int perlinOctaves = 3;
  public float perlinPersistance = 8f;
  public float perlinHeightScale = 0.09f;

  // Multiple Perlin Noise  
  public List<PerlinParameters> perlinParameters = new List<PerlinParameters>()
  {
    new PerlinParameters()
  };

  // Voronoi Parameters
  public int peakCount;
  public float fallOff;
  public float dropOff;
  public float minHeight;
  public float maxHeight;
  public EVoronoiType voronoiType = EVoronoiType.Linear;

  // Midpoint Displacement
  public float minHeightForMPD = -2f;
  public float maxHeightForMPD = 2f;
  public float heightDampenerForMPD = 2.0f;
  public float roughnessForMPD = 2f;

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

  private float[,] GetHeightMap()
  {
    if(addPrevTerrainHeight)    
      return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    else
      return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];    
  }

  public void RandomTerrain()
  {
    float[,] heightMap = GetHeightMap();
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
    float[,] heightMap = GetHeightMap();
   
    for (int x = 0; x < terrainData.heightmapResolution; x++)
    {
      for (int z = 0; z < terrainData.heightmapResolution; z++)
      {
        heightMap[x, z] += heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
      }
    }

    terrainData.SetHeights(0,0, heightMap);
  }

  public void Perlin()
  {
    float[,] heightMap = GetHeightMap();
    for (int y = 0; y < terrainData.heightmapResolution; y++)
    {
      for (int x = 0; x < terrainData.heightmapResolution; x++)
      {
        heightMap[x, y] += Mathf.PerlinNoise((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale);
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void FBM()
  {
    float[,] heightMap = GetHeightMap();
    for (int y = 0; y < terrainData.heightmapResolution; y++)
    {
      for (int x = 0; x < terrainData.heightmapResolution; x++)
      {
        heightMap[x, y] += Utils.fBM((x + perlinOffsetX) * perlinXScale,
                                    (y + perlinOffsetY) * perlinYScale,
                                    perlinOctaves,
                                    perlinPersistance) * perlinHeightScale;
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void MultiplePerlin()
  {
    float[,] heightMap = GetHeightMap();
    for (int y = 0; y < terrainData.heightmapResolution; y++)
    {
      for (int x = 0; x < terrainData.heightmapResolution; x++)
      {
        foreach (PerlinParameters p in perlinParameters)
        {
          heightMap[x, y] += Utils.fBM((x + p.perlinOffsetX) * p.perlinXScale,
                                       (y + p.perlinOffsetY) * p.perlinYScale,
                                       p.perlinOctaves, p.perlinPersistance) * p.perlinHeightScale;
        }   
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }
  public void AddNewPerlin() // for multiple perlin function
  {
    perlinParameters.Add(new PerlinParameters());
  }
  public void RemovePerlin() // for multiple perlin function
  {
    List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
    for (int i = 0; i < perlinParameters.Count; i++)
    {
      if(!perlinParameters[i].remove)      
        keptPerlinParameters.Add(perlinParameters[i]);      
    }

    if(keptPerlinParameters.Count == 0)    
      keptPerlinParameters.Add(perlinParameters[0]);    

    perlinParameters = keptPerlinParameters;
  }

  public void VoronoiTessellation()
  {
    float[,] heightMap = GetHeightMap();

    for (int i = 0; i < peakCount; i++)
    {
      Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                               UnityEngine.Random.Range(minHeight, maxHeight),
                               UnityEngine.Random.Range(0, terrainData.heightmapResolution));

      if(heightMap[(int)peak.x, (int)peak.z] < peak.y)
        heightMap[(int)peak.x, (int)peak.z] = peak.y;
      else
        continue;

      Vector2 peakLocation = new Vector2(peak.x, peak.z);
      float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapResolution, terrainData.heightmapResolution));
      for (int z = 0; z < terrainData.heightmapResolution; z++)
      {
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
          if (!(x == peak.x && z == peak.z))
          {
            float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, z)) / maxDistance;
            float h;
            
            if(voronoiType == EVoronoiType.Combined)
            {
              h = peak.y - distanceToPeak * fallOff - Mathf.Pow(distanceToPeak, dropOff);
            }
            else if(voronoiType == EVoronoiType.Power)
            {
              h = peak.y - Mathf.Pow(distanceToPeak, dropOff) * fallOff;
            }
            else if (voronoiType == EVoronoiType.SinPow)
            {
              h = peak.y - Mathf.Pow(distanceToPeak * 3, fallOff) - Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / dropOff;
            }
            else
            {
              h = peak.y - distanceToPeak * fallOff;
            }
           
            if(heightMap[x, z] < h)            
              heightMap[x, z] = h;
          }
        }
      }
    }

    terrainData.SetHeights(0, 0, heightMap);
  }

  public void MidPointDisplacement() // Midpoint Displacement or Diamond Step
  {
    float[,] heightMap = GetHeightMap();
    int width = terrainData.heightmapResolution -1;
    int squreSize = width;
    float heightMin = minHeightForMPD;
    float heightMax = maxHeightForMPD;
    float heightDampener = Mathf.Pow(heightDampenerForMPD, -1 * roughnessForMPD);
    // float height = (float)squreSize / 2.0f * 0.01f;
    // float roughness = 2.0f;
    // float heightDampener = Mathf.Pow(2, -1 * roughness);

    int cornerX, cornerY;
    int midX, midY;
    int pmidXL, pmidXR, pmidYU, pmidYD;
    // heightMap[0, 0] = UnityEngine.Random.Range(0f, 0.2f);
    // heightMap[0, terrainData.heightmapResolution -2] = UnityEngine.Random.Range(0f, 0.2f);
    // heightMap[terrainData.heightmapResolution - 2, 0] = UnityEngine.Random.Range(0f, 0.2f);
    // heightMap[terrainData.heightmapResolution - 2, terrainData.heightmapResolution - 2] = UnityEngine.Random.Range(0f, 0.2f);

    while(squreSize > 0)
    {
      for (int x = 0; x < width; x += squreSize)
      {
        for (int y = 0; y < width; y += squreSize)
        {
          cornerX = (x + squreSize);
          cornerY = (y + squreSize);

          midX = (int)(x + squreSize / 2.0f);
          midY = (int)(y + squreSize / 2.0f);

          heightMap[midX, midY] = (float)((heightMap[x, y] + heightMap[cornerX, y] + 
                                           heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f 
                                           + UnityEngine.Random.Range(heightMin, heightMax));
        }
      }
      
      // Squre step
      for (int x = 0; x < width; x += squreSize)
      {
        for (int y = 0; y < width; y += squreSize)
        {
          cornerX = (x + squreSize);
          cornerY = (y + squreSize);

          midX = (int)(x + squreSize / 2.0f);
          midY = (int)(y + squreSize / 2.0f);

          pmidXR = (int)(midX + squreSize);
          pmidYU = (int)(midY + squreSize);
          pmidXL = (int)(midX - squreSize);
          pmidYD = (int)(midY - squreSize);

          if(pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width -1 || pmidYU >= width -1) continue;

          // Calculate the square value for the top side
          heightMap[midX, cornerY] = (heightMap[midX, pmidYU] + heightMap[midX, midY] +
                                      heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f
                                      + UnityEngine.Random.Range(heightMin, heightMax);

          // Calculate the square value for the bottom side
          heightMap[midX, y] = (heightMap[midX, midY] + heightMap[midX, pmidYD] + 
                                heightMap[x, y] + heightMap[cornerX, y]) / 4.0f
                                + UnityEngine.Random.Range(heightMin, heightMax);

          // Calculate the square value for the left side
          heightMap[x, midY] = (heightMap[x, cornerY] + heightMap[x, y] +
                                heightMap[pmidXL, midY] + heightMap[midX, midY]) / 4.0f
                                + UnityEngine.Random.Range(heightMin, heightMax);

          // Calculate the square value for the right side
          heightMap[cornerX, midY] = (heightMap[cornerX, cornerY] + heightMap[cornerX, y] +
                                      heightMap[midX, midY] + heightMap[pmidXR, midY]) / 4.0f
                                      + UnityEngine.Random.Range(heightMin, heightMax);          
        }
      }

      squreSize = (int)(squreSize / 2.0f);
      heightMin *= heightDampener;
      heightMax *= heightDampener;
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

[System.Serializable]
public class PerlinParameters
{
  public float perlinXScale = 0.01f;
  public float perlinYScale = 0.01f;
  public int perlinOctaves = 3;
  public float perlinPersistance = 8f;
  public float perlinHeightScale = 0.09f;
  public int perlinOffsetX = 0;
  public int perlinOffsetY = 0;
  public bool remove = false;
}

public enum EVoronoiType { Linear = 0, Power = 1, Combined = 2, SinPow = 3 }