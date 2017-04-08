using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
  private static GameManager instance;
  public static GameManager GetInstance() { return instance;}

  public int _mapSize = 11;
  public int currentCharacterIndex;
  public Transform mapTransform;

  public List<List<Tile>> map = new List<List<Tile>>();
  public List<Character> character = new List<Character> ();
  public List<int> enemyID = new List<int>();

  public Character selectedCharacter;
  public Vector3 oldGridPosition;
  public Vector3 oldPosition;
  public GameObject chaSelector;

  public GameObject playerUI;
  public GameObject results;
  public ShowingResultOfAttack showingResultOfAttack;
  public Transform playerController;

  List<GameObject> targetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  public int oldCharacterNo = -1;
  public bool isPlayerTurn = true;
  public bool isAutoPlay = false;
  public bool isTouch = true;
  public int abilityPage;

  public bool hitButton = false;

  public AbilityStatus usingAbility;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (PlayerPrefs.GetInt(Const.MapNo,1));
    GenerateCharacter ();
    
    playerController.gameObject.SetActive (false);
    results.SetActive (false);
    showingResultOfAttack.gameObject.SetActive (false);
  }

  private void Start()
  {
    SelectedCharacter (character [0]);
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0) && !hitButton && isPlayerTurn && isTouch /*&& Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(0).phase != TouchPhase.Moved*/)
    {
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

      RaycastHit hit;
      if (Physics.Raycast (ray, out hit, 1000f)) 
      {
        if (hit.transform.tag == "Player") 
        {
          if (oldCharacterNo < 0) 
          {
            SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
          }
          else
          {
            if (oldCharacterNo != hit.transform.GetComponent<PlayerCharacter>().ordering) 
            {
              selectedCharacter.gridPosition = oldGridPosition;
              selectedCharacter.transform.position = oldPosition;
              SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
                /*if (targetInRange.Count > 0) 
                {
                  foreach (GameObject h in targetInRange)
                  {
                    if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) 
                    {
                      if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0)
                      {
                        Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                        MoveCurrentCharacter (desTile);

                        selectedCharacter.target = hit.transform.GetComponent<Character> ();

                        break;
                      } 
                      else 
                      {
                        AttackWithCurrentCharacter (map [(int)hit.transform.GetComponent<Character> ().gridPosition.x] [(int)hit.transform.GetComponent<Character> ().gridPosition.z]);
                        break;
                      }
                    }
                  }
                }*/
            }
          }
        }

        if (!selectedCharacter.played) 
        {
          if (hit.transform.name.Contains ("Tile"))
          {
            foreach (Character c in character) 
            {
              if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Enemy")
              {
                AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
                break;
              }
              else if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Player")
              {
                if (usingAbility.ability.abilityType == -1 || usingAbility.ability.abilityType == -2 || usingAbility.ability.abilityType == -3) 
                {  
                  AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
                  break;
                }
              }
              else if (c.gridPosition != hit.transform.GetComponent<Tile> ().gridPosition)
              {
                foreach (GameObject m in highlightTileMovement)
                {
                  if (m.transform.position.x == hit.transform.position.x && m.transform.position.z == hit.transform.position.z)
                  {
                    MoveCurrentCharacter (hit.transform.GetComponent<Tile> ());
                    break;
                  }
                }
                break;
              }
            }
          }
          if (hit.transform.tag == "Enemy") 
          {
            if (targetInRange.Count > 0) 
            {
              foreach (GameObject h in targetInRange)
              {
                if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) 
                {
                  if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0)
                  {
                   
                    Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                    MoveCurrentCharacter (desTile);

                    selectedCharacter.target = hit.transform.GetComponent<Character> ();

                    break;
                  } 
                  else 
                  {
                    AttackWithCurrentCharacter (map [(int)hit.transform.GetComponent<Character> ().gridPosition.x] [(int)hit.transform.GetComponent<Character> ().gridPosition.z]);
                    break;
                  }
                }
              }
            }
            else 
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
    for (int i = 0; i < mapTransform.childCount; i++)
    {
      Destroy (mapTransform.GetChild (i).gameObject);
    }

    MapDatabaseContainner container = MapSaveAndLoad.Load (mapNumber);

    _mapSize = container.size;

    map = new List<List<Tile>> ();
    for (int x = 0; x < _mapSize; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < _mapSize; z++)
      {
        GameObject tileObj = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 ((PrefabHolder.GetInstance ().Base_TilePrefab.transform.localScale.x * x) - Mathf.Floor (_mapSize / 2), 0,
          (PrefabHolder.GetInstance ().Base_TilePrefab.transform.localScale.z * z) - Mathf.Floor (_mapSize / 2)),Quaternion.Euler (new Vector3 (0, 0, 0)));
        Tile tile = tileObj.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType ((TileTypes)container.tiles.Where(a=>a.locX == x && a.locZ ==z).First().type);
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
      
    for (int x = 0; x < map.Count; x++)
    {
      for (int z = 0; z < map.Count; z++)
      {
        map [x] [z].GenerateNeighbors ();
      }
    }
      
    mapTransform.gameObject.SetActive (true);
    CameraManager.GetInstance ().SetUpStartCamera (new Vector3(_mapSize,0,0));
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

    for(int i = 0; i < startPlayer.Count;i++) 
    {
      if (i > TemporaryData.GetInstance ().playerData.characters.Where (x => x.partyOrdering == i).Count ()) 
      {
        break; 
      }

      GameObject playerObj = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3 (startPlayer [i].transform.position.x, 1.5f, startPlayer [i].transform.position.z),Quaternion.identity);
      playerObj.transform.GetChild(0).rotation = Quaternion.Euler (0, 90, 0);
      PlayerCharacter player = playerObj.GetComponent<PlayerCharacter> ();
      player.SetStatus(TemporaryData.GetInstance ().playerData.characters.Where (x => x.partyOrdering == i).First());
      player.gridPosition = startPlayer[i].gridPosition;
      character.Add (player);
      player.ordering = character.Count - 1;
    }

    foreach (Tile a in startEnemy) 
    {
      GameObject aiPlayerObj = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3 (a.transform.position.x, 1.5f, a.transform.position.z),Quaternion.identity);
      aiPlayerObj.transform.GetChild(0).rotation = Quaternion.Euler (0, 90, 0);
      AICharacter aiPlayer = aiPlayerObj.GetComponent<AICharacter> ();
      aiPlayer.SetStatus (2001);
      for(int i= 0; i < aiPlayer.characterStatus.basicStatus.learnAbleAbility.Count;i++)
      {
        AbilityStatus equiped = new AbilityStatus ();
        string learnAbleAbility = aiPlayer.characterStatus.basicStatus.learnAbleAbility [i];
        string[] learnAbleAb = learnAbleAbility.Split (" " [0]);
        for(int j = 0; j < learnAbleAb.Length; j=j+2)
        {
          if(int.Parse(learnAbleAb[j+1]) == aiPlayer.characterStatus.characterLevel)
          {
            equiped.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
            equiped.level = 1;
            equiped.exp = 0;
          }
        }
        aiPlayer.characterStatus.equipedAbility.Add (equiped);
      }
      aiPlayer.gridPosition = a.gridPosition;
      character.Add (aiPlayer);
      aiPlayer.ordering = character.Count - 1;
    }
  }

  public void SetUseAble()
  {
    if (abilityPage == 1) 
    {
      SetUseAble (2);
    }
    else
    {
      SetUseAble (1);
    } 
  }
  private void SetUseAble(int page)
  {
    foreach (Transform child in playerUI.transform.GetChild (0).GetChild (1).GetChild(0))
    {
      Destroy (child.gameObject);
    }
    abilityPage = page;
    
    List<UsingAbilityManager> abilityObjs = new List<UsingAbilityManager> ();
    
    if (abilityPage == 1) 
    {
      List<AbilityStatus> normalAttack = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 1 || x.ability.abilityType == -1).ToList (); 
      AbilityStatus specialAttack = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 3 || x.ability.abilityType == -3).First ();
      
      for (int i = 0; i < 2; i++) 
      {
        GameObject abilityObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
        abilityObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(0));
        abilityObj.transform.localScale = Vector3.one;
        abilityObj.transform.localPosition = new Vector3 (-120+(85*i), 0, 0); 
        if (normalAttack.Where (x => x.ordering == i).Count () > 0)
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + normalAttack.Where (x => x.ordering == i).First ().ability.ID); 
          abilityObj.GetComponent<UsingAbilityManager> ().data = normalAttack.Where (x => x.ordering == i).First ();
          abilityObjs.Add (abilityObj.GetComponent<UsingAbilityManager> ());
        }
        else
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/0000"); 
          abilityObj.GetComponent<Toggle> ().interactable = false;
        }
        abilityObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
      }
      GameObject specialAbObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
      specialAbObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(0));
      specialAbObj.transform.localScale = Vector3.one;
      specialAbObj.transform.localPosition = new Vector3 (50, 0, 0); 
      specialAbObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + specialAttack.ability.ID); 
      specialAbObj.GetComponent<UsingAbilityManager> ().data = specialAttack;
      abilityObjs.Add (specialAbObj.GetComponent<UsingAbilityManager> ());
      specialAbObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
    }
    else
    {
      List<AbilityStatus> skill = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 2 || x.ability.abilityType == -2).ToList (); 

      for (int i = 0; i < 3; i++) 
      {
        GameObject abilityObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
        abilityObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(0));
        abilityObj.transform.localScale = Vector3.one;
        abilityObj.transform.localPosition = new Vector3 (-120+(85*i), 0, 0); 
        if (skill.Where (x => x.ordering == i).Count () > 0)
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + skill.Where (x => x.ordering == i).First ().ability.ID); 
          abilityObj.GetComponent<UsingAbilityManager> ().data = skill.Where (x => x.ordering == i).First ();
          abilityObjs.Add (abilityObj.GetComponent<UsingAbilityManager> ());
        }
        else
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/0000"); 
          abilityObj.GetComponent<Toggle> ().interactable = false;
        }
        abilityObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
      }
    }
    
    if (usingAbility != null && abilityObjs.Where (x => x.data.ability.ID == usingAbility.ability.ID).Count() > 0) 
      abilityObjs.Where (x => x.data.ability.ID == usingAbility.ability.ID).First ().GetComponent<Toggle> ().isOn = true;
  }
    
  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance)
  {
    HighlightTileAt(originLocation, highlight, distance, 0, true);
  } 

  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance, int type)
  {
    HighlightTileAt(originLocation, highlight, distance, type, false);
  }

  public void HighlightTileAt(Vector3 originLocation, GameObject highlight, int distance, int type, bool ignoreCharacter = false)
  {
    List<Tile> highlightedTiles = new List<Tile> ();
    if (ignoreCharacter) 
    {
      map [(int)originLocation.x] [(int)originLocation.z].canMove = true;
      highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, character.Where (x => x.gridPosition != originLocation && x.ordering != selectedCharacter.ordering).Select (x => x.gridPosition).ToArray ());

      foreach (Tile t in highlightedTiles) 
      {
        GameObject h = Instantiate (highlight, t.transform.position + (0.51f * Vector3.up), Quaternion.Euler(new Vector3(90,0,0)))as GameObject;
        t.canMove = true;
        h.transform.SetParent (t.transform);
        h.name = "highlightMovement";
        highlightTileMovement.Add (h);
      }
    } 
    else 
    {
      if (selectedCharacter.played) return;

      if (type == 2) 
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
      else if (type == 1) 
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

  public Tile CheckingMovementToAttackTarget(Transform target)
  {
    List<Tile> targetTile = new List<Tile> ();

    if(usingAbility.ability.rangeType == 1)
    {
      for(int i = 0;i<highlightTileMovement.Count;i++)
      {
        List<Tile> canAttacking = TileHighLight.FindHighLight (highlightTileMovement[i].transform.parent.GetComponent<Tile>(), usingAbility.ability.range, true, true);
        if (canAttacking.Where (x => x.transform.position.x == target.position.x && x.transform.position.z == target.position.z).Count () > 0)
        {
          targetTile.Add(highlightTileMovement [i].transform.parent.GetComponent<Tile>());
        }
      }
    }
    else if (usingAbility.ability.rangeType == 2) 
    {
      for (int i = 0; i < highlightTileMovement.Count; i++)
      {
        List<Tile> canAttacking = new List<Tile> ();
        foreach (Tile t in TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), usingAbility.ability.range, true, true)) 
        {
          canAttacking.Add (t);
        }
        foreach (Tile t in TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), usingAbility.ability.range, true, false)) 
        {
          canAttacking.Add (t);
        }
        if (canAttacking.Where (x => x.transform.position.x == target.position.x && x.transform.position.z == target.position.z).Count () > 0) 
        {
          targetTile.Add(highlightTileMovement [i].transform.parent.GetComponent<Tile>());
        }
      }
    }
    else
    {
      for (int i = 0; i < highlightTileMovement.Count; i++)
      {
        List<Tile> canAttacking = TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), usingAbility.ability.range, true, false);
        if (canAttacking.Where (x => x.transform.position.x == target.position.x && x.transform.position.z == target.position.z).Count () > 0) 
        {
          targetTile.Add(highlightTileMovement [i].transform.parent.GetComponent<Tile>());
        }
      }
    }

    Vector3 targetPosition = new Vector3(target.transform.position.x,0,target.transform.position.z);

    if (targetTile.Count > 0) 
    {
      targetTile.Sort (delegate(Tile a, Tile b)
        {
          return(Vector3.Distance (a.transform.position, targetPosition).CompareTo (Vector3.Distance (b.transform.position, targetPosition)));
      });

      return targetTile [0];
    }

    else return null;
  }

  public void RemoveMapHighlight()
  {
    foreach (List<Tile> t in map)
    {
      foreach (Tile a in t) 
      {
        a.canMove = false;
      }
    }

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

  public void RemoveAttackHighLightOnly()
  {
    foreach (GameObject a in highlightTileAttack) 
    {
      Destroy (a);
    }
    highlightTileAttack.Clear ();
  }
  
  public void SelectedAbility(AbilityStatus selectedAbility)
  {
    RemoveAttackHighLightOnly ();
    usingAbility = selectedAbility;
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().AttackTile, usingAbility.range, usingAbility.ability.rangeType);
    HighlightTargetInRange (usingAbility);
  }
  
  public void SelectedCharacter(Character playerSelected)
  {
    RemoveMapHighlight ();
    selectedCharacter = playerSelected;
    oldGridPosition = selectedCharacter.gridPosition;
    oldPosition = selectedCharacter.transform.position;
    currentCharacterIndex = selectedCharacter.ordering;
    oldCharacterNo = selectedCharacter.ordering;
    CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
    SelectedAbility (selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 1 || x.ability.abilityType == -1 && x.ordering == 0).First ());
    Destroy (chaSelector);
    chaSelector = null;
    chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
    chaSelector.transform.SetParent (selectedCharacter.transform);

    if (selectedCharacter.GetType () == typeof(PlayerCharacter))
    {
      playerUI.transform.GetChild (0).gameObject.SetActive (true);
      playerUI.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + selectedCharacter.name);
      playerUI.transform.GetChild (0).GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ().text = selectedCharacter.name.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.characterLevel.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = selectedCharacter.currentHP.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.attack.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (4).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.defense.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (5).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.criRate.ToString ();
    } 
    else 
    {
      playerUI.transform.GetChild (0).gameObject.SetActive (false);
      playerController.gameObject.SetActive (false);
    }

    SetUseAble (1);
  }

  public void MoveCurrentCharacter(Tile desTile)
  {
    List<Vector3> occupied = character.Where (x => x.gridPosition != desTile.gridPosition && x.gridPosition != selectedCharacter.gridPosition).Select (x => x.gridPosition).ToList ();

    foreach (List<Tile> t in map)
    {
      foreach (Tile a in t) 
      {
        if (!a.canMove) occupied.Add (a.gridPosition);
      }
    }

    foreach(GameObject h in highlightTileMovement)
    {
      if (desTile.gridPosition == h.GetComponentInParent<Tile>().gridPosition && selectedCharacter.positionQueue.Count == 0)
      {
        RemoveAttackHighLightOnly ();
        foreach (Tile t in TilePathFinder.FindPathPlus(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile, occupied.ToArray())) 
        {
          selectedCharacter.positionQueue.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z].transform.position + selectedCharacter.transform.position.y * Vector3.up);
        }
        selectedCharacter.gridPosition = desTile.gridPosition;
        break;
      } 
    }
  }
  
  public void CheckingSelectedTile(Vector3 gridPosition)
  {
    if (gridPosition.z >= 0 && gridPosition.x >= 0 && gridPosition.z < map [(int)gridPosition.x].Count && gridPosition.x < map.Count) 
    {
      if (character.Where (x => x.gridPosition == gridPosition).Count () > 0)
      {
        SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
      }
      else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0)
      {
        MoveCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
      }
      else if (highlightTileAttack.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0) 
      {
        AttackWithCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
      } 
      else if (targetInRange.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0) 
      {
        MoveCurrentCharacter (CheckingMovementToAttackTarget(map [(int)gridPosition.x] [(int)gridPosition.z].transform));

        selectedCharacter.target = targetInRange.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).First ().GetComponentInParent<Character> ();
      }
    }
    else
      Debug.Log ("Invalid Tile");
  }

  public void AttackWithCurrentCharacter(Tile desTile)
  {
    foreach (GameObject h in highlightTileAttack) 
    {
      if (desTile.gridPosition == h.GetComponentInParent<Tile>().gridPosition && !desTile.impassible) 
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
          if (usingAbility.ability.abilityType != -1)
          {
            int amountOfDamage = Mathf.FloorToInt (selectedCharacter.characterStatus.attack * usingAbility.power) - target.characterStatus.defense;
            if (amountOfDamage <= 0) amountOfDamage = 0;
            showingResultOfAttack.UpdateStatus (selectedCharacter, target, amountOfDamage);
            if (usingAbility.ability.gaugeUse > 0) FinishingGaugeManager.GetInstance ().ChangeSliderValue (-usingAbility.ability.gaugeUse);
            StartCoroutine (WaitDamageFloating (target, -amountOfDamage));
          }
          else
          {
            int healing = Mathf.Clamp (Mathf.FloorToInt (usingAbility.power), Mathf.FloorToInt (usingAbility.power), target.characterStatus.maxHp - target.currentHP);
            showingResultOfAttack.UpdateStatus (selectedCharacter, target, healing);
            StartCoroutine (WaitDamageFloating (target, healing));
          }
        }
        break;
      }
    }
  }

  private IEnumerator WaitDamageFloating(Character target, int amountOfResults)
  {
    int i = 0;
    CameraManager.GetInstance ().FocusCamera (selectedCharacter.transform.position, target.transform.position);
    
    while (i < usingAbility.hitAmount) 
    {
      selectedCharacter.transform.GetChild(0).GetComponent<Animator> ().Play ("Test");
      target.currentHP += amountOfResults;
      if(amountOfResults <= 0) FloatingTextController (amountOfResults*-1, target.transform);
      else FloatingTextController (amountOfResults, target.transform);
      if (target.GetType () == typeof(AICharacter)) FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
      else FinishingGaugeManager.GetInstance ().ChangeSliderValue (2.5f);
      i++;
      Animator anim = selectedCharacter.transform.GetChild(0).GetComponent<Animator> ();
      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo (0).length * anim.GetCurrentAnimatorStateInfo (0).speed);
    }
    CameraManager.GetInstance ().ResetCamera ();
    selectedCharacter.played = true;
    if (target.currentHP <= 0) 
    {
      RemoveDead ();
      if (target.GetType () == typeof(AICharacter)) FinishingGaugeManager.GetInstance ().ChangeSliderValue (10);
      else FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
    }
    NextTurn ();
  }

  public void NextTurn()
  {
    StartCoroutine(WaitEndTurn ());
  }

  private IEnumerator WaitEndTurn()
  {
    while (true) 
    {
      Animator anim = selectedCharacter.transform.GetChild(0).GetComponent<Animator> ();

      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo (0).length * anim.GetCurrentAnimatorStateInfo (0).speed);

      break;
    }
    GameManager.GetInstance ().playerUI.transform.GetChild (0).gameObject.SetActive (true);
    showingResultOfAttack.gameObject.SetActive (false);

    int playAbleAmount = 0;
    int playAbleCharacter = -1;
    if (isPlayerTurn) 
    {
      playAbleAmount = character.Where (x => x.GetType () == typeof(PlayerCharacter) && !x.played).Count ();
      if (character.Where (x => x.GetType () == typeof(PlayerCharacter) && !x.played).FirstOrDefault () != null)
        playAbleCharacter = character.Where (x => x.GetType () == typeof(PlayerCharacter) && !x.played).FirstOrDefault ().ordering;
    }
    else
    {
      playAbleAmount = character.Where (x => x.GetType () == typeof(AICharacter) && !x.played).Count ();
      if (character.Where (x => x.GetType () == typeof(AICharacter) && !x.played).FirstOrDefault () != null)
        playAbleCharacter = character.Where (x => x.GetType () == typeof(AICharacter) && !x.played).FirstOrDefault ().ordering;
    }
      
    if (playAbleAmount > 0) 
    {
      if (playAbleCharacter != -1) 
      {
        currentCharacterIndex = playAbleCharacter;
      }
      else 
      {
        currentCharacterIndex++;
      }
    }
    else
    {
      isPlayerTurn = !isPlayerTurn;
      if (currentCharacterIndex >= character.Count - 1) 
      {
        foreach (Character c in character) 
        {
          c.played = false;
        }
        currentCharacterIndex = 0;
        HitButton (false);
      } 
      else
      {
        currentCharacterIndex ++;
      }
    }
    ShowPlayerUI (isPlayerTurn);

    if (character [currentCharacterIndex] != null && character [currentCharacterIndex].currentHP > 0 && character[currentCharacterIndex].played == false)
    {
      SelectedCharacter (character [currentCharacterIndex]);
      if (character [currentCharacterIndex].GetType() == typeof(AICharacter))
      {
        character [currentCharacterIndex].TurnUpdate ();
      }
      else 
      {
        if(isAutoPlay) character [currentCharacterIndex].TurnUpdate ();
      }
    } 
    else
    {
      NextTurn ();
    }
  }

  public void HitButton(bool hit)
  {
    hitButton = hit; 
  }

  public void EndTurn()
  {
    selectedCharacter.played = true;
    oldCharacterNo = -1;

    NextTurn ();
  }

  private void ShowPlayerUI(bool showing)
  {
    playerUI.SetActive (showing);
  }

  public void HighlightTargetInRange(AbilityStatus usingAbility)
  {
    if (selectedCharacter.played) return;

    foreach (GameObject obj in targetInRange) 
    {
      Destroy (obj);
    }
    targetInRange.Clear ();

    List<Tile> highlighted = new List<Tile> ();

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z],selectedCharacter.characterStatus.movementPoint, character.Where (x => x.gridPosition != selectedCharacter.gridPosition && x.ordering != selectedCharacter.ordering).Select (x => x.gridPosition).ToArray ()))
    {
      if (usingAbility.ability.rangeType == 2)
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true))
        {
          highlighted.Add (a);
        }
        foreach (Tile b in TileHighLight.FindHighLight (t, usingAbility.range, true, true)) 
        {
          highlighted.Add (b);
        }
      } 
      else if (usingAbility.ability.rangeType == 0)
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true))
        {
          highlighted.Add (a);
        }
      }
      else
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true, true))
        {
          highlighted.Add (a);
        }
      }
    }

    foreach (GameObject a in highlightTileAttack) 
    {
      highlighted.Add(a.GetComponentInParent<Tile> ());
    }
      
    foreach (Tile t in highlighted) 
    {
      GameObject inRange;

      Character[] cha = character.Where (x => x.gridPosition == t.gridPosition && x.gridPosition != selectedCharacter.gridPosition).Select(x=>x).ToArray();

      foreach (Character c in cha) 
      {
        if (usingAbility.ability.abilityType != -1) 
        {
          if (c != null && c.tag != selectedCharacter.tag)
          {
            inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
            inRange.GetComponent<Renderer> ().material.color = Color.red;
            inRange.transform.SetParent (c.transform);

            targetInRange.Add (inRange);
          }
        }
        else
        {
          if (c != null && c.tag == selectedCharacter.tag)
          {
            inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
            inRange.GetComponent<Renderer> ().material.color = Color.green;
            inRange.transform.SetParent (c.transform);

            targetInRange.Add (inRange);
          }
        }
      }
    }
  }

  public void RemoveDead()
  {
    Result addResult = new Result ();

    for(int i = 0 ; i < character.Count ; i++) 
    {
      if (character[i].currentHP <= 0 && character[i].GetType() == typeof (AICharacter)) 
      {
        for (int j = 0; j < character [i].GetComponent<AICharacter> ().aiInfo.droppedItem.Count; j++) 
        {
          string[] droppedItem = character [i].GetComponent<AICharacter> ().aiInfo.droppedItem [j].Split (" " [0]);
          for (int k = 0; k < droppedItem.Length; k += 2)
          {
            if (UnityEngine.Random.Range (0, 101) <= int.Parse (droppedItem [k + 1]))
            {
              addResult.droppedItem.Add (GetDataFromSql.GetItemFromID (int.Parse (droppedItem [k])));
            }
          }
        }
        addResult.givenExp += character [i].GetComponent<AICharacter> ().aiInfo.givenExp;
        addResult.givenGold += character [i].GetComponent<AICharacter> ().aiInfo.givenGold;
        Destroy (character [i].gameObject);
        character.Remove (character[i]);
        break;
      }
      else if (character[i].currentHP <= 0 && character[i].GetType() == typeof (PlayerCharacter))
      {
        Destroy (character [i].gameObject);
        character.Remove (character[i]);
      }
    }
      
    for (int i = 0; i < addResult.droppedItem.Count; i++) 
    {
      TemporaryData.GetInstance ().result.droppedItem.Add (addResult.droppedItem [i]);
    }
    TemporaryData.GetInstance ().result.givenExp += addResult.givenExp;
    TemporaryData.GetInstance ().result.givenGold += addResult.givenGold;

    for(int i = 0 ; i < character.Count ; i++) 
    {
      character [i].ordering = i;
    }
    
    if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() <= 0) StartCoroutine (ShowResults ());
  }

  public IEnumerator ShowResults()
  {
    while (true)
    {
      yield return new WaitForSeconds (2f);

      break;
    }
    
    results.SetActive (true);

    results.transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenExp.ToString();
    results.transform.GetChild (0).GetChild (2).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenGold.ToString();

    List<CharacterStatus> party = TemporaryData.GetInstance ().playerData.characters.Where (x => x.isInParty).ToList ();

    for (int i = 0; i < party.Count; i++) 
    {
      GameObject characterInParty = Instantiate (Resources.Load<GameObject> ("ResultCharacter"));
      characterInParty.transform.SetParent (results.transform.GetChild (0).GetChild (3));
      characterInParty.GetComponent<Image>().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + party[i].basicStatus.characterName);
      characterInParty.transform.localScale = new Vector3 (1, 1, 1);
      party [i].experience += TemporaryData.GetInstance ().result.givenExp;
    }

    for (int i = 0; i < TemporaryData.GetInstance ().result.droppedItem.Count; i++) 
    {
      Item addedItem = new Item ();
      addedItem.item = TemporaryData.GetInstance ().result.droppedItem [i];
      addedItem.equiped = false;
      addedItem.ordering = TemporaryData.GetInstance ().playerData.inventory.Count;
      GameObject itemObj = Instantiate (Resources.Load<GameObject> ("Item/ItemGet"));
      itemObj.transform.SetParent (results.transform.GetChild (0).GetChild (4));
      itemObj.transform.localScale = new Vector3 (1, 1, 1);
      itemObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + addedItem.item.name);
      itemObj.transform.GetChild (1).GetComponent<Text> ().text = addedItem.item.name.ToString();
      TemporaryData.GetInstance ().playerData.inventory.Add (addedItem);
    }
    TemporaryData.GetInstance ().playerData.gold += TemporaryData.GetInstance ().result.givenGold;

    TemporaryData.GetInstance ().result = new Result ();

    while (true) 
    {
      if (Input.GetMouseButtonDown (0)/*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
      {
        SceneManager.LoadScene ("MainMenuScene");
        break;
      }
	  yield return 0;
    }
  }

  public void FloatingTextController(int value, Transform location)
  {
    FloatingText popUpText = Resources.Load<FloatingText> ("PopupTextParent");
    FloatingText instance = Instantiate (popUpText);
    Vector2 screenPosition = Camera.main.WorldToScreenPoint (location.position);

    instance.transform.SetParent (GameObject.Find ("Canvas").transform,false);
    instance.transform.position = screenPosition;
    instance.SetText (value);
  }
  
  public void OpenController()
  {
    isTouch = !isTouch;
    playerController.gameObject.SetActive (!isTouch);
    if (!isTouch)
      playerController.GetComponent<PlayerController> ().SetUpSelectedPosition ();
    else
      playerController.GetComponent<PlayerController> ().RemoveSelected ();
  }
  
  public void Auto()
  {
    isAutoPlay = !isAutoPlay;
    if(isAutoPlay) 
    {
      RemoveMapHighlight ();        
      character.Where(x=>x.GetType() == typeof (PlayerCharacter) && x.played == false).First().TurnUpdate ();
    }
  }
}
