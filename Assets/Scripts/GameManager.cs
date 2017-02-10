﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

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

  public GameObject playerUI;
  List<GameObject> targetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (1);
    GenerateCharacter ();

    selectedCharacter = character [0];
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().MovementTile, selectedCharacter.characterStatus.movementPoint);
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().AttackTile, selectedCharacter.setupAbility[0].range,selectedCharacter.setupAbility[0].rangeType);
    selectedCharacter.attacking = true;
    HighlightTargetInRange ();
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
          HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().MovementTile, selectedCharacter.characterStatus.movementPoint);
          HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().AttackTile, selectedCharacter.setupAbility[0].range,selectedCharacter.setupAbility[0].rangeType);
          selectedCharacter.attacking = true;
          HighlightTargetInRange ();
        }
        if (hit.transform.name.Contains ("Tile") ) 
        {
          MoveCurrentCharacter (hit.transform.GetComponent<Tile> ());
          AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
        }
        if (hit.transform.tag == "Enemy")
        {
          foreach (GameObject h in targetInRange)
          {
            if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) 
            {
              
            }
          }
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

  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance)
  {
    HighlightTileAt(originLocation, highlight, distance, "plus", true);
  } 

  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance, string type = "")
  {
    HighlightTileAt(originLocation, highlight, distance, type, false);
  }

  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance, string type = "", bool ignoreCharacter = false)
  {
    List<Tile> highlightedTiles = new List<Tile> ();
    if (ignoreCharacter) 
    {
      highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, character.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
      foreach (Tile t in highlightedTiles) 
      {
        GameObject h = Instantiate (highlight, t.transform.position + (0.51f * Vector3.up), Quaternion.Euler(new Vector3(90,0,0)))as GameObject;
        h.transform.SetParent (t.transform);
        h.name = "highlightMovement";
        highlightTileMovement.Add (h);
      }
    } 
    else 
    {
      if (type == "both") 
      {
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, true)) 
        {
          highlightedTiles.Add (t);
        }
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, false)) 
        {
          highlightedTiles.Add (t);
        }
      } 
      else if (type == "cross") 
      {
        highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, true);
      } 
      else 
      {
        highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, false);
      }

      foreach (Tile t in highlightedTiles) 
      {
        GameObject h = Instantiate (highlight, t.transform.position + (0.51f * Vector3.up), Quaternion.Euler(new Vector3(90,0,0)))as GameObject;
        h.transform.SetParent (t.transform);
        h.name = "highlightAttack";
        highlightTileAttack.Add (h);
      }
    }
  }

  public void RemoveMapHighlight()
  {
    foreach (GameObject a in highlightTileMovement) 
    {
      Destroy (a);
    }
    foreach (GameObject b in highlightTileAttack) 
    {
      Destroy (b);
    }
    highlightTileMovement.Clear ();
    highlightTileAttack.Clear ();
  }

  public void MoveCurrentCharacter(Tile desTile)
  {
    foreach(GameObject h in highlightTileMovement)
    {
      if (desTile.transform.position.x == h.transform.position.x && desTile.transform.position.z == h.transform.position.z && selectedCharacter.positionQueue.Count == 0)
      {
        RemoveMapHighlight ();
        foreach (Tile t in TilePathFinder.FindPathPlus(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile, character.Where(x=>x.gridPosition != selectedCharacter.gridPosition).Select(x=>x.gridPosition).ToArray())) 
        {
          selectedCharacter.positionQueue.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z].transform.position + selectedCharacter.transform.position.y * Vector3.up);
        }
        selectedCharacter.gridPosition = desTile.gridPosition;
        break;
      } 
    }
  }
  public void AttackWithCurrentCharacter(Tile desTile)
  {
    foreach (GameObject h in highlightTileAttack) 
    {
      if (desTile.transform.position.x == h.transform.position.x && desTile.transform.position.z == h.transform.position.z && !desTile.impassible) 
      {
        Character target = null;
        foreach (Character p in character) 
        {
          if (p.gridPosition == desTile.gridPosition)
          {
            target = p;
          }
        }
        if (target != null)
        {
          RemoveMapHighlight ();
          selectedCharacter.attacking = false;

          int amountOfDamage = Mathf.Max (0, (int)Mathf.Floor (selectedCharacter.attackOverAll)) - target.characterStatus.defense;
          target.currentHP -= amountOfDamage;
        }
        break;
      }
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
    if (character [currentCharacterIndex].GetComponent<PlayerCharacter> () != null)
    {
      ShowPlayerUI (true);
    }
    else
    {
      ShowPlayerUI (false);
    }
  }

  private void ShowPlayerUI(bool showing)
  {
    playerUI.SetActive (showing);
  }

  public void HighlightTargetInRange()
  {
    foreach (GameObject obj in targetInRange) 
    {
      Destroy (obj);
    }
    targetInRange.Clear ();

    List<Tile> highlighted = new List<Tile> ();

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z],selectedCharacter.characterStatus.movementPoint))
    {
      if (selectedCharacter.setupAbility [0].rangeType == "both")
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, selectedCharacter.setupAbility [0].range, true))
        {
          highlighted.Add (a);
        }
        foreach (Tile b in TileHighLight.FindHighLight (t, selectedCharacter.setupAbility [0].range, true, true)) 
        {
          highlighted.Add (b);
        }
      } 
      else if (selectedCharacter.setupAbility [0].rangeType == "plus")
      {
        highlighted = TileHighLight.FindHighLight (t, selectedCharacter.setupAbility [0].range, true);
      }
      else
      {
        highlighted = TileHighLight.FindHighLight (t, selectedCharacter.setupAbility [0].range, true, true);
      }
    }

    foreach (Tile t in highlighted) 
    {
      GameObject inRange;

      Character[] cha = character.Where (x => x.gridPosition == t.gridPosition && x.gridPosition != selectedCharacter.gridPosition).Select(x=>x).ToArray();

      foreach (Character c in cha) 
      {
        inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
        inRange.GetComponent<Renderer> ().material.color = Color.red;
        inRange.transform.SetParent (c.transform);

        targetInRange.Add (inRange);
      }
    }
  }
}
