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
  public List<EnemyInMapData> enemies = new List<EnemyInMapData>();
  public List<Character> character = new List<Character> ();
  public List<int> playerCharacterID = new List<int> ();

  public Character selectedCharacter;
  public Vector3 oldGridPosition;
  public Vector3 oldPosition;
  public GameObject chaSelector;

  public GameObject playerUI;
  public GameObject results;
  public GameObject menu;
  public GameObject pauseMenu;
  public ShowingResultOfAttack showingResultOfAttack;
  public Transform playerController;

  List<Character> targetInRange = new List<Character> ();
  List<GameObject> highlightedTileTargetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  public int oldCharacterNo = -1;
  public bool isPlayerTurn = true;
  public bool isAutoPlay = false;
  public bool isTouch = true;
  public bool isPause = false;
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
    pauseMenu.SetActive (false);
    showingResultOfAttack.gameObject.SetActive (false);
  }

  private void Start()
  {
    SelectedCharacter (character [0]);
  }

  private void Update()
  {
    foreach (Touch toches in Input.touches) 
    {
      Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);

      RaycastHit hit;
      if (Physics.Raycast (ray, out hit, 1000f)) 
      {
        if (/*Input.GetMouseButtonDown(0) && */!hitButton && isPlayerTurn && isTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !CameraManager.GetInstance().isMoving)
        {
          if (!selectedCharacter.played)
          {
            if (hit.transform.tag == "Player") 
            {
              if (oldCharacterNo < 0) {
                SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
              } 
              else 
              {
                if (oldCharacterNo != hit.transform.GetComponent<PlayerCharacter> ().ordering) 
                {
                  if (usingAbility.ability.abilityType < 0 && highlightedTileTargetInRange.Count>0) 
                  {
                    foreach (GameObject h in highlightedTileTargetInRange) 
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
                    selectedCharacter.gridPosition = oldGridPosition;
                    selectedCharacter.transform.position = oldPosition;
                    SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
                  }
                }
              }
            }
            if (hit.transform.name.Contains ("Tile"))
            {
              foreach (Character c in character) {
                if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Enemy") 
                {
                  AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
                  break;
                }
                else if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Player")
                {
                  if (usingAbility.ability.abilityType < 0)
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
              if (highlightedTileTargetInRange.Count > 0) 
              {
                foreach (GameObject h in highlightedTileTargetInRange) 
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
          else 
          {
            if (hit.transform.tag == "Player") 
            {
              if (oldCharacterNo < 0) 
              {
                SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
              } 
              else
              {
                 selectedCharacter.gridPosition = oldGridPosition;
                 selectedCharacter.transform.position = oldPosition;
                 SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
              }
            }
          }
        }
      }
    }
  }
  
  public void LateUpdate()
  {
    if (selectedCharacter.positionQueue.Count > 0)
    {
      if(isTouch) CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
      selectedCharacter.MoveToDesTile ();
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
    enemies = container.enemies;
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
      player.ID = character.Count - 1;
      playerCharacterID.Add (player.characterStatus.basicStatus.ID);
      foreach (AbilityStatus a in player.characterStatus.equipedAbility) 
      {
        GetUsedAbility.AddAbility (player.ID, a.ability);
      }
    }

    foreach (Tile a in startEnemy) 
    {
      GameObject aiPlayerObj = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3 (a.transform.position.x, 1.5f, a.transform.position.z),Quaternion.identity);
      aiPlayerObj.transform.GetChild(0).rotation = Quaternion.Euler (0, 90, 0);
      AICharacter aiPlayer = aiPlayerObj.GetComponent<AICharacter> ();
      aiPlayer.SetStatus (enemies.Where(x=>x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).First().enemyID);
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
      aiPlayer.ID = character.Count - 1;
      foreach (AbilityStatus ability in aiPlayer.characterStatus.equipedAbility) 
      {
        GetUsedAbility.AddAbility (aiPlayer.ID, ability.ability);
      }
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
        if (i <= normalAttack.Count-1)
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + normalAttack[i].ability.ID); 
          abilityObj.GetComponent<UsingAbilityManager> ().data = normalAttack [i];
          abilityObjs.Add (abilityObj.GetComponent<UsingAbilityManager> ());
          abilityObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
          if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != -99 && GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != 0)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (true);
            abilityObj.transform.GetChild (1).GetComponent<Text> ().text = GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID).ToString ();
            abilityObj.GetComponent<Toggle> ().interactable = false;
          }
          else if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == 0 || GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == -99)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (false);
          }
        }
        else
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/0000"); 
          abilityObj.GetComponent<Toggle> ().interactable = false;
        }
      }
      GameObject specialAbObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
      specialAbObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(0));
      specialAbObj.transform.localScale = Vector3.one;
      specialAbObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + specialAttack.ability.ID); 
      specialAbObj.GetComponent<UsingAbilityManager> ().data = specialAttack;
      abilityObjs.Add (specialAbObj.GetComponent<UsingAbilityManager> ());
      specialAbObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
      if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != -99 && GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != 0)
      {
        specialAbObj.transform.GetChild (1).gameObject.SetActive (true);
        specialAbObj.transform.GetChild (1).GetComponent<Text> ().text = GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID).ToString ();
        specialAbObj.GetComponent<Toggle> ().interactable = false;
      }
      else if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == 0 || GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == -99)
      {
        specialAbObj.transform.GetChild (1).gameObject.SetActive (false);
      }
    }
    else
    {
      List<AbilityStatus> skill = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 2 || x.ability.abilityType == -2).ToList (); 

      for (int i = 0; i < 3; i++) 
      {
        GameObject abilityObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
        abilityObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(0));
        abilityObj.transform.localScale = Vector3.one;
        if (i <= skill.Count-1)
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + skill[i].ability.ID); 
          abilityObj.GetComponent<UsingAbilityManager> ().data = skill [i];
          abilityObjs.Add (abilityObj.GetComponent<UsingAbilityManager> ());
          abilityObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<ToggleGroup> ();
          if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != -99 && GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != 0)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (true);
            abilityObj.transform.GetChild (1).GetComponent<Text> ().text = GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID).ToString ();
            abilityObj.GetComponent<Toggle> ().interactable = false;
          }
          else if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == 0 || GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == -99)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (false);
          }
        }
        else
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/0000"); 
          abilityObj.GetComponent<Toggle> ().interactable = false;
        }
      }
    }
    
    if (abilityObjs.Count > 0) 
    {
      foreach (UsingAbilityManager a in abilityObjs) 
      {
        if (FinishingGaugeManager.GetInstance ().GetSliderValue () < a.data.ability.gaugeUse)
        {
          a.GetComponent<Toggle> ().interactable = false;
        }
      }
      if (abilityObjs.Where (x => x.GetComponent<Toggle> ().IsInteractable ()).Count () > 0 && !selectedCharacter.played) 
      {
        abilityObjs.Where (x => x.GetComponent<Toggle> ().IsInteractable ()).FirstOrDefault ().GetComponent<Toggle> ().isOn = true;
      }
    }
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
    foreach (GameObject c in highlightedTileTargetInRange) 
    {
      Destroy (c);
    }
    highlightTileMovement.Clear ();
    highlightTileAttack.Clear ();
    highlightedTileTargetInRange.Clear ();
    targetInRange.Clear ();
  }

  public void RemoveAttackHighLightOnly()
  {
    foreach (GameObject a in highlightTileAttack) 
    {
      Destroy (a);
    }
    foreach (GameObject c in highlightedTileTargetInRange) 
    {
      Destroy (c);
    }
    highlightTileAttack.Clear ();
    highlightedTileTargetInRange.Clear ();
    targetInRange.Clear ();
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
    if (selectedCharacter.GetType () == typeof(AICharacter))
      SelectedAbility (selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 1 || x.ability.abilityType == -1).First ());
    else 
    {
      if (!selectedCharacter.played)
      {
        playerUI.transform.parent.GetChild (1).GetChild (0).gameObject.SetActive (true);
      } 
      else 
      {
        playerUI.transform.parent.GetChild (1).GetChild (0).gameObject.SetActive (false);
      }
    }
    Destroy (chaSelector);
    chaSelector = null;
    chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
    chaSelector.transform.SetParent (selectedCharacter.transform);

    if (selectedCharacter.GetType () == typeof(PlayerCharacter))
    {
      playerUI.transform.GetChild (0).gameObject.SetActive (true);
      playerUI.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + selectedCharacter.name);
      playerUI.transform.GetChild (0).GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ().text = selectedCharacter.name.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.characterLevel.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = selectedCharacter.currentHP.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.attack.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (4).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.defense.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (5).GetChild (0).GetComponent<Text> ().text = selectedCharacter.characterStatus.criRate.ToString ();
      SetUseAble (1);
    } 
    else 
    {
      playerUI.transform.GetChild (0).gameObject.SetActive (false);
    }
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
      if (targetInRange.Count > 0)
      {
        if(targetInRange.Where(x=>x.gridPosition == gridPosition && x.GetType() == typeof(AICharacter)).Count()>0)
        {
          Character target =  targetInRange.Where (x => x.gridPosition == gridPosition).First ();
          
          if (highlightTileAttack.Where (x => x.transform.position.x == target.transform.position.x && x.transform.position.z == target.transform.position.z).Count () <= 0)
          {
            MoveCurrentCharacter (CheckingMovementToAttackTarget (map [(int)gridPosition.x] [(int)gridPosition.z].transform));
            
            selectedCharacter.target = target;
          }
          else
          {
            AttackWithCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
          }
        }
        else if (targetInRange.Where(x=>x.gridPosition == gridPosition && x.GetType() == typeof(PlayerCharacter)).Count()>0)
        {
          Character target =  targetInRange.Where (x => x.gridPosition == gridPosition).First ();

          if (highlightTileAttack.Where (x => x.transform.position.x == target.transform.position.x && x.transform.position.z == target.transform.position.z).Count () <= 0)
          {
            MoveCurrentCharacter (CheckingMovementToAttackTarget (map [(int)gridPosition.x] [(int)gridPosition.z].transform));

            selectedCharacter.target = target;
          }
          else
          {
            AttackWithCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
          }
        }
        else 
        {
          if (character.Where (x => x.gridPosition == gridPosition && x.GetType() == typeof(PlayerCharacter)).Count () > 0)
          {
            SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
          } 
          else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0 && !selectedCharacter.played) 
          {
            MoveCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
          }
        }
      }
      else 
      {
        if (character.Where (x => x.gridPosition == gridPosition && x.GetType() == typeof(PlayerCharacter)).Count () > 0)
        {
          SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
        } 
        else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0 && !selectedCharacter.played) 
        {
          MoveCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
        }
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
          if (usingAbility.ability.abilityType < 0)
          {
            int amountOfDamage = Mathf.FloorToInt (selectedCharacter.characterStatus.attack * usingAbility.power) - target.characterStatus.defense;
            if (amountOfDamage <= 0) amountOfDamage = 0;
            showingResultOfAttack.UpdateStatus (selectedCharacter, target, amountOfDamage);
            
            if (usingAbility.ability.gaugeUse > 0 && selectedCharacter.GetType () == typeof(PlayerCharacter))
              FinishingGaugeManager.GetInstance ().ChangeSliderValue (-usingAbility.ability.gaugeUse);
            else if (usingAbility.ability.gaugeUse > 0 && selectedCharacter.GetType () == typeof(AICharacter))
              selectedCharacter.GetComponent<AICharacter> ().rageGuage -= usingAbility.ability.gaugeUse;
            StartCoroutine (WaitDamageFloating (target, -amountOfDamage));
          }
          else
          {
            int healing = Mathf.Clamp (Mathf.FloorToInt (usingAbility.power), Mathf.FloorToInt (usingAbility.power), target.characterStatus.maxHp - target.currentHP);
            showingResultOfAttack.UpdateStatus (selectedCharacter, target, healing);
            StartCoroutine (WaitDamageFloating (target, healing));
          }
          GetUsedAbility.ModifyAbility (selectedCharacter.ID, usingAbility.ability.ID, usingAbility.ability.coolDown);
        }
        break;
      }
    }
  }

  private IEnumerator WaitDamageFloating(Character target, int amountOfResults)
  {
    int i = 0;
    CameraManager.GetInstance ().FocusCamera (selectedCharacter.transform.position, target.transform.position);
    if(target.GetType() == typeof (AICharacter))
      target.GetComponent<AICharacter> ().rageGuage += 1;
    
    while (i < usingAbility.hitAmount) 
    {
      selectedCharacter.transform.GetChild(0).GetComponent<Animator> ().Play ("Test");
      Animator anim = selectedCharacter.transform.GetChild(0).GetComponent<Animator> ();
      target.currentHP += amountOfResults;
      target.GetComponent<Character>().info.transform.GetChild (1).GetComponent<TextMesh> ().text = target.currentHP.ToString();
      if(amountOfResults <= 0) FloatingTextController (amountOfResults*-1, target.transform);
      else FloatingTextController (amountOfResults, target.transform);
      if (target.GetType () == typeof(AICharacter)) FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
      else FinishingGaugeManager.GetInstance ().ChangeSliderValue (2.5f);
      i++;
      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo (0).length * anim.GetCurrentAnimatorStateInfo (0).speed);
    }
    if (target.currentHP <= 0) 
    {
      RemoveDead ();
      if (target.GetType () == typeof(AICharacter))
        FinishingGaugeManager.GetInstance ().ChangeSliderValue (10);
      else
        FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
    }
    CameraManager.GetInstance ().ResetCamera ();
    selectedCharacter.played = true;
    NextTurn ();
  }

  public void NextTurn()
  {
    if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() > 0 && character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() > 0 ) StartCoroutine(WaitEndTurn ());
    else if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() <= 0) StartCoroutine (ShowResults (true));
    else if(character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() <= 0) StartCoroutine (ShowResults (false));
  }

  private IEnumerator WaitEndTurn()
  {
    while (true) 
    {
      Animator anim = selectedCharacter.transform.GetChild(0).GetComponent<Animator> ();

      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo (0).length * anim.GetCurrentAnimatorStateInfo (0).speed);

      break;
    }
    if (currentCharacterIndex == character.Count - 1) 
    {
      foreach(Character c in character)
      {
        foreach(AbilityStatus a in c.characterStatus.equipedAbility)
        {
          if(GetUsedAbility.GetCoolDown(c.ID,a.ability.ID) != -99 && GetUsedAbility.GetCoolDown(c.ID,a.ability.ID) != 0)
            GetUsedAbility.ModifyAbility (c.ID, a.ability.ID,-1);
        }
      }
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
        menu.transform.GetChild (0).gameObject.SetActive (false);
        menu.transform.GetChild (1).gameObject.SetActive (false);
        menu.transform.GetChild (2).gameObject.SetActive (false);
        playerController.gameObject.SetActive (false);
        playerController.GetComponent<PlayerController> ().RemoveSelected ();
        character [currentCharacterIndex].TurnUpdate ();
      }
      else 
      {
        if (isAutoPlay)
        {
          character [currentCharacterIndex].TurnUpdate ();
        }
        else 
        {
          menu.transform.GetChild (0).gameObject.SetActive (true);
          menu.transform.GetChild (1).gameObject.SetActive (true);
          menu.transform.GetChild (2).gameObject.SetActive (true);
          if (!isTouch) 
          {
            playerController.gameObject.SetActive (true);
            playerController.GetComponent<PlayerController> ().SetUpSelectedPosition ();
          }
          else
          {
            playerController.gameObject.SetActive (false);
            playerController.GetComponent<PlayerController> ().RemoveSelected ();
          }
        }
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

    foreach (GameObject obj in highlightedTileTargetInRange) 
    {
      Destroy (obj);
    }
    highlightedTileTargetInRange.Clear ();
    targetInRange.Clear ();

    List<Tile> highlighted = new List<Tile> ();

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z],selectedCharacter.characterStatus.movementPoint, character.Where (x => x.gridPosition != selectedCharacter.gridPosition && x.ordering != selectedCharacter.ordering).Select (x => x.gridPosition).ToArray ()))
    {
      if (usingAbility.ability.rangeType == 2)
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true))
        {
          if(!highlighted.Contains(a))
            highlighted.Add (a);
        }
        foreach (Tile b in TileHighLight.FindHighLight (t, usingAbility.range, true, true)) 
        {
          if(!highlighted.Contains(b))
            highlighted.Add (b);
        }
      } 
      else if (usingAbility.ability.rangeType == 0)
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true))
        {
          if(!highlighted.Contains(a))
            highlighted.Add (a);
        }
      }
      else
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, usingAbility.range, true, true))
        {
          if(!highlighted.Contains(a))
            highlighted.Add (a);
        }
      }
    }

    foreach (GameObject a in highlightTileAttack) 
    {
      if(!highlighted.Contains(a.GetComponentInParent<Tile> ()))
        highlighted.Add(a.GetComponentInParent<Tile> ());
    }
      
    if (usingAbility.ability.abilityType>0) 
    {
      var cha = highlighted.Select (x => GameManager.GetInstance ().character.Where (z => z.currentHP > 0 && z.GetType() != selectedCharacter.GetType() && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
      List<Character> inRangeCha = cha.Where(x=>x != null).ToList ();
      
      foreach(Character c in inRangeCha)
      {
        if (c.tag !=selectedCharacter.tag)
        {
          GameObject inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
          inRange.GetComponent<Renderer> ().material.color = Color.red;
          inRange.transform.SetParent (c.transform);

          highlightedTileTargetInRange.Add (inRange);
          targetInRange.Add (c);
        }
      }
    }
    else
    {
      var cha = highlighted.Select (x => GameManager.GetInstance ().character.Where (z => z.currentHP > 0 && z.GetType() == selectedCharacter.GetType() && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
      List<Character> inRangeCha = cha.Where(x=>x != null).ToList ();

      foreach(Character c in inRangeCha)
      {
        if (c.tag ==selectedCharacter.tag)
        {
          GameObject inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
          inRange.GetComponent<Renderer> ().material.color = Color.red;
          inRange.transform.SetParent (c.transform);

          highlightedTileTargetInRange.Add (inRange);
          targetInRange.Add (c);
        }
      }
    }
  }

  public void RemoveDead()
  {
    Result addResult = new Result ();
    DroppedItem itemGet = new DroppedItem ();
    
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
              itemGet.itemStatus = GetDataFromSql.GetItemFromID (int.Parse (droppedItem [k]));
              itemGet.amount += 1;
              addResult.droppedItem.Add (itemGet);
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
        foreach (AbilityStatus a in character[i].characterStatus.equipedAbility)
        {
          GetUsedAbility.RemoveAbility (i);
        }
        break;
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
  }

  public IEnumerator ShowResults(bool isWin)
  {
    TemporaryData.GetInstance ().playerData.gold += TemporaryData.GetInstance ().result.givenGold;    
    
    while (true)
    {
      yield return new WaitForSeconds (0.5f);

      break;
    }
   
    List<CharacterStatus> party = TemporaryData.GetInstance ().playerData.characters.Where (x => x.isInParty).ToList ();
    List<Transform> showResultCharacters = new List<Transform> ();
    
    for (int i = 0; i < party.Count; i++) 
    {
      if (playerCharacterID.Where (x => x == party [i].basicStatus.ID).Count () > 0) 
      {
        GameObject characterInParty = Instantiate (Resources.Load<GameObject> ("ResultCharacter"));
        characterInParty.transform.SetParent (results.transform.GetChild (0).GetChild (3));
        characterInParty.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + party [i].basicStatus.characterName);
        characterInParty.transform.localScale = new Vector3 (1, 1, 1);
        showResultCharacters.Add (characterInParty.transform);
      }
    }
    AddingResultItem ();
    results.transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenExp.ToString();
    results.transform.GetChild (0).GetChild (2).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenGold.ToString();
    
    if(party.Count > 0 && playerCharacterID.Where(x=>x == party[0].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [0], TemporaryData.GetInstance ().result.givenExp, showResultCharacters[0].GetChild(0)));
    if(party.Count > 1 && playerCharacterID.Where(x=>x == party[1].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [1], TemporaryData.GetInstance ().result.givenExp, showResultCharacters[1].GetChild(0)));
    if(party.Count > 2 && playerCharacterID.Where(x=>x == party[2].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [2], TemporaryData.GetInstance ().result.givenExp,showResultCharacters[2].GetChild(0)));
    if(party.Count > 3 && playerCharacterID.Where(x=>x == party[3].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [3], TemporaryData.GetInstance ().result.givenExp,showResultCharacters[3].GetChild(0)));

    while (true)
    {
      yield return new WaitForSeconds (1f);

      break;
    }

    if (isWin)
      results.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite>("GamePlay/Image/Winner");
    else
      results.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite>("GamePlay/Image/Loser");

    results.SetActive (true);
    
    while (true) 
    {
      if (/*Input.GetMouseButtonDown (0) && */SystemManager.isFinishLevelUp && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
      {
        AddingResultItem ();
        TemporaryData.GetInstance ().result = new Result ();
        SceneManager.LoadScene ("MainMenuScene");
        break;
      }
	    yield return 0;
    }
  }
  
  private void AddingResultItem()
  {
    for (int i = TemporaryData.GetInstance ().result.droppedItem.Count-1; i >= 0; i--) 
    {
      Item addedItem = new Item ();
      for(int j = 0; j < TemporaryData.GetInstance ().result.droppedItem[0].amount;j++)
      {
        addedItem = new Item ();
        addedItem.item = TemporaryData.GetInstance ().result.droppedItem [0].itemStatus;
        addedItem.equiped = false;
        addedItem.ordering = TemporaryData.GetInstance ().playerData.inventory.Count;
        TemporaryData.GetInstance ().playerData.inventory.Add (addedItem);
      }
      
      GameObject itemObj = Instantiate (Resources.Load<GameObject> ("Item/ItemGet"));
      itemObj.transform.SetParent (results.transform.GetChild (0).GetChild (4).GetChild(0));
      itemObj.transform.localScale = Vector3.one;
      itemObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + addedItem.item.name);
      itemObj.transform.GetChild (1).GetComponent<Text> ().text = addedItem.item.name.ToString();
      itemObj.transform.GetChild (2).GetComponent<Text> ().text = TemporaryData.GetInstance ().result.droppedItem [0].amount.ToString();
      TemporaryData.GetInstance ().result.droppedItem.RemoveAt (0);
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
  
  public void ShowFakeResult()
  {
    TemporaryData.GetInstance ().result.givenExp = 1000;
    
    StartCoroutine (ShowResults (true));
  }
  
  public void PauseGame()
  {
    isPause = !isPause;
    if (isPause)
    {
      Time.timeScale = 0;
      pauseMenu.SetActive (true);
    } 
    else 
    {
      Time.timeScale = 1;
      pauseMenu.SetActive (false);
    }
  }
  
  public void RestartGame()
  {
    SceneManager.LoadScene ("GamePlayScene");
    Time.timeScale = 1;
  }
  
  public void Surrender()
  {
    SceneManager.LoadScene ("MainMenuScene");
    Time.timeScale = 1;
  }
}
