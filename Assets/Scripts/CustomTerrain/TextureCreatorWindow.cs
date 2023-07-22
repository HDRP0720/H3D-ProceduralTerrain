using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureCreatorWindow : EditorWindow
{
  [MenuItem("Window/TextureCreatorWindow")]
  public static void ShowWindow()
  {
    EditorWindow.GetWindow(typeof(TextureCreatorWindow));
  }

  private string fileName = "myProceduralTexture";
  private float perlinXScale;
  private float perlinYScale;
  private int perlinOctaves;
  private float perlinPersistance;
  private float perlinHeightScale;
  private int perlinOffsetX;
  private int perlinOffsetY;
  private bool alphaToggle = false;
  private bool mapToggle = false;
  private bool seamlessToggle = false;

  private Texture2D pTexture;

  private void OnEnable() 
  {
    pTexture = new Texture2D(513, 513, TextureFormat.ARGB32, false);
  }
  private void OnGUI()
  {
    GUILayout.Label("Settings", EditorStyles.boldLabel);    
    fileName = EditorGUILayout.TextField("Texture Name", fileName);
    EditorGUILayout.Space(20);

    int wSize = (int)(EditorGUIUtility.currentViewWidth - 100);

    perlinXScale = EditorGUILayout.Slider("X Scale", perlinXScale, 0, 0.1f);
    perlinYScale = EditorGUILayout.Slider("Y Scale", perlinYScale, 0, 0.1f);
    perlinOctaves = EditorGUILayout.IntSlider("Octaves", perlinOctaves, 1, 10);
    perlinPersistance = EditorGUILayout.Slider("Persistance", perlinPersistance, 1, 10);
    perlinHeightScale = EditorGUILayout.Slider("Height Scale", perlinHeightScale, 0, 1);
    perlinOffsetX = EditorGUILayout.IntSlider("Offset X", perlinOffsetX, 0, 10000);
    perlinOffsetY = EditorGUILayout.IntSlider("Offset Y", perlinOffsetY, 0, 10000);
    alphaToggle = EditorGUILayout.Toggle("Alpha", alphaToggle);
    mapToggle = EditorGUILayout.Toggle("Map", mapToggle);
    seamlessToggle = EditorGUILayout.Toggle("Seamless", seamlessToggle);

    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Generate", GUILayout.Width(wSize)))
    {
      int w = 513;
      int h = 513;
      float pValue;
      Color pixCol = Color.white;
      for (int y = 0; y < h; y++)
      {
        for (int x = 0; x < w; x++)
        {
          pValue = Utils.fBM((x + perlinOffsetX) * perlinXScale, (y + perlinOffsetY) * perlinYScale, 
                              perlinOctaves, perlinPersistance) * perlinHeightScale;
          float colValue = pValue;
          pixCol = new Color(colValue, colValue, colValue, alphaToggle ? colValue : 1);
          pTexture.SetPixel(x, y, pixCol);
        }
      }
      pTexture.Apply(false, false);
    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.Label(pTexture, GUILayout.Width(wSize), GUILayout.Height(wSize));
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if (GUILayout.Button("Save", GUILayout.Width(wSize)))
    {

    }
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }
}
