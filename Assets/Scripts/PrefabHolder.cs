using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrefabHolder 
{
  public GameObject Base_TilePrefab 
  { 
    get 
    { 
      GameObject tile = Resources.Load<GameObject> ("TilePrefab/BaseTile"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Normal_TilePrefab 
  { 
    get 
    { 
      GameObject tile = Resources.Load<GameObject> ("TilePrefab/Normal"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Impassible_TilePrefab 
  { 
    get 
    { 
      GameObject tile = Resources.Load<GameObject> ("TilePrefab/Impassible"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject StartPlayer_TilePrefab 
  { 
    get 
    { 
      GameObject tile = Resources.Load<GameObject> ("TilePrefab/StartPlayer"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Selected_TilePrefab 
  { 
    get 
    { 
      GameObject tile = Resources.Load<GameObject> ("TilePrefab/SelectedCharacter"); 
      return tile;
    } 
    private set {} 
  }

  public GameObject Player 
  { 
    get 
    { 
      GameObject player = Resources.Load<GameObject> ("PlayerPrefab/PlayerA"); 
      return player;
    } 
    private set {} 
  }

  public GameObject AIPlayer 
  { 
    get 
    { 
      GameObject aiPlayer = Resources.Load<GameObject>("PlayerPrefab/AIPlayer"); 
      return aiPlayer;
    } 
    private set {} 
  }
    
  public GameObject MovementTile 
  { 
    get 
    { 
      GameObject movement = Resources.Load<GameObject> ("TilePrefab/Highlight/Movement"); 
      return movement;
    } 
    private set {} 
  }

  public GameObject AttackTile 
  { 
    get 
    { 
      GameObject attack = Resources.Load<GameObject> ("TilePrefab/Highlight/Attack"); 
      return attack;
    } 
    private set {} 
  }

  public GameObject HealingTile 
  { 
    get 
    { 
      GameObject attack = Resources.Load<GameObject> ("TilePrefab/Highlight/Healing"); 
      return attack;
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
