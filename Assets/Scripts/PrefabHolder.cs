using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder 
{
  public GameObject Base_TilePrefab 
  { 
    get 
    { 
      GameObject tile = (GameObject)Resources.Load ("TilePrefab/BaseTile"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Normal_TilePrefab 
  { 
    get 
    { 
      GameObject tile = (GameObject)Resources.Load ("TilePrefab/Normal"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Impassible_TilePrefab 
  { 
    get 
    { 
      GameObject tile = (GameObject)Resources.Load ("TilePrefab/Impassible"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject StartPos_TilePrefab 
  { 
    get 
    { 
      GameObject tile = (GameObject)Resources.Load ("TilePrefab/StartPos"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Player 
  { 
    get 
    { 
      GameObject player = (GameObject)Resources.Load ("PlayerPrefab/Player"); 
      return player;
    } 
    private set {} 
  }
    
  private static PrefabHolder instance;

  public static PrefabHolder GetInstance()
  {
      if (instance == null) instance = new PrefabHolder();
    return instance;
  }

}
