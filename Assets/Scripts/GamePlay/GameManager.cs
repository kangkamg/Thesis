using System.Collections;
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
  public List<int> enemyID = new List<int>();

  public Character selectedCharacter;
  public Character previousSelectedCharacter;
  public GameObject chaSelector;

  public GameObject playerUI;
  public GameObject results;
  public ShowingResultOfAttack showingResultOfAttack;

  List<GameObject> targetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  List<Text> allHpText = new List<Text>();

  public Vector3 originPos;
  public Vector3 originGrid;
  public int oldCharacterNo = -1;
  public bool isPlayerTurn;

  public bool hitButton = false;

  private int usingWhat = -1;
  public AbilityStatus usingAbility;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (PlayerPrefs.GetInt(Const.MapNo,1));
    GenerateCharacter ();

    isPlayerTurn = true;
    results.SetActive (false);
  }

  private void Start()
  {
    selectedCharacter = character [0];
    previousSelectedCharacter = selectedCharacter;
    SelectedCharacter ();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0) && !hitButton && isPlayerTurn/*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
    {
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition/*Input.GetTouch(0).position*/);

      RaycastHit hit;
      if (Physics.Raycast (ray, out hit, 1000f)) 
      {
        if (hit.transform.tag == "Player") 
        {
          if (!selectedCharacter.played)
            currentCharacterIndex = selectedCharacter.ordering;
          
          if (oldCharacterNo < 0) 
          {
            selectedCharacter = hit.transform.GetComponent<PlayerCharacter> ();
            SelectedCharacter ();
          }
          else
          {
            if (oldCharacterNo != hit.transform.GetComponent<PlayerCharacter>().ordering) 
            {
              if (usingWhat != 1)
              {
                RemoveMapHighlight ();
                previousSelectedCharacter.transform.position = originPos;
                previousSelectedCharacter.gridPosition = originGrid;
                selectedCharacter = hit.transform.GetComponent<PlayerCharacter> ();
                SelectedCharacter ();
              } 
              else 
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
              }
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
              else if (c.gridPosition == hit.transform.GetComponent<Tile> ().gridPosition && c.tag == "Player" && usingWhat == 1)
              {
                AttackWithCurrentCharacter (hit.transform.GetComponent<Tile> ());
                break;
              }
              else if (c.gridPosition != hit.transform.GetComponent<Tile> ().gridPosition)
              {
                foreach (GameObject m in highlightTileMovement)
                {
                  if (m.transform.position.x == hit.transform.position.x && m.transform.position.z == hit.transform.position.z)
                  {
                    RemoveAttackHighLightOnly ();
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

    if (character.Where (x => x.GetType () == typeof(AICharacter)).Count() <= 0 && !results.activeSelf) 
    {
      ShowResults ();
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
    CameraManager.GetInstance ().MoveCameraToTarget (new Vector3(_mapSize,0,0),true);
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

      GameObject playerObj = Instantiate (PrefabHolder.GetInstance ().Player, new Vector3 (startPlayer [i].transform.position.x, 1.5f, startPlayer [i].transform.position.z), Quaternion.Euler (new Vector3 (0, 90, 0)));
      PlayerCharacter player = playerObj.GetComponent<PlayerCharacter> ();
      player.SetStatus(TemporaryData.GetInstance ().playerData.characters.Where (x => x.partyOrdering == i).First());
      player.gridPosition = startPlayer[i].gridPosition;
      character.Add (player);
      player.ordering = character.Count - 1;
      CharacterInMapInfo (player);
    }

    foreach (Tile a in startEnemy) 
    {
      GameObject aiPlayerObj = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3 (a.transform.position.x, 1.5f, a.transform.position.z), Quaternion.Euler (new Vector3 (0, -90, 0)));
      AICharacter aiPlayer = aiPlayerObj.GetComponent<AICharacter> ();
      aiPlayer.SetStatus ("Crodile");
      aiPlayer.characterStatus.normalAttack.ability = aiPlayer.characterStatus.basicStatus.normalAttack;
      aiPlayer.characterStatus.normalAttack.level = 1;
      if (aiPlayer.characterStatus.specialAttack.ability != null)
      {
        aiPlayer.characterStatus.specialAttack.ability = aiPlayer.characterStatus.basicStatus.specialAttack;
        aiPlayer.characterStatus.specialAttack.level = 1;
      }
      aiPlayer.gridPosition = a.gridPosition;
      character.Add (aiPlayer);
      aiPlayer.ordering = character.Count - 1;
      CharacterInMapInfo (aiPlayer);
    }
  }

  public void SetUseAble()
  {
    Transform showing = playerUI.transform.GetChild (0).GetChild (1).transform;
    for (int i = 0; i < showing.childCount; i++)
    {
      Destroy (showing.GetChild (i).gameObject);
    }
    List<Transform> useAble = new List<Transform>();

    if (selectedCharacter.GetType () == typeof(PlayerCharacter)) 
    {
      for (int i = 0; i < 2; i++) 
      {
        GameObject abilityObj = Instantiate (Resources.Load<GameObject> ("GamePlay/Using"));
        abilityObj.transform.SetParent (showing);
        abilityObj.transform.localScale = Vector3.one;
        abilityObj.name = "useAble" + i;
        abilityObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UseAble/NoAttack");
        abilityObj.GetComponent<Button> ().interactable = false;
        useAble.Add (abilityObj.transform);
      }

      if (selectedCharacter.characterStatus.specialAttack.ability != null) 
      {
        useAble [0].GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UseAble/" + selectedCharacter.characterStatus.specialAttack.ability.abilityName);
        useAble [0].GetComponent<Button> ().interactable = true;
        useAble[0].GetComponent<Button>().onClick.AddListener(()=>SetUsing(0));
      }

      if (selectedCharacter.characterStatus.equipItem.Where(x=>x.item.itemType1 == "Items").Count()>0) 
      {
        useAble [1].GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UseAble/" + selectedCharacter.characterStatus.equipItem.Where(x=>x.item.itemType1 == "Items").First().item.name);
        useAble [1].GetComponent<Button> ().interactable = true;
        useAble[1].GetComponent<Button>().onClick.AddListener(()=>SetUsing(1));
      }
    }
  }

  public void SetUsing(int selected)
  {
    GameObject highlighted = new GameObject();
    if (usingWhat != selected) 
    {
      usingWhat = selected;
      if (usingWhat == 0) 
      {
        usingAbility = selectedCharacter.characterStatus.specialAttack;
        highlighted = PrefabHolder.GetInstance ().AttackTile;
      } 

      else if (usingWhat == 1) 
      {
        ItemStatus usingItem = selectedCharacter.characterStatus.equipItem.Where (x => x.item.itemType1 == "Items").First ().item;
        usingAbility = new AbilityStatus ();
        usingAbility.ability = GetDataFromSql.itemAbilityStatus (usingItem.ID);
        usingAbility.ability.power = usingItem.increaseHP;
        usingAbility.level = 1;
        usingAbility.ability.hitAmount = 1;
        highlighted = PrefabHolder.GetInstance ().HealingTile;
      }
    }
    else
    {
      usingWhat = -1;
      usingAbility = selectedCharacter.characterStatus.normalAttack;
      highlighted = PrefabHolder.GetInstance ().AttackTile;
    }
      
    RemoveMapHighlight ();
    HighlightTileAt (originGrid, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
    HighlightTileAt (selectedCharacter.gridPosition, highlighted, usingAbility.range, usingAbility.ability.rangeType);
    HighlightTargetInRange (usingAbility);
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

  public Tile CheckingMovementToAttackTarget(Transform target)
  {
    List<Tile> targetTile = new List<Tile> ();

    if(usingAbility.ability.rangeType == "cross")
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
    else if (usingAbility.ability.rangeType == "both") 
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

  public void SelectedCharacter()
  {
    RemoveMapHighlight ();
    usingWhat = -1;
    usingAbility = selectedCharacter.characterStatus.normalAttack;
    previousSelectedCharacter = selectedCharacter;
    oldCharacterNo = selectedCharacter.ordering;
    originGrid = selectedCharacter.gridPosition;
    originPos = selectedCharacter.transform.position;
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().AttackTile, usingAbility.range, usingAbility.ability.rangeType);
    HighlightTargetInRange (usingAbility);
    Destroy (chaSelector);
    chaSelector = null;
    chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
    chaSelector.transform.SetParent (selectedCharacter.transform);

    if (selectedCharacter.GetType () == typeof(PlayerCharacter)) 
    {
      playerUI.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + selectedCharacter.name);
      playerUI.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ().text = "Name : " + selectedCharacter.name.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (2).GetComponent<Text> ().text = "HP : " + selectedCharacter.currentHP.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (3).GetComponent<Text> ().text = "Attack : " + selectedCharacter.characterStatus.attack.ToString ();
      playerUI.transform.GetChild (0).GetChild (0).GetChild (4).GetComponent<Text> ().text = "Defense : " + selectedCharacter.characterStatus.defense.ToString ();
    }

    SetUseAble ();
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
        foreach (Tile t in TilePathFinder.FindPathPlus(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z], desTile, occupied.ToArray())) 
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
          selectedCharacter.GetComponent<Animator> ().Play ("Test");
          foreach (Text text in allHpText) 
          {
            text.gameObject.SetActive (false);
          }
          if (usingAbility.ability.abilityType != "Support")
          {
            int amountOfDamage = Mathf.FloorToInt (selectedCharacter.characterStatus.attack * usingAbility.power) - target.characterStatus.defense;
            if (amountOfDamage <= 0) amountOfDamage = 0;
            showingResultOfAttack.UpdateStatus (selectedCharacter, target, amountOfDamage);
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
      target.currentHP += amountOfResults;
      if(amountOfResults <= 0) FloatingTextController (amountOfResults*-1, target.transform);
      else FloatingTextController (amountOfResults, target.transform);
      i++;
      yield return 0;
    }
    selectedCharacter.played = true;
    if (target.currentHP <= 0) 
    {
      RemoveDead ();
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
      Animator anim = selectedCharacter.GetComponent<Animator> ();

      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo (0).length * anim.GetCurrentAnimatorStateInfo (0).speed);

      break;
    }
      
    CameraManager.GetInstance ().ResetCamera ();
    GameManager.GetInstance ().playerUI.transform.GetChild (0).gameObject.SetActive (true);
    showingResultOfAttack.gameObject.SetActive (false);
    foreach (Text text in allHpText) 
    {
      text.gameObject.SetActive (true);
    }

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
      selectedCharacter = character [currentCharacterIndex];
      SelectedCharacter ();
      character [currentCharacterIndex].TurnUpdate ();
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
    character [currentCharacterIndex].played = true;
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

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)originGrid.x][(int)originGrid.z],selectedCharacter.characterStatus.movementPoint, character.Where (x => x.gridPosition != selectedCharacter.gridPosition && x.ordering != selectedCharacter.ordering).Select (x => x.gridPosition).ToArray ()))
    {
      if (usingAbility.ability.rangeType == "both")
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
      else if (usingAbility.ability.rangeType == "plus")
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
        if (usingAbility.ability.abilityType != "Support") 
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
        addResult = GetDataFromSql.GetResult (character [i].characterStatus.basicStatus.ID);
        Destroy (character [i].gameObject);
        character.Remove (character[i]);
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

  public void ShowResults()
  {
    results.SetActive (true);

    results.transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenExp.ToString();
    results.transform.GetChild (0).GetChild (2).GetChild (1).GetComponent<Text>().text = TemporaryData.GetInstance ().result.givenGold.ToString();

    List<CharacterStatus> party = TemporaryData.GetInstance ().playerData.characters.Where (x => x.isInParty).ToList ();

    for (int i = 0; i < party.Count; i++) 
    {
      GameObject characterInParty = Instantiate (Resources.Load<GameObject> ("ResultCharacter"));
      characterInParty.transform.SetParent (results.transform.GetChild (0).GetChild (3));
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
      TemporaryData.GetInstance ().playerData.inventory.Add (addedItem);
    }
    TemporaryData.GetInstance ().playerData.gold += TemporaryData.GetInstance ().result.givenGold;

    TemporaryData.GetInstance ().result = new Result ();

    if (Input.GetMouseButtonDown (0))
    {
      
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

  public void CharacterInMapInfo(Character target)
  {
    Vector2 screenPosition = Camera.main.WorldToScreenPoint (target.transform.position);
    GameObject hpText = Instantiate (Resources.Load<GameObject> ("GamePlay/HpText"));
    allHpText.Add (hpText.GetComponent<Text> ());
    hpText.transform.SetParent(GameObject.Find ("Canvas").transform,false);
    hpText.transform.position = new Vector2(screenPosition.x - 9f,screenPosition.y - 15f);
    hpText.GetComponent<UpdateCharacterInfo> ().UpdateInfo (target);
  }

  public void Auto()
  {
    if(character.Where(x=>x.GetType() == typeof (PlayerCharacter) && x.isAI).Count() > 0) 
    {
      foreach (Character c in character)
      {
        if(c.GetType() == typeof (PlayerCharacter))
        {
          c.isAI = false;
        }
      }
    }

    else
    {
      foreach (Character c in character)
      {
        if(c.GetType() == typeof (PlayerCharacter))
        {
          c.isAI = true;
          ShowPlayerUI (false);
        }
      }

      RemoveMapHighlight ();

      if (previousSelectedCharacter != null) 
      {
        previousSelectedCharacter.transform.position = originPos;
        previousSelectedCharacter.gridPosition = originGrid;
      }
        
      character.Where(x=>x.GetType() == typeof (PlayerCharacter) && x.played == false).First().TurnUpdate ();
    }
    
  }
}
