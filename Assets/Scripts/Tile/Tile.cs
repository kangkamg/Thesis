﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour 
{
  GameObject _Prefab;

  public Vector3 gridPosition = Vector3.zero;

  public GameObject visual;

  public TileTypes type = TileTypes.Normal;
  public Renderer rend = new Renderer();

  public List<Tile> neighborsPlus = new List<Tile>();
  public List<Tile> neighborsCross = new List<Tile>();

  public int movementCost = 1;
  public bool impassible = false;
  public bool canMove = false; 

  private void Start()
  {
    rend = visual.GetComponent<Renderer> ();
  }

  public void GenerateNeighbors()
  {
    //forward
    if (gridPosition.z < GameManager.GetInstance()._mapSize[1]-1)
    {
      Vector3 n = new Vector3 (gridPosition.x, 0, gridPosition.z + 1);
      neighborsPlus.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //backward
    if (gridPosition.z > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x, 0, gridPosition.z - 1);
      neighborsPlus.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //right
    if (gridPosition.x < GameManager.GetInstance()._mapSize[0]-1)
    {
      Vector3 n = new Vector3 (gridPosition.x+1, 0, gridPosition.z);
      neighborsPlus.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //left
    if (gridPosition.x > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x-1, 0, gridPosition.z);
      neighborsPlus.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //rightforward
    if (gridPosition.x < GameManager.GetInstance()._mapSize[0]-1 && gridPosition.z < GameManager.GetInstance()._mapSize[1]-1)
    {
      Vector3 n = new Vector3 (gridPosition.x + 1, 0, gridPosition.z + 1);
      neighborsCross.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //leftforward
    if (gridPosition.x > 0 && gridPosition.z < GameManager.GetInstance()._mapSize[1]-1)
    {
      Vector3 n = new Vector3 (gridPosition.x - 1, 0, gridPosition.z + 1);
      neighborsCross.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //rightbackward
    if (gridPosition.x < GameManager.GetInstance()._mapSize[0]-1 && gridPosition.z > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x + 1, 0, gridPosition.z - 1);
      neighborsCross.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }
    //leftbackward
    if (gridPosition.x > 0 && gridPosition.z > 0)
    {
      Vector3 n = new Vector3 (gridPosition.x - 1, 0, gridPosition.z - 1);
      neighborsCross.Add (GameManager.GetInstance().map [(int)Mathf.Round(n.x)] [(int)Mathf.Round(n.z)]);
    }

  }

  public void SetImpassible()
  {
    impassible = true;
  }
  
  public void SetType(TileTypes t)
  {
    type = t;

    switch (t) 
    {
      case TileTypes.Normal:
      movementCost = 1;
      impassible = false;
      _Prefab = Resources.Load<GameObject> ("TilePrefab/Normal");
      break;
      
      case TileTypes.Grass:
      movementCost = 1;
      impassible = false;
      _Prefab = Resources.Load<GameObject> ("TilePrefab/Grass");
      break;

      case TileTypes.Water:
      movementCost = int.MaxValue;
      impassible = true;
      _Prefab = Resources.Load<GameObject> ("TilePrefab/Water");
      break;
      
      case TileTypes.Ground:
      movementCost = 1;
      impassible = false;
      _Prefab = Resources.Load<GameObject> ("TilePrefab/Ground");
      break;

      case TileTypes.Rock:
      movementCost = 1;
      impassible = false;
      _Prefab = Resources.Load<GameObject> ("TilePrefab/Rock");
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
}
