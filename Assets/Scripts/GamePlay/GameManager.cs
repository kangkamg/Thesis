using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager GetInstance() { return instance;}

  public int[] _mapSize = new int[]{1,1};
  public int currentCharacterIndex;
  public Transform mapTransform;

  public List<List<Tile>> map = new List<List<Tile>>();
  public List<EnemyInMapData> enemies = new List<EnemyInMapData>();
  public List<PlayerInMapData> players = new List<PlayerInMapData>();
  public List<Vector3> objectivePos = new List<Vector3>();
  public int mapObjective;
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
  public GameObject changingTurn;
  public GameObject objective;
  public ShowingResultOfAttack showingResultOfAttack;
  public Transform playerController;

  List<Character> targetInRange = new List<Character> ();
  List<GameObject> highlightedTileTargetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  public int oldCharacterNo = -1;
  private int amountOfDamage;
  public bool isPlayerTurn = true;
  public bool isAutoPlay = false;
  public bool isTouch = true;
  public bool isPause = false;
  public bool isChangingTurn = false;
  
  public bool hitButton = false;

  public AbilityStatus usingAbility;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (PlayerPrefs.GetInt(Const.MapNo,1));
    GenerateCharacter ();
    
    isPause = false;
    isAutoPlay = false;
    isPlayerTurn = true;
    playerController.gameObject.SetActive (false);
    results.SetActive (false);
    pauseMenu.SetActive (false);
    showingResultOfAttack.gameObject.SetActive (false);
    playerUI.transform.GetChild (0).gameObject.SetActive (false);
    changingTurn.SetActive (false);
    
    if (!TemporaryData.GetInstance ().isTutorialDone)
      menu.transform.GetChild (2).gameObject.SetActive (false);
    
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene ().name);
  }

  private void Start()
  {
    if (!TemporaryData.GetInstance ().isTutorialDone) gameObject.AddComponent<Tutorial> ();
      StartCoroutine (InitGame ());
  }
  
  private void Update()
  {    
    foreach (Touch toches in Input.touches) 
    {
      if (Input.touchCount > 1) break;
      if (showingResultOfAttack.gameObject.activeSelf || isChangingTurn || isPause || changingTurn.activeSelf) continue;

      if (Input.touchCount  ==  1 && Input.GetTouch(0).phase == TouchPhase.Began)
      {
        hitButton = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
      }
      
      if (Input.touchCount  ==  1 && Input.GetTouch(0).phase == TouchPhase.Moved)
      {
        break;
      }

      Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
      
      RaycastHit hit;
      
      if (Physics.Raycast (ray, out hit, 1000f)) 
      {
        if (/*Input.GetMouseButtonDown(0) &&*/ !hitButton && isPlayerTurn && isTouch && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && !CameraManager.GetInstance().isMoving)
        {
          if (selectedCharacter != null)
          {
            if (!selectedCharacter.played && !selectedCharacter.isActioning) {
              if (hit.transform.tag == "Player") {
                if (oldCharacterNo < 0) {
                  SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
                } else {
                  if (oldCharacterNo != hit.transform.GetComponent<PlayerCharacter> ().ordering) {
                    if (usingAbility.ability.abilityType < 0 && highlightedTileTargetInRange.Count > 0) {
                      foreach (GameObject h in highlightedTileTargetInRange) {
                        if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) {
                          if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0) {
                            Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                            MoveCurrentCharacter (desTile);

                            selectedCharacter.target = hit.transform.GetComponent<Character> ();

                            break;
                          } else {
                            AttackWithCurrentCharacter (map [(int)hit.transform.GetComponent<Character> ().gridPosition.x] [(int)hit.transform.GetComponent<Character> ().gridPosition.z]);
                            selectedCharacter.target = hit.transform.GetComponent<Character> ();
                            break;
                          }
                        }
                      }
                    } else {
                      SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
                    }
                  } else {
                    CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
                  }
                }
              }
              if (hit.transform.name.Contains ("Tile")) {
                foreach (Character c in character) {
                  
                  if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Enemy") {
                    if (highlightedTileTargetInRange.Count > 0) {
                      foreach (GameObject h in highlightedTileTargetInRange) {
                        if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) {
                          if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0) {

                            Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                            MoveCurrentCharacter (desTile);

                            selectedCharacter.target = c;

                            break;
                          } else {
                            AttackWithCurrentCharacter (map [(int)c.gridPosition.x] [(int)c.gridPosition.z]);
                            selectedCharacter.target = c;
                            break;
                          }
                        }
                      }
                    } else {
                      RemoveSelectedCharacter ();
                      ShowPlayerUI (true, hit.transform.GetComponent<Character> ());
                    }
                  } else if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Player") {
                    if (oldCharacterNo != hit.transform.GetComponent<PlayerCharacter> ().ordering) {
                      if (usingAbility.ability.abilityType < 0 && highlightedTileTargetInRange.Count > 0) {
                        foreach (GameObject h in highlightedTileTargetInRange) {
                          if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) {
                            if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0) {
                              Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                              MoveCurrentCharacter (desTile);

                              selectedCharacter.target = c;

                              break;
                            } else {
                              AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
                              selectedCharacter.target = c;
                              break;
                            }
                          }
                        }
                      } else {
                        SelectedCharacter (c);
                        break;
                      }
                    } else {
                      CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
                    } 
                  } else if (c.gridPosition != hit.transform.GetComponent<Tile> ().gridPosition) {
                    if (highlightTileMovement.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () > 0) {
                      MoveCurrentCharacter (hit.transform.GetComponent<Tile> ());
                      break;
                    } else {
                      if (selectedCharacter.transform.position != oldPosition && selectedCharacter.gridPosition != oldGridPosition) {
                        selectedCharacter.gridPosition = oldGridPosition;
                        selectedCharacter.transform.position = oldPosition;
                        CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
                        break;
                      } else {
                        RemoveSelectedCharacter ();
                        break;
                      }
                    } 
                  }
                }
              }
              if (hit.transform.tag == "Enemy") {
                if (highlightedTileTargetInRange.Count > 0) {
                  foreach (GameObject h in highlightedTileTargetInRange) {
                    if (h.transform.position.x == hit.transform.position.x && h.transform.position.z == hit.transform.position.z) {
                      if (highlightTileAttack.Where (x => x.transform.position.x == hit.transform.position.x && x.transform.position.z == hit.transform.position.z).Count () <= 0) {

                        Tile desTile = CheckingMovementToAttackTarget (hit.transform);

                        MoveCurrentCharacter (desTile);

                        selectedCharacter.target = hit.transform.GetComponent<Character> ();

                        break;
                      } else {
                        AttackWithCurrentCharacter (map [(int)hit.transform.GetComponent<Character> ().gridPosition.x] [(int)hit.transform.GetComponent<Character> ().gridPosition.z]);
                        selectedCharacter.target = hit.transform.GetComponent<Character> ();
                        break;
                      }
                    }
                  }
                } else {
                  RemoveSelectedCharacter ();
                  ShowPlayerUI (true, hit.transform.GetComponent<Character> ());
                }
              }
            } 
            else if (selectedCharacter.played)
            {
              if (hit.transform.tag == "Player") 
              {
                SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
              } else if (hit.transform.tag == "Tile")
              {
                RemoveSelectedCharacter ();
              } else if (hit.transform.tag == "Enemy") 
              {
                RemoveSelectedCharacter ();
                ShowPlayerUI (true, hit.transform.GetComponent<Character> ());
              }
            }
            else if (selectedCharacter.isActioning)
            {
              
            }
          } 
          else 
          {
            if (hit.transform.tag == "Player") 
            {
              SelectedCharacter (hit.transform.GetComponent<PlayerCharacter> ());
            }
            else if (hit.transform.tag == "Tile") 
            {
              RemoveSelectedCharacter ();
            }
            else if (hit.transform.tag == "Enemy") 
            {
              RemoveSelectedCharacter ();
              ShowPlayerUI (true, hit.transform.GetComponent<Character> ());
            }
          }
        }
      }
    }
  }
  
  public void LateUpdate()
  {
    if (selectedCharacter != null) 
    {
      if (selectedCharacter.positionQueue.Count > 0)
      {
        CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
        selectedCharacter.MoveToDesTile ();
      }
    }
  }
  
  private IEnumerator InitGame()
  { 
    CameraManager.GetInstance ().SetUpStartCamera (new Vector3(_mapSize[0] + 2,20,_mapSize[1]+3));
    playerUI.SetActive(false);
    menu.SetActive (false);
    
    yield return new WaitForSeconds (1f);
    
    objective.SetActive(true);
    
    if(mapObjective == 1)
    {
      objective.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ().text = "Kill All Enemys";
    }
    else if(mapObjective == 3)
    {
      if (character.Where (x => x.characterStatus.basicStatus.ID >= 3000).Count () > 0) 
      {
        objective.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ().text = "Kill The " + character.Where (x => x.characterStatus.basicStatus.ID >= 3000).First().characterStatus.basicStatus.characterName;
        
        CameraManager.GetInstance ().transform.eulerAngles = new Vector3 (40, -45, 0);
        
        CameraManager.GetInstance ().MoveCameraToTarget (character.Where (x => x.characterStatus.basicStatus.ID >= 3000).First ().transform);
      }
    } 
    
    yield return new WaitForSeconds (1.5f);
    
    objective.SetActive(false);
      
    StartCoroutine (ChangingTurn ());
    
    yield return new WaitForSeconds (1f);
    
    isTouch = true;
    
    playerUI.SetActive(true);
    menu.SetActive (true);
    menu.transform.GetChild (0).gameObject.SetActive (true);
    menu.transform.GetChild (1).gameObject.SetActive (true);
    
    
    CameraManager.GetInstance ().transform.eulerAngles = new Vector3 (40, -45, 0);
    
    SelectedCharacter (character [0]);
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
    List<ObstacleInMap> obstacle = new List<ObstacleInMap> ();
    
    _mapSize = container.size;

    map = new List<List<Tile>> ();
    obstacle = container.objs;
    mapObjective = container.mapObjective;
    if (mapObjective == 2) objectivePos = container.objectivePos;
    
    for (int x = 0; x < _mapSize[0]; x++)
    {
      List<Tile> row = new List<Tile> ();
      for (int z = 0; z < _mapSize[1]; z++)
      {
        GameObject tileObj = Instantiate (PrefabHolder.GetInstance ().Base_TilePrefab, new Vector3 ((PrefabHolder.GetInstance ().Base_TilePrefab.transform.localScale.x * x) - Mathf.Floor (_mapSize[0] / 2), 0,
          (PrefabHolder.GetInstance ().Base_TilePrefab.transform.localScale.z * z) - Mathf.Floor (_mapSize[1] / 2)),Quaternion.Euler (new Vector3 (0, 0, 0)));
        Tile tile = tileObj.GetComponent<Tile> ();
        tile.gridPosition = new Vector3 (x, 0, z);
        tile.SetType ((TileTypes)container.tiles.Where(a=>a.locX == x && a.locZ ==z).First().type);
        if (obstacle.Where (a => a.locX == x && a.locZ == z).Count () > 0) 
        {
          
          GameObject obstacleObj = Instantiate (Resources.Load<GameObject> ("TilePrefab/Obstacle/" + obstacle.Where (a => a.locX == x && a.locZ == z).First ().objs));
          obstacleObj.transform.SetParent (tileObj.transform);
          obstacleObj.transform.localPosition = new Vector3 (0, obstacleObj.GetComponent<ObstacleTypes>().yPos,0);  
          tile.SetImpassible ();
        }
        tile.transform.SetParent (mapTransform);
        row.Add (tile);
      }
      map.Add(row);
    }
      
    for (int x = 0; x < map.Count; x++)
    {
      for (int z = 0; z < map[x].Count; z++)
      {
        map [x] [z].GenerateNeighbors ();
      }
    }
      
    mapTransform.gameObject.SetActive (true);
    enemies = container.enemies;
    players = container.players;
  }
  
  public void GenerateCharacter()
  {
    List<Tile> startPlayer = new List<Tile> ();
    List<Tile> startEnemy = new List<Tile> ();

    foreach (List<Tile> t in map)
    {
      foreach (Tile a in t) 
      {
        if (players.Where(x=>x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).Count()>0) 
        {
          startPlayer.Add (a);
          continue;
        }
        else if (enemies.Where(x=>x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).Count()>0) 
        {
          startEnemy.Add (a);
        }
      }
    }

    for(int i = 0; i < startPlayer.Count;i++) 
    {
      if (i >= TemporaryData.GetInstance ().playerData.characters.Where (x => x.isInParty).Count ()) 
      {
        break; 
      }

      GameObject playerObj = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3 (startPlayer [i].transform.position.x, 2.4f, startPlayer [i].transform.position.z),Quaternion.identity);
      
      PlayerCharacter player = playerObj.GetComponent<PlayerCharacter> ();
      player.SetStatus(TemporaryData.GetInstance ().playerData.characters.Where (x => x.partyOrdering == i).First());
      player.gridPosition = startPlayer[i].gridPosition;
      character.Add (player);
      player.ordering = character.Count - 1;
      player.ID = character.Count - 1;
      playerCharacterID.Add (player.characterStatus.basicStatus.ID);
      GameObject renderer = null;
      if (Resources.Load<GameObject> ("PlayerPrefab/" + player.characterStatus.basicStatus.ID) != null)
      {
        renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/" + player.characterStatus.basicStatus.ID));
      }
      else
      {
        renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/0000"));
      }
      renderer.transform.SetParent (playerObj.transform);
      renderer.transform.SetAsFirstSibling ();
      renderer.transform.localScale = Vector3.one*2;
      renderer.transform.localPosition = new Vector3(0,-0.46f,0);
      renderer.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));  
      
      foreach (AbilityStatus a in player.characterStatus.equipedAbility) 
      {
        GetUsedAbility.AddAbility (player.ID, a.ability);
      }
    }

    foreach (Tile a in startEnemy) 
    {
      GameObject aiPlayerObj = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3 (a.transform.position.x, 2.4f, a.transform.position.z),Quaternion.identity);
      
      AICharacter aiPlayer = aiPlayerObj.GetComponent<AICharacter> ();
      aiPlayer.SetStatus (enemies.Where(x=>x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).First().enemyID, enemies.Where(x=>x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).First().level);
      aiPlayer.aiStyle = enemies.Where (x => x.locX == a.gridPosition.x && x.locZ == a.gridPosition.z).First ().style;
      
      for(int i= 0; i < aiPlayer.characterStatus.basicStatus.learnAbleAbility.Count;i++)
      {
        AbilityStatus equiped = new AbilityStatus ();
        string learnAbleAbility = aiPlayer.characterStatus.basicStatus.learnAbleAbility [i];
        string[] learnAbleAb = learnAbleAbility.Split (" " [0]);
        for(int j = 0; j < learnAbleAb.Length; j=j+2)
        {
          if(int.Parse(learnAbleAb[j+1]) <= aiPlayer.characterStatus.characterLevel)
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
      GameObject renderer = null;
      
      if (Resources.Load<GameObject> ("PlayerPrefab/" + aiPlayer.characterStatus.basicStatus.ID) != null)
      {
        renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/" + aiPlayer.characterStatus.basicStatus.ID),Vector3.zero,Quaternion.identity);
      }
      else
      {
        renderer = Instantiate (Resources.Load<GameObject> ("PlayerPrefab/0000"));
      }
      renderer.transform.SetParent (aiPlayerObj.transform);
      renderer.transform.SetAsFirstSibling ();
      renderer.transform.localScale = Vector3.one*2;
      renderer.transform.localPosition = new Vector3(0,-0.46f,0);
      
      Vector3 heading = character [0].transform.position - aiPlayerObj.transform.position;
      float distance = heading.magnitude;
      
      Vector3 direction = heading / distance;
      
      List<Tile> neighbors = new List<Tile> ();
      
      if (direction.x <= 0 && direction.z < 0)
      {
        neighbors = map [(int)aiPlayer.gridPosition.x];
        Vector3 rotateDirection = neighbors.Where (x => x.gridPosition.x == aiPlayer.gridPosition.x && x.gridPosition.z == aiPlayer.gridPosition.z-1).First ().transform.position;
        rotateDirection.y = aiPlayerObj.transform.position.y;
        aiPlayer.RotateTo (rotateDirection);
      }
      else if(direction.x <= 0 && direction.z == 0)
      {
        neighbors = map [(int)aiPlayer.gridPosition.x-1];
        Vector3 rotateDirection = neighbors.Where (x => x.gridPosition.x == aiPlayer.gridPosition.x-1 && x.gridPosition.z == aiPlayer.gridPosition.z).First ().transform.position;
        rotateDirection.y = aiPlayerObj.transform.position.y;
        aiPlayer.RotateTo (rotateDirection);
      }
      else if(direction.x > 0 && direction.z == 0)
      {
        neighbors = map [(int)aiPlayer.gridPosition.x+1];
        Vector3 rotateDirection = neighbors.Where (x => x.gridPosition.x == aiPlayer.gridPosition.x+1 && x.gridPosition.z == aiPlayer.gridPosition.z).First ().transform.position;
        rotateDirection.y = aiPlayerObj.transform.position.y;
        aiPlayer.RotateTo (rotateDirection);
      }
      else if(direction.x >= 0 && direction.z > 0)
      {
        neighbors = map [(int)aiPlayer.gridPosition.x];
        Vector3 rotateDirection = neighbors.Where (x => x.gridPosition.x == aiPlayer.gridPosition.x && x.gridPosition.z == aiPlayer.gridPosition.z+1).First ().transform.position;
        rotateDirection.y = aiPlayerObj.transform.position.y;
        aiPlayer.RotateTo (rotateDirection);
      }
      
      foreach (AbilityStatus ability in aiPlayer.characterStatus.equipedAbility) 
      {
        GetUsedAbility.AddAbility (aiPlayer.ID, ability.ability);
      }
    }
  }
  
  private void SetUseAble(int page)
  {
    foreach (Transform child in playerUI.transform.GetChild (0).GetChild (1).GetChild(1))
    {
      Destroy (child.gameObject);
    }
    
    List<UsingAbilityManager> abilityObjs = new List<UsingAbilityManager> ();
    
    List<AbilityStatus> normalAttack = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 1 || x.ability.abilityType == -1).ToList (); 
    
    AbilityStatus specialAttack = null;
    if(selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 3 || x.ability.abilityType == -3).Count() > 0)
      specialAttack = selectedCharacter.characterStatus.equipedAbility.Where (x => x.ability.abilityType == 3 || x.ability.abilityType == -3).First ();
      
      for (int i = 0; i < 2; i++) 
      {
        GameObject abilityObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
        abilityObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(1));
        abilityObj.transform.localScale = Vector3.one;
        if (i <= normalAttack.Count-1)
        {
          if(Resources.Load<Sprite> ("Ability/Normal/" +  normalAttack[i].ability.ID) != null)
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/" + normalAttack[i].ability.ID);
          else
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/" + normalAttack[i].ability.abilityEff);
        
          abilityObj.GetComponent<UsingAbilityManager> ().data = normalAttack [i];
          abilityObjs.Add (abilityObj.GetComponent<UsingAbilityManager> ());
          abilityObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<ToggleGroup> ();
          if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != -99 && GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != 0)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (true);
            abilityObj.transform.GetChild (1).GetChild(1).GetComponent<Text> ().text = GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID).ToString ();
            abilityObj.transform.GetChild (1).GetComponent<Slider> ().maxValue = abilityObj.GetComponent<UsingAbilityManager> ().data.ability.coolDown;
            abilityObj.transform.GetChild (1).GetComponent<Slider> ().value = GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID);
            abilityObj.GetComponent<Toggle> ().interactable = false;
          }
          else if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == 0 || GetUsedAbility.GetCoolDown (selectedCharacter.ID, abilityObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == -99)
          {
            abilityObj.transform.GetChild (1).gameObject.SetActive (false);
          }
        }
        else
        {
          abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/0"); 
          abilityObj.GetComponent<Toggle> ().interactable = false;
          abilityObj.transform.GetChild (1).gameObject.SetActive (true);
          abilityObj.transform.GetChild (1).GetComponent<Slider> ().maxValue = 1;
          abilityObj.transform.GetChild (1).GetComponent<Slider> ().value = 1;
        }
      }
      GameObject specialAbObj = Instantiate (Resources.Load<GameObject> ("GamePlay/UsedAbility"));
      specialAbObj.transform.SetParent (playerUI.transform.GetChild (0).GetChild (1).GetChild(1));
      specialAbObj.transform.localScale = Vector3.one;
    
    if (specialAttack != null) 
    {
      if (Resources.Load<Sprite> ("Ability/Special/" + specialAttack.ability.ID) != null)
        specialAbObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Special/" + specialAttack.ability.ID);
      else
        specialAbObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Special/" + specialAttack.ability.abilityEff);
    
      specialAbObj.GetComponent<UsingAbilityManager> ().data = specialAttack;
      abilityObjs.Add (specialAbObj.GetComponent<UsingAbilityManager> ());
      specialAbObj.GetComponent<Toggle> ().group = playerUI.transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<ToggleGroup> ();
    
      if (FinishingGaugeManager.GetInstance ().GetSliderValue () >= specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.gaugeUse) 
      {
        if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != -99 && GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) != 0) {
          specialAbObj.transform.GetChild (1).gameObject.SetActive (true);
          specialAbObj.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID).ToString ();
          specialAbObj.transform.GetChild (1).GetComponent<Slider> ().maxValue = specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.coolDown;
          specialAbObj.transform.GetChild (1).GetComponent<Slider> ().value = GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID);
          specialAbObj.GetComponent<Toggle> ().interactable = false;
        } 
        else if (GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == 0 || GetUsedAbility.GetCoolDown (selectedCharacter.ID, specialAbObj.GetComponent<UsingAbilityManager> ().data.ability.ID) == -99) {
          specialAbObj.transform.GetChild (1).gameObject.SetActive (false);
        }
      } 
      else 
      {
        specialAbObj.transform.GetChild (1).gameObject.SetActive (true);
        specialAbObj.transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = "Gauge\nNotEnough";
        specialAbObj.transform.GetChild (1).GetComponent<Slider> ().maxValue = 1;
        specialAbObj.transform.GetChild (1).GetComponent<Slider> ().value = 1;
        specialAbObj.GetComponent<Toggle> ().interactable = false;
      }
    }
    else
    {
      specialAbObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/0");
      specialAbObj.GetComponent<Toggle> ().interactable = false;
      specialAbObj.transform.GetChild (1).GetComponent<Slider> ().maxValue = 1;
      specialAbObj.transform.GetChild (1).GetComponent<Slider> ().value = 1;
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
      
      highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, character.Where (x => x.gridPosition != originLocation && x.ordering != character.Where(z=>z.gridPosition == originLocation).First().ordering).Select (x => x.gridPosition).ToArray ());

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
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, false)) 
          {
            highlightedTiles.Add (t);
          }
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, true))
          {
            highlightedTiles.Add (t);
          }
      } 
      else if (type == 1) 
      {
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, true))
          {
            highlightedTiles.Add (t);
          }
      } 
      else 
      {
        foreach (Tile t in TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, true, false)) 
          {
            highlightedTiles.Add (t);
          }
      }
      
      for(int i = 0;i<highlightedTiles.Count;i++)
      {
        if (usingAbility.ability.abilityType > 0)
        {
            if (character.Where (x => x.GetType () == selectedCharacter.GetType ()  && x.gridPosition == highlightedTiles [i].gridPosition).Count() > 0)
            {
              highlightedTiles.RemoveAt (i);
            }
        }
        else if (usingAbility.ability.abilityType < 0)
        {
          if (character.Where (x => x.GetType () != selectedCharacter.GetType () && x.gridPosition == highlightedTiles [i].gridPosition).Count() > 0)
          {
            highlightedTiles.RemoveAt (i);
          }
        }
      }
      
      foreach (Tile t in highlightedTiles) 
      {
        if (highlightTileMovement.Where (x => x.GetComponentInParent<Tile> ().gridPosition == t.gridPosition).Count () <= 0) 
        {
            GameObject h = Instantiate (highlight, t.transform.position + (0.51f * Vector3.up), Quaternion.Euler (new Vector3 (90, 0, 0)))as GameObject;
            h.transform.SetParent (t.transform);
            h.name = "highlightAttack";
            highlightTileAttack.Add (h);
        }
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

    if (targetTile.Count > 0) 
    {
      targetTile.Sort (delegate(Tile a, Tile b)
        {
          return(Vector3.Distance (a.transform.position, selectedCharacter.transform.position).CompareTo (Vector3.Distance (b.transform.position, selectedCharacter.transform.position)));
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
  
  public void RemoveTargetInRangeHighlight()
  {
    foreach (GameObject c in highlightedTileTargetInRange) 
    {
      Destroy (c);
    }
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
  
  public void RemoveSelectedCharacter()
  {
    if (selectedCharacter != null) 
    {
      selectedCharacter.transform.position = oldPosition;
      selectedCharacter.gridPosition = oldGridPosition;
    }
    
    selectedCharacter = null;
    oldPosition = Vector3.zero;
    oldGridPosition = Vector3.zero;
    currentCharacterIndex = -1;
    oldCharacterNo = -1;
    RemoveMapHighlight ();
    if (chaSelector != null) 
    {
      Destroy (chaSelector);
      chaSelector = null;
    }
    playerUI.transform.GetChild (0).gameObject.SetActive (false);
    menu.transform.GetChild (0).gameObject.SetActive (false);
  }
  
  private IEnumerator ChangingTurn()
  {
    isPause = true;
    selectedCharacter = null;
    oldGridPosition = Vector3.zero;
    oldPosition = Vector3.zero;
    oldCharacterNo = -1;
    
    changingTurn.SetActive (true);
    
    playerUI.transform.GetChild (0).gameObject.SetActive (false);
    menu.transform.GetChild (0).gameObject.SetActive (false);
    menu.transform.GetChild (1).gameObject.SetActive (false);
    playerController.gameObject.SetActive (false);
        
    if (isPlayerTurn) 
    {
      changingTurn.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "PlayerTurn";
      foreach (Character c in character)
      {
        if (c.GetType () == typeof(AICharacter))
        {
          foreach (Renderer a in c.transform.GetChild(0).GetComponent<CharacterModelManager>().materials) 
          {
            a.material.color = Color.white;
          }
        }
      }
    }
    else
    {
      playerController.GetComponent<PlayerController> ().RemoveSelected ();
      changingTurn.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = "EnemyTurn";
      foreach (Character c in character)
      {
        if (c.GetType () == typeof(PlayerCharacter))
        {
          foreach (Renderer a in c.transform.GetChild(0).GetComponent<CharacterModelManager>().materials) 
          {
            a.material.color = Color.white;
          }
        }
        else
          continue;
      }
    }
    
    yield return new WaitForSeconds (1f);
    
    changingTurn.SetActive (false);
    isChangingTurn = false;
    isPause = false;
    
    yield return 0;
  }
  
  public void SelectedCharacter(Character playerSelected)
  {
    if (oldCharacterNo >= 0) 
    {
      if (!selectedCharacter.played) 
      {
        selectedCharacter.gridPosition = oldGridPosition;
        selectedCharacter.transform.position = oldPosition;
      }
    }
    
    RemoveMapHighlight ();
    selectedCharacter = playerSelected;
    oldGridPosition = selectedCharacter.gridPosition;
    oldPosition = selectedCharacter.transform.position;
    currentCharacterIndex = selectedCharacter.ordering;
    oldCharacterNo = selectedCharacter.ordering;
    CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
      
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
    
    if (selectedCharacter.GetType () != typeof(AICharacter))
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
    
    if (chaSelector != null)
    {
      Destroy (chaSelector);
      chaSelector = null;
    }
    if (chaSelector == null) 
    {
      chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
      chaSelector.transform.SetParent (selectedCharacter.transform);
      chaSelector.transform.localPosition = new Vector3 (0, -0.47f, 0);
    }

    ShowPlayerUI (true);
    if(selectedCharacter.GetType() == typeof(PlayerCharacter))
    {
      SetUseAble (1);
    }
  }

  public void MoveCurrentCharacter(Tile desTile)
  {
    if (selectedCharacter != null)
    {
      List<Vector3> occupied = character.Where (x => x.gridPosition != desTile.gridPosition && x.gridPosition != selectedCharacter.gridPosition).Select (x => x.gridPosition).ToList ();

      foreach (List<Tile> t in map) 
      {
        foreach (Tile a in t)
        {
          if (!a.canMove)
            occupied.Add (a.gridPosition);
        }
      }

      foreach (GameObject h in highlightTileMovement)
      {
        if (desTile.gridPosition == h.GetComponentInParent<Tile> ().gridPosition && selectedCharacter.positionQueue.Count == 0) 
        {
          RemoveAttackHighLightOnly ();
          foreach (Tile t in TilePathFinder.FindPathPlus(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile, occupied.ToArray())) 
          {
            selectedCharacter.positionQueue.Add (map [(int)t.gridPosition.x] [(int)t.gridPosition.z].transform.position + selectedCharacter.transform.position.y * Vector3.up);
          }
          selectedCharacter.transform.GetChild (0).GetComponent<Animator> ().SetInteger ("animatorIndex", 3);
          selectedCharacter.gridPosition = desTile.gridPosition;
          selectedCharacter.isActioning = true;
          break;
        } 
      }
    }
  }
  
  public void CheckingSelectedTile(Vector3 gridPosition)
  {
    if (selectedCharacter != null) 
    {
      if (gridPosition.z >= 0 && gridPosition.x >= 0 && gridPosition.z < map [(int)gridPosition.x].Count && gridPosition.x < map.Count) 
      {
        if (targetInRange.Count > 0) 
        {
          if (targetInRange.Where (x => x.gridPosition == gridPosition && x.GetType () == typeof(AICharacter)).Count () > 0)
          {
            Character target = targetInRange.Where (x => x.gridPosition == gridPosition).First ();
          
            if (highlightTileAttack.Where (x => x.transform.position.x == target.transform.position.x && x.transform.position.z == target.transform.position.z).Count () <= 0) 
            {
              MoveCurrentCharacter (CheckingMovementToAttackTarget (map [(int)gridPosition.x] [(int)gridPosition.z].transform));
            
              selectedCharacter.target = target;
            } 
            else
            {
              AttackWithCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
              selectedCharacter.target = target;
            }
          } 
          else if (targetInRange.Where (x => x.gridPosition == gridPosition && x.GetType () == typeof(PlayerCharacter)).Count () > 0) 
          {
            Character target = targetInRange.Where (x => x.gridPosition == gridPosition).First ();

            if (highlightTileAttack.Where (x => x.transform.position.x == target.transform.position.x && x.transform.position.z == target.transform.position.z).Count () <= 0)
            {
              MoveCurrentCharacter (CheckingMovementToAttackTarget (map [(int)gridPosition.x] [(int)gridPosition.z].transform));

              selectedCharacter.target = target;
            }
            else
            {
              AttackWithCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
              selectedCharacter.target = target;
            }
          } 
          else 
          {
            if (character.Where (x => x.gridPosition == gridPosition && x.GetType () == typeof(PlayerCharacter)).Count () > 0) 
            {
              SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
            } 
            else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0)
            {
              if (!selectedCharacter.played)
              {
                MoveCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
              } 
              else
              {
                RemoveSelectedCharacter ();
              }
            }
            else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () <= 0)
            {
              RemoveSelectedCharacter ();
            } 
          }
        } 
        else 
        {
          if (character.Where (x => x.gridPosition == gridPosition && x.GetType () == typeof(PlayerCharacter)).Count () > 0) 
          {
            SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
          } 
          else if (character.Where (x => x.gridPosition == gridPosition && x.GetType () == typeof(AICharacter)).Count () > 0)
          {
            RemoveSelectedCharacter ();
            ShowPlayerUI (true, character.Where (x => x.gridPosition == gridPosition).First ());
          } 
          else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () > 0 && !selectedCharacter.played)
          {
            if (!selectedCharacter.played)
            {
              MoveCurrentCharacter (map [(int)gridPosition.x] [(int)gridPosition.z]);
            } 
            else
            {
              RemoveSelectedCharacter ();
            }
          }
          else if (highlightTileMovement.Where (x => x.transform.parent.GetComponent<Tile> ().gridPosition == gridPosition).Count () <= 0)
          {
            RemoveSelectedCharacter ();
          } 
        }
      } 
      else
        Debug.Log ("Invalid Tile");
    }
    else 
    {
      if (character.Where (x => x.gridPosition == gridPosition).Count () > 0)
      {
        if (character.Where (x => x.gridPosition == gridPosition).GetType () != typeof(AICharacter))
            SelectedCharacter (character.Where (x => x.gridPosition == gridPosition).First ());
        else
        {
          RemoveSelectedCharacter ();
          ShowPlayerUI (true, character.Where (x => x.gridPosition == gridPosition).First ());
        }
      }
      else
      {
        RemoveSelectedCharacter ();
      }
    }
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
          
          selectedCharacter.skillText.transform.parent.gameObject.SetActive (true);
          selectedCharacter.skillText.text = usingAbility.ability.abilityName;
          
          selectedCharacter.transform.GetChild(0).rotation = Quaternion.LookRotation (Vector3.RotateTowards (selectedCharacter.transform.GetChild(0).forward, target.transform.position - selectedCharacter.transform.position, 360f, 0.0f));
          if (usingAbility.ability.abilityType > 0)
          {
            float multiply = 1f;
            if (target.GetType() == typeof(AICharacter)) 
            {
              if (target.GetComponent<AICharacter> ().aiInfo.effectiveAttack == usingAbility.ability.abilityEff)
                multiply = 1.35f;
              else
                multiply = 0.9f;
            }
            
            bool isCritical = false;
            int randomCri = UnityEngine.Random.Range (0, 101);
            
            if (randomCri <= selectedCharacter.characterStatus.criRate) isCritical = true;
            
            if(isCritical)
              amountOfDamage = -((Mathf.FloorToInt (((selectedCharacter.characterStatus.attack * usingAbility.power * multiply) - target.characterStatus.defense)*1.5f)));
            else
              amountOfDamage = -(Mathf.FloorToInt (selectedCharacter.characterStatus.attack * usingAbility.power * multiply) - target.characterStatus.defense);
            
            if (amountOfDamage >= 0) amountOfDamage = -1;
            if (usingAbility.ability.gaugeUse > 0 && selectedCharacter.GetType () == typeof(PlayerCharacter))
              FinishingGaugeManager.GetInstance ().ChangeSliderValue (-usingAbility.ability.gaugeUse);
            else if (usingAbility.ability.gaugeUse > 0 && selectedCharacter.GetType () == typeof(AICharacter))
              selectedCharacter.GetComponent<AICharacter> ().rageGuage -= usingAbility.ability.gaugeUse;
            StartCoroutine (WaitDamageFloating (target));
          }
          else
          {
            amountOfDamage = Mathf.Clamp (Mathf.FloorToInt (usingAbility.power), Mathf.FloorToInt (usingAbility.power), target.characterStatus.maxHp - target.currentHP);
            StartCoroutine (WaitDamageFloating (target));
          }
          GetUsedAbility.ModifyAbility (selectedCharacter.ID, usingAbility.ability.ID, usingAbility.ability.coolDown);
        }
        break;
      }
    }
  }
  
  public int DamageResults()
  {
    return amountOfDamage;
  }
  
  private IEnumerator WaitDamageFloating(Character target)
  {
    if (isPlayerTurn) 
    {
      playerUI.transform.GetChild (3).gameObject.SetActive (false);
      menu.transform.GetChild (0).gameObject.SetActive (false);
      menu.transform.GetChild (1).gameObject.SetActive (false);
    }
    
    int i = 0;
    selectedCharacter.isActioning = true;
    //CameraManager.GetInstance ().FocusCamera (selectedCharacter.transform.position, target.transform.position);
    
    if (target.GetType () == typeof(AICharacter))
    {
      if (target.characterStatus.basicStatus.ID < 3000)
      {
        if(target.GetComponent<AICharacter> ().rageGuage < 2)
          target.GetComponent<AICharacter> ().rageGuage += 1;
      } 
      else
      {
        if(target.GetComponent<AICharacter> ().rageGuage < 5)
          target.GetComponent<AICharacter> ().rageGuage += 1;
      }
    }
    Animator anim = selectedCharacter.transform.GetChild(0).GetComponent<Animator> ();
    Animator targetAnim = target.transform.GetChild(0).GetComponent<Animator> ();
    
    while (i < usingAbility.hitAmount) 
    {
      if (usingAbility.ability.abilityType == 3)
        anim.Play ("FinalAttacking");
      else
        anim.Play  ("Attacking");
      
      do 
      {
        showingResultOfAttack.UpdateStatus (selectedCharacter, target, Mathf.Abs(amountOfDamage));
        yield return null;
      } while(!isAnimatorPlaying (targetAnim,"Damaged"));
      target.Standing ();
      i++;
    }
    
    selectedCharacter.skillText.transform.parent.gameObject.SetActive (false);
    //CameraManager.GetInstance ().ResetCamera ();
    
    if (target.currentHP <= 0) 
    {
      RemoveDead ();  
      if (target.GetType () == typeof(AICharacter))
        FinishingGaugeManager.GetInstance ().ChangeSliderValue (10);
      else
        FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
    }
    
    if (isPlayerTurn) 
    {
      playerUI.transform.GetChild (3).gameObject.SetActive (true);
      menu.transform.GetChild (0).gameObject.SetActive (true);
      menu.transform.GetChild (1).gameObject.SetActive (true);
    }
    selectedCharacter.played = true;
    selectedCharacter.isActioning = false;
    anim.SetInteger  ("animatorIndex", 0);
    NextTurn ();
    yield return 0;
  }
        
  private bool isAnimatorPlaying(Animator anim, string name)
  {
    return anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1 && anim.GetCurrentAnimatorStateInfo(0).IsName(name);
  }

  public void NextTurn()
  {
    RemoveMapHighlight ();
    RemoveTargetInRangeHighlight ();
    
    foreach (Renderer a in selectedCharacter.transform.GetChild(0).GetComponent<CharacterModelManager>().materials) 
    {
      a.material.color = Color.gray;
    }
    
    if(mapObjective == 1)
    {
      if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() > 0 && character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() > 0 ) StartCoroutine(WaitEndTurn ());
      else if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() <= 0) StartCoroutine (ShowResults (true));
      else if(character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() <= 0) StartCoroutine (ShowResults (false));
    }
    else if(mapObjective == 2)
    {
      if (character.Where (x => objectivePos.Where (z => x.GetType() == typeof(PlayerCharacter) && x.gridPosition == z).Count () > 0).Count () > 0)
        StartCoroutine (ShowResults (true));
      else 
      {
        if (character.Where (x => x.GetType () == typeof(PlayerCharacter)).Count () > 0)
          StartCoroutine (WaitEndTurn ());
        else if (character.Where (x => x.GetType () == typeof(PlayerCharacter)).Count () <= 0)
          StartCoroutine (ShowResults (false));
      }
    }
    else if(mapObjective == 3)
    {
      if(character.Where(x=>x.GetType() == typeof(AICharacter)).Count() > 0 && character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() > 0 ) StartCoroutine(WaitEndTurn ());
      else if(character.Where(x=>x.GetType() == typeof(AICharacter) && x.characterStatus.basicStatus.ID > 3000).Count() <= 0) StartCoroutine (ShowResults (true));
      else if(character.Where(x=>x.GetType() == typeof(PlayerCharacter)).Count() <= 0) StartCoroutine (ShowResults (false));
    }
  }

  private IEnumerator WaitEndTurn()
  {
    CameraManager.GetInstance ().MoveCameraToTarget (selectedCharacter.transform);
    RemoveMapHighlight ();
    if (chaSelector != null) 
    {
      Destroy (chaSelector);
      chaSelector = null;
    }
    
    yield return new WaitForSeconds (0.75f);
    
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

      if (currentCharacterIndex >= character.Count - 1) 
      {
        foreach (Character c in character) 
        {
          c.played = false;
        }
        isChangingTurn = true;
        isPlayerTurn = !isPlayerTurn;
        yield return StartCoroutine (ChangingTurn ());
        currentCharacterIndex = 0;
        
        if(isTouch)
          HitButton (false);
        oldGridPosition = Vector3.zero;
        oldPosition = Vector3.zero;
        oldCharacterNo = -1;
      } 
      else
      { 
        currentCharacterIndex ++;
        isChangingTurn = true;
        isPlayerTurn = !isPlayerTurn;
        yield return StartCoroutine (ChangingTurn ());
      }
    }
    
    if (character [currentCharacterIndex] != null && character [currentCharacterIndex].currentHP > 0 && character[currentCharacterIndex].played == false)
    {
      SelectedCharacter (character [currentCharacterIndex]);
      if (character [currentCharacterIndex].GetType() == typeof(AICharacter))
      {
        menu.transform.GetChild (0).gameObject.SetActive (false);
        menu.transform.GetChild (1).gameObject.SetActive (false);
        playerController.gameObject.SetActive (false);
        playerController.GetComponent<PlayerController> ().RemoveSelected ();
        character [currentCharacterIndex].TurnUpdate ();
        
        
        while (true) 
        {
          if (!isPlayerTurn) yield return new WaitForSeconds (1f);

          break;
        }
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

  private void ShowPlayerUI(bool showing, Character _selectedCharacter = null)
  {
    if (_selectedCharacter == null) _selectedCharacter = selectedCharacter;
    playerUI.transform.GetChild (0).gameObject.SetActive (showing);
    playerUI.transform.GetChild (0).GetChild (1).gameObject.SetActive (showing);
    playerUI.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + _selectedCharacter.name);
    playerUI.transform.GetChild (0).GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.name.ToString ();
    playerUI.transform.GetChild (0).GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.characterStatus.characterLevel.ToString ();
    playerUI.transform.GetChild (0).GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.currentHP.ToString ();
    playerUI.transform.GetChild (0).GetChild (0).GetChild (4).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.characterStatus.attack.ToString ();
    playerUI.transform.GetChild (0).GetChild (0).GetChild (5).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.characterStatus.defense.ToString ();
    playerUI.transform.GetChild (0).GetChild (0).GetChild (6).GetChild (0).GetComponent<Text> ().text = _selectedCharacter.characterStatus.criRate.ToString ();
    if (_selectedCharacter.GetType () == typeof(AICharacter)) 
    {
      RemoveMapHighlight ();
      playerUI.transform.GetChild (0).GetChild (1).gameObject.SetActive (false);
      HighlightTileAt (_selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, _selectedCharacter.characterStatus.movementPoint);
    }
  }

  public void HighlightTargetInRange(AbilityStatus usingAbility)
  {
    if (selectedCharacter.played) return;

    RemoveTargetInRangeHighlight ();

    List<Tile> highlighted = new List<Tile> ();

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)oldGridPosition.x][(int)oldGridPosition.z],selectedCharacter.characterStatus.movementPoint, character.Where (x => x.gridPosition != oldGridPosition && x.ordering != selectedCharacter.ordering).Select (x => x.gridPosition).ToArray ()))
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
          GameObject inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.52f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
          inRange.GetComponent<Renderer> ().material.color = Color.yellow;
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
          GameObject inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.52f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
          inRange.GetComponent<Renderer> ().material.color = Color.yellow;
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
        addResult.givenExp += Mathf.RoundToInt((character [i].GetComponent<AICharacter> ().aiInfo.givenExp * character [i].characterStatus.characterLevel)/1.5f);
        addResult.givenGold += Mathf.RoundToInt((character [i].GetComponent<AICharacter> ().aiInfo.givenExp * character [i].characterStatus.characterLevel)/1.75f);
          
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
      
    if (addResult.droppedItem.Count > 0) 
    {
      for (int i = 0; i < addResult.droppedItem.Count; i++) 
      {
        if (TemporaryData.GetInstance ().result.droppedItem.Count > 0) 
        {
          if (TemporaryData.GetInstance ().result.droppedItem.Where (x => x.itemStatus.ID == addResult.droppedItem [i].itemStatus.ID).Count () > 0)
            TemporaryData.GetInstance ().result.droppedItem.Where (x => x.itemStatus.ID == addResult.droppedItem [i].itemStatus.ID).First ().amount += 1;
          else
            TemporaryData.GetInstance ().result.droppedItem.Add (addResult.droppedItem [i]);
        } 
        else
          TemporaryData.GetInstance ().result.droppedItem.Add (addResult.droppedItem [i]);
      }
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
    isPause = true;
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
    
    if (isWin) 
    {
      if (TemporaryData.GetInstance ().playerData.passedMap.Where (x => x == PlayerPrefs.GetInt (Const.MapNo)).Count () <= 0)
      {
        TemporaryData.GetInstance ().playerData.passedMap.Add (PlayerPrefs.GetInt (Const.MapNo));
      }
      results.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ().text = "You Win";
    }
    else
    {
      results.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ().text = "You Lose";
    }
    results.SetActive (true);
      
    if(party.Count > 0 && playerCharacterID.Where(x=>x == party[0].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [0], TemporaryData.GetInstance ().result.givenExp, showResultCharacters[0].GetChild(0)));
    if(party.Count > 1 && playerCharacterID.Where(x=>x == party[1].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [1], TemporaryData.GetInstance ().result.givenExp, showResultCharacters[1].GetChild(0)));
    if(party.Count > 2 && playerCharacterID.Where(x=>x == party[2].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [2], TemporaryData.GetInstance ().result.givenExp,showResultCharacters[2].GetChild(0)));
    if(party.Count > 3 && playerCharacterID.Where(x=>x == party[3].basicStatus.ID).Count()>0)StartCoroutine (SystemManager.LevelUpSystem(party [3], TemporaryData.GetInstance ().result.givenExp,showResultCharacters[3].GetChild(0)));
    
    while (true) 
    {
      if (Input.GetMouseButtonDown (0) && SystemManager.isFinishLevelUp /*&& Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
      {
        AddingResultItem ();
        TemporaryData.GetInstance ().result = new Result ();
        SceneManager.LoadScene ("LoadScene");
        SystemManager.isFinishLevelUp = false;
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
      addedItem = new Item ();
      addedItem.item = TemporaryData.GetInstance ().result.droppedItem [0].itemStatus;
      addedItem.equiped = false;
      addedItem.ordering = TemporaryData.GetInstance ().playerData.inventory.Count;
      TemporaryData.GetInstance ().playerData.inventory.Add (addedItem);
      
      GameObject itemObj = Instantiate (Resources.Load<GameObject> ("Item/ItemGet"));
      itemObj.transform.SetParent (results.transform.GetChild (0).GetChild (4).GetChild(0));
      itemObj.transform.localScale = Vector3.one;
      
      if(Resources.Load<Sprite> ("Item/Texture/" + addedItem.item.name) != null)
        itemObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + addedItem.item.name);
      else
        itemObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + addedItem.item.itemType1);
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
  
  public void FloatingTextController(int value, Transform location, Vector3 Position)
  {
    FloatingText popUpText = Resources.Load<FloatingText> ("PopupTextParent");
    FloatingText instance = Instantiate (popUpText);

    instance.transform.SetParent (location);
    instance.transform.localPosition = Position;
    instance.transform.localScale = Vector3.one;
    instance.GetComponent<RectTransform> ().sizeDelta = new Vector2 (160, 60);
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
