using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour 
{
  private static GameManager instance;
  public static GameManager GetInstance() { return instance;}

  public int _mapSize = 11;
  public int currentCharacterIndex;
  public Transform mapTransform;

  public List<List<Tile>> map = new List<List<Tile>>();
  public List<Character> character = new List<Character> ();

  public Character selectedCharacter;
  public GameObject chaSelector;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (1);
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
          RemoveMapHighlight ();
          selectedCharacter = hit.transform.GetComponent<PlayerCharacter> ();
          HighlightTileAt (selectedCharacter.gridPosition, Color.blue,selectedCharacter.characterStatus.movementPoint, true);
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
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 ((PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.x * x) - Mathf.Floor (_mapSize / 2),0,
        (PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.z * z) - Mathf.Floor (_mapSize / 2)), Quaternion.identity).gameObject.GetComponent<Tile> ();
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
    List<Tile> startPlayer = new List<Tile> ();
    List<Tile> startEnemy = new List<Tile> ();

    foreach (List<Tile> t in map)
    {
      foreach (Tile a in t) 
      {
        if (a.type == TileTypes.StartPlayer) 
        {
          startPlayer.Add (a);
        }
        else if (a.type == TileTypes.StartEnemy) 
        {
          startEnemy.Add (a);
        }
      }
    }

    foreach (Tile a in startPlayer) 
    {
      PlayerCharacter player;

      player = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3(a.transform.position.x, 1.5f, a.transform.position.z), Quaternion.Euler (new Vector3 (0, 90, 0))).GetComponent<PlayerCharacter>();
      player.gridPosition = a.gridPosition;
      character.Add (player);
    }

    foreach (Tile a in startEnemy) 
    {
      AICharacter aiPlayer;

      aiPlayer = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3(a.transform.position.x, 2.175f, a.transform.position.z), Quaternion.Euler (new Vector3 (0, -90, 0))).GetComponent<AICharacter>();
      aiPlayer.gridPosition = a.gridPosition;
      character.Add (aiPlayer);
    }
  }

  public void HighlightTileAt(Vector3 originLocation, Color highlightColor, int distance, bool ignoreCharacter = false)
  {
    List<Tile> highlightedTiles = new List<Tile> ();
    if (ignoreCharacter)
      highlightedTiles = TileHighLight.FindHighLightPlus (map [(int)originLocation.x] [(int)originLocation.z], distance, character.Where(x=>x.gridPosition!=originLocation).Select(x=>x.gridPosition).ToArray());
    else
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
    if (desTile.rend.material.color != Color.white && character [currentCharacterIndex].positionQueue.Count == 0)
    {
      RemoveMapHighlight ();
      foreach (Tile t in TilePathFinder.FindPathPlus(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile, character.Where(x=>x.gridPosition!=character[currentCharacterIndex].gridPosition).Select(x=>x.gridPosition).ToArray())) 
      {
        selectedCharacter.positionQueue.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z].transform.position + 1.5f * Vector3.up);
      }
      selectedCharacter.gridPosition = desTile.gridPosition;
    } 
  }

  public void NextTurn()
  {
    if (currentCharacterIndex < character.Count - 1) 
    {
      currentCharacterIndex++;
    } 
    else 
    {
      currentCharacterIndex = 0;
    }
    selectedCharacter = character [currentCharacterIndex];
    Destroy (chaSelector);
    chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler(90,0,0));
    chaSelector.transform.SetParent (selectedCharacter.transform);
    character [currentCharacterIndex].TurnUpdate ();
  }
}
