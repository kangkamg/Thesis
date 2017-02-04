using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour 
{
  private static GameManager instance;
  public static GameManager GetInstance() { return instance;}

  public int _mapSize = 11;
  public Transform mapTransform;

  public List<List<Tile>> map = new List<List<Tile>>();
  public List<Character> character = new List<Character> ();

  public Character selectedCharacter;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (4);
    GenerateCharacter ();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown (0))
    {
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray,out hit, 1000f))
      {
        if (hit.transform.tag == "Player") 
        {
          selectedCharacter = hit.transform.GetComponent<PlayerCharacter> ();
          HighlightTileAt (hit.transform.GetComponent<PlayerCharacter> ().gridPosition, Color.blue, hit.transform.GetComponent<PlayerCharacter> ().movementPoint);
          CameraManager.GetInstance ().SetOffset (selectedCharacter.transform.position);
        }

        if (hit.transform.name.Contains ("Tile") && hit.transform.GetComponent<Tile> ().rend.material.color == Color.blue)
        {
          MoveCurrentCharacter (hit.transform.GetComponent<Tile> ());
        }
      }
    }
  }

  public void GenerateMap(int mapNumber)
  {
    LoadMap(mapNumber);
  }

  private void LoadMap(int mapNumber)
  {
    MapDatabaseContainner container = MapSaveAndLoad.Load (mapNumber);

    _mapSize = container.size;

    for (int i = 0; i < mapTransform.childCount; i++)
    {
      Destroy (mapTransform.GetChild (i).gameObject);
    }

    map = new List<List<Tile>> ();
    for (int x = 0; x < _mapSize; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < _mapSize; z++)
      {
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 (x - Mathf.Floor (_mapSize / 2) + x,0,z - Mathf.Floor (_mapSize / 2) + z), Quaternion.identity).gameObject.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType ((TileTypes)container.tiles.Where(a=>a.locX == x && a.locZ ==z).First().type);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
  }

  public void GenerateCharacter()
  {
    List<Tile> startPos = new List<Tile> ();

    foreach (List<Tile> t in map)
    {
      foreach (Tile a in t) 
      {
        if (a.type == TileTypes.StartPos) 
        {
          startPos.Add (a);
        }
      }
    }

    PlayerCharacter player;

    player = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3(startPos [0].transform.position.x, 1.5f, startPos [0].transform.position.z), Quaternion.Euler (new Vector3 (0, 90, 0))).GetComponent<PlayerCharacter>();
    player.gridPosition = startPos [0].gridPosition;

    character.Add (player);

    player = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3(startPos [1].transform.position.x, 1.5f, startPos [1].transform.position.z), Quaternion.Euler (new Vector3 (0, 90, 0))).GetComponent<PlayerCharacter>();
    player.gridPosition = startPos [1].gridPosition;

    character.Add (player);
  }
  public void HighlightTileAt(Vector3 originLocation, Color highlightColor, int distance)
  {
    List<Tile> highlightedTiles = new List<Tile> ();
    highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance);

    foreach (Tile t in highlightedTiles) 
    {
      t.rend.material.color = highlightColor;
    }
  }

  public void RemoveMapHighlight()
  {
    for (int x = 0; x < _mapSize; x++) 
    {
      for (int z = 0; z < _mapSize; z++) 
      {
        if (!map[x][z].impassible) map [x] [z].rend.material.color = Color.white;
      }
    }
  }

  public void MoveCurrentCharacter(Tile desTile)
  {
    if (desTile.rend.material.color != Color.white) 
    {
      RemoveMapHighlight ();
      List<Tile> highlightedTiles = new List<Tile> ();
      foreach (Tile t in TilePathFinder.FindPath(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile))
      {
        selectedCharacter.positionQueue.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z].transform.position + 1.5f * Vector3.up);
        highlightedTiles.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z]);
      }
      selectedCharacter.gridPosition = desTile.gridPosition;
    }
  }
}
