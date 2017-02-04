using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour 
{
  private GameObject _Prefab;

  public Vector3 gridPosition = Vector3.zero;

  public GameObject visual;

  public TileTypes type = TileTypes.Normal;
  public Renderer rend;

  public List<Tile> neighbors = new List<Tile>();

  public int movementCost = 1;
  public bool impassible = false;

  private void Start()
  {
    rend = visual.GetComponent<Renderer> ();

    if (SceneManager.GetActiveScene().name == "TestScene") GenerateNeighbors ();
  }

  private void GenerateNeighbors()
  {
    neighbors = new List<Tile> ();
    //forward
    if (gridPosition.z < GameManager.GetInstance()._mapSize-1)
    {
      Vector3 n = new Vector3 (gridPosition.x, 0, gridPosition.z + 1);
      neighbors.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //backward
    if (gridPosition.z > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x, 0, gridPosition.z - 1);
      neighbors.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //right
    if (gridPosition.x < GameManager.GetInstance()._mapSize-1)
    {
      Vector3 n = new Vector3 (gridPosition.x+1, 0, gridPosition.z);
      neighbors.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //left
    if (gridPosition.x > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x-1, 0, gridPosition.z);
      neighbors.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }

  }

  public void SetType(TileTypes t)
  {
    type = t;

    switch (t) 
    {
      case TileTypes.Normal:
      movementCost = 1;
      impassible = false;
      _Prefab = PrefabHolder.GetInstance ().Normal_TilePrefab;
      break;

      case TileTypes.Impassible:
      movementCost = int.MaxValue;
      impassible = true;
      _Prefab = PrefabHolder.GetInstance ().Impassible_TilePrefab;
      break;

      case TileTypes.StartPos:
      movementCost = 1;
      impassible = false;
      if (SceneManager.GetActiveScene ().name == "MapCreator") _Prefab = PrefabHolder.GetInstance ().StartPos_TilePrefab;
      else _Prefab = PrefabHolder.GetInstance ().Normal_TilePrefab;
      break;
    }

    GenerateVisuals ();
  }

  private void GenerateVisuals()
  {
    GameObject container = transform.FindChild ("Visuals").gameObject;

    for (int i = 0; i < container.transform.childCount; i++) 
    {
      Destroy (container.transform.GetChild (i).gameObject);
    }

    GameObject newVisual = Instantiate (_Prefab, transform.position, Quaternion.identity) as GameObject;
    newVisual.transform.SetParent (container.transform);

    visual = newVisual;
  }

  void OnMouseEnter()
  {
    if (SceneManager.GetActiveScene().name.Equals("MapCreator") && Input.GetMouseButton(0)) 
    {
      SetType (MapCreatorManager.GetInstance ().palletSelection);
    }
  }

}
