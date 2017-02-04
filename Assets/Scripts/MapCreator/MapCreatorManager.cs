using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MapCreatorManager : MonoBehaviour
{
  private static MapCreatorManager instance;

  public int mapSize;
  public List<List<Tile>> map = new List<List<Tile>>();
  public Transform mapTransform;

  public TileTypes palletSelection = TileTypes.Normal;

  private string editMapSize = "EditMapSize";
  private string editMapNumber = "EditMapNumber";
  private string loadMapNumber = "LoadMapNumber";

  public static MapCreatorManager GetInstance()
  {
    return instance;
  }

  void Awake()
  {
    instance = GetComponent<MapCreatorManager> ();
  }

  private void GenerateBlankMap(int mSize)
  {
    mapSize = mSize;

    for (int i = 0; i < mapTransform.childCount; i++) 
    {
      Destroy (mapTransform.GetChild (i).gameObject);
    }

    map = new List<List<Tile>> ();
    for (int x = 0; x < mapSize; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < mapSize; z++)
      {
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 (x - Mathf.Floor (mapSize / 2) + x,0,z - Mathf.Floor (mapSize / 2) + z), Quaternion.identity).gameObject.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType (TileTypes.Normal);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
  }

  private void SaveMap(int mapNumber)
  {
    MapSaveAndLoad.SaveData (MapSaveAndLoad.CreateMapContainer (map), mapNumber);
  }

  private void LoadMap(int mapNumber)
  {
    MapDatabaseContainner container = MapSaveAndLoad.Load (mapNumber);

    mapSize = container.size;

    for (int i = 0; i < mapTransform.childCount; i++)
    {
      Destroy (mapTransform.GetChild (i).gameObject);
    }

    map = new List<List<Tile>> ();
    for (int x = 0; x < mapSize; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < mapSize; z++)
      {
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 (x - Mathf.Floor (mapSize / 2) + x,0,z - Mathf.Floor (mapSize / 2) + z), Quaternion.identity).gameObject.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType ((TileTypes)container.tiles.Where(a=>a.locX == x && a.locZ ==z).First().type);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
  }
  private void OnGUI()
  {
    Rect rect = new Rect (10, Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "Normal"))
    {
      palletSelection = TileTypes.Normal;
    }

    rect = new Rect(10 + (100+10) * 2, Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "StartPos")) 
    {
      palletSelection = TileTypes.StartPos;
    }

    rect = new Rect(10 + (100+10) * 3, Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "Impassible")) 
    {
      palletSelection = TileTypes.Impassible;
    }

    rect = new Rect(Screen.width - (10 + (100+10) * 3), Screen.height - 160, 100, 60);
    editMapSize = GUI.TextField (rect, editMapSize, 25);


    rect = new Rect(Screen.width - (10 + (100+10) * 3), Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "CreateNewMap")) 
    {
      GenerateBlankMap (int.Parse(editMapSize));
    }

    rect = new Rect(Screen.width - (10 + (100+10) * 2), Screen.height - 160, 100, 60);
    editMapNumber = GUI.TextField (rect, editMapNumber, 25);

    rect = new Rect(Screen.width - (10 + (100+10) * 2), Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "SaveMap")) 
    {
      SaveMap (int.Parse(editMapNumber));
    }

    rect = new Rect(Screen.width - (10 + (100+10) * 1), Screen.height - 160, 100, 60);
    loadMapNumber = GUI.TextField (rect, loadMapNumber, 25);

    rect = new Rect(Screen.width - (10 + (100+10) * 1), Screen.height - 80, 100, 60);
    if (GUI.Button (rect, "LoadMap")) 
    {
      LoadMap (int.Parse(loadMapNumber));
    }
  }
}
