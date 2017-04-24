using UnityEngine;
using System.Collections.Generic;

public class ObjectGradient : MonoBehaviour
{
  public Color topColor = Color.white;
  public Color bottomColor = Color.black;

  public void Start()
  {
    MeshFilter mesh = GetComponent<MeshFilter> ();
    Vector2[] uv = mesh.mesh.uv;
    Color[] colors = new Color[uv.Length];

    for (int i = 0; i < uv.Length; i++)
    {
      colors[i] = Color.Lerp(bottomColor, topColor, uv[i].x);
    }
    
    mesh.mesh.colors = colors;
  }
}