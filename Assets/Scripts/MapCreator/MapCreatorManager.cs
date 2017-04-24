using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCreatorManager : MonoBehaviour
{
  private static MapCreatorManager instance;

  public int mapSize;
  public List<List<Tile>> map = new List<List<Tile>>();
  public List<EnemyInMapData> enemies = new List<EnemyInMapData> ();
  public Transform mapTransform;

  public TileTypes palletSelection = TileTypes.Normal;

  public string enemiesID = "2001";
  
  public Text mapID;

  public static MapCreatorManager GetInstance()
  {
    return instance;
  }

  void Awake()
  {
    instance = GetComponent<MapCreatorManager>();
    GetDataFromSql.OpenDB ("ThesisDatabase.db");
  }

  private void Update()
  {
    if (Input.GetMouseButton(0)) 
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast (ray, out hit, 1000f))
      {
        if (hit.transform.name.Contains ("Tile")) 
        {
          hit.transform.GetComponent<Tile> ().SetType (palletSelection);
          if (palletSelection == TileTypes.StartEnemy)
          {
            if (enemies.Where (x => x.locX == hit.transform.GetComponent<Tile> ().gridPosition.x && x.locZ == hit.transform.GetComponent<Tile> ().gridPosition.z).Count () > 0)
            {
              MapCreatorManager.GetInstance ().RemoveEnemy (hit.transform.GetComponent<Tile> ().gridPosition);
            }
            CreateEnemy (hit.transform, int.Parse (enemiesID));
          } 
          else
          {
            if (enemies.Where (x => x.locX == hit.transform.GetComponent<Tile> ().gridPosition.x && x.locZ == hit.transform.GetComponent<Tile> ().gridPosition.z).Count () > 0)
            {
              MapCreatorManager.GetInstance ().RemoveEnemy (hit.transform.GetComponent<Tile> ().gridPosition);
            }
          }
        }
      }
    }
  }
  private void GenerateBlankMap(int mapSize)
  {
    for (int i = 0; i < mapTransform.childCount; i++) 
    {
      Destroy (mapTransform.GetChild (i).gameObject);
    }
    
    enemies = new List<EnemyInMapData> ();
    map = new List<List<Tile>> ();
    
    for (int x = 0; x < mapSize; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < mapSize; z++)
      {
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 ((PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.x * x) - Mathf.Floor (mapSize / 2),
          0,(PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.z * z) - Mathf.Floor (mapSize / 2)), Quaternion.identity).gameObject.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType (TileTypes.Normal);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
    
    Camera.main.orthographicSize = mapSize * 2f;
    Camera.main.transform.position = new Vector3 ((mapSize / 2f)-0.5f, Camera.main.transform.position.y, mapSize / 2f);
  }

  private void SaveMap(int mapNumber)
  {
    MapSaveAndLoad.SaveData (MapSaveAndLoad.CreateMapContainer (map,enemies), mapNumber);
  }

  private void LoadMap(int mapNumber)
  {
    MapDatabaseContainner container = MapSaveAndLoad.Load (mapNumber);

    mapSize = container.size;
    enemies = container.enemies;

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
        Tile tile = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 ((PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.x * x) - Mathf.Floor (mapSize / 2),0,(PrefabHolder.GetInstance().Base_TilePrefab.transform.localScale.z * z) - Mathf.Floor (mapSize / 2)), Quaternion.identity).gameObject.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType ((TileTypes)container.tiles.Where(a=>a.locX == x && a.locZ ==z).First().type);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
        if(enemies.Where(i=>i.locX == x && i.locZ ==z).Count()>0)  
        {
          CreateEnemy (tile.transform, enemies.Where (i => i.locX == x && i.locZ == z).First ().enemyID);
        }
      }
      map.Add(row);
    }
    
    Camera.main.orthographicSize = mapSize * 2f;
    Camera.main.transform.position = new Vector3 ((mapSize / 2f)-0.5f, Camera.main.transform.position.y, mapSize / 2f);
  }
  
  public void ChangePallet(int t)
  {
    palletSelection = (TileTypes)t;
  }
  
  public void AddEnemy()
  {
    palletSelection = TileTypes.StartEnemy;
    enemiesID = EventSystem.current.currentSelectedGameObject.transform.GetChild (1).GetComponent<InputField> ().text;
  }
  
  public void RemoveEnemy(Vector3 gridPosition)
  {
    enemies.Remove(enemies.Where (x => x.locX == gridPosition.x && x.locZ == gridPosition.z).First ());
    Destroy (map [(int)gridPosition.x] [(int)gridPosition.z].transform.GetChild (1).gameObject);
  }
  
  public void CreateEnemy(Transform tilePos, int ID)
  {
    GameObject enemyObj = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/AIPlayer"));
    if (Resources.Load ("PlayerPrefab/" + ID) != null) 
    {
      GameObject renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/" + ID));
      renderer.transform.SetParent (enemyObj.transform);
      renderer.transform.SetAsFirstSibling ();
      renderer.transform.localScale = Vector3.one*2;
      renderer.transform.localPosition = Vector3.zero - Vector3.up;
    } 
    else 
    {
      GameObject renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/0000"));
      renderer.transform.SetParent (enemyObj.transform);
      renderer.transform.SetAsFirstSibling ();
      renderer.transform.localScale = Vector3.one*2;
      renderer.transform.localPosition = Vector3.zero - Vector3.up;
    }
    enemyObj.transform.SetParent (tilePos);
    enemyObj.transform.localScale = Vector3.one + Vector3.up;
    enemyObj.transform.position = tilePos.position + ((Vector3.up * enemyObj.transform.localScale.y) + (Vector3.up/2));
    
    EnemyInMapData newEnemy = new EnemyInMapData ();
    newEnemy.enemyID = ID;
    newEnemy.locX = (int)tilePos.GetComponent<Tile>().gridPosition.x;
    newEnemy.locZ = (int)tilePos.GetComponent<Tile>().gridPosition.z;
    enemies.Add (newEnemy);
  }
  
  public void SaveAndLoad(bool isSaving)
  {
    if (isSaving) 
    {
      SaveMap (int.Parse (mapID.text));
    } 
    else 
    {
      LoadMap (int.Parse (mapID.text));
    }
  }
  
  public void GenerateBlankMap()
  {
    GenerateBlankMap(int.Parse(EventSystem.current.currentSelectedGameObject.transform.GetChild (1).GetComponent<InputField> ().text));
  }
  
  
}
