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

  public Character selectedCharacter;
  public Character previousSelectedCharacter;
  public GameObject chaSelector;

  public GameObject playerUI;

  List<GameObject> targetInRange = new List<GameObject> ();
  List<GameObject> highlightTileMovement = new List<GameObject> ();
  List<GameObject> highlightTileAttack = new List<GameObject> ();

  public Vector3 originPos;
  public Vector3 originGrid;
  public int oldCharacterNo = -1;
  public bool isPlayerTurn;

  public bool hitButton = false;

  private void Awake()
  {
    instance = GetComponent<GameManager> ();

    GenerateMap (PlayerPrefs.GetInt(Const.MapNo,1));
    GenerateCharacter ();

    isPlayerTurn = true;
  }

  private void Start()
  {
    selectedCharacter = character [0];
    previousSelectedCharacter = selectedCharacter;
    oldCharacterNo = selectedCharacter.ordering;
    originGrid = selectedCharacter.gridPosition;
    originPos = selectedCharacter.transform.position;
    chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
    chaSelector.transform.SetParent (selectedCharacter.transform);
    RemoveMapHighlight ();
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().MovementTile, selectedCharacter.characterStatus.movementPoint);
    HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance().AttackTile, selectedCharacter.characterStatus.normalAttack.range,selectedCharacter.characterStatus.normalAttack.ability.rangeType);
    HighlightTargetInRange ();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0) && !hitButton/*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
    {
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition/*Input.GetTouch(0).position*/);

      RaycastHit hit;
      if (Physics.Raycast (ray, out hit, 1000f)) 
      {
        if (hit.transform.tag == "Player") 
        {
          selectedCharacter = hit.transform.GetComponent<PlayerCharacter> ();

          if (!selectedCharacter.played)
            currentCharacterIndex = selectedCharacter.ordering;

          if (oldCharacterNo < 0) 
          {
            previousSelectedCharacter = selectedCharacter;
            oldCharacterNo = selectedCharacter.ordering;
            originGrid = selectedCharacter.gridPosition;
            originPos = selectedCharacter.transform.position;
            RemoveMapHighlight ();
            HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
            HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().AttackTile, selectedCharacter.characterStatus.normalAttack.range, selectedCharacter.characterStatus.normalAttack.ability.rangeType);
            HighlightTargetInRange ();
            Destroy (chaSelector);
            chaSelector = null;
            chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
            chaSelector.transform.SetParent (selectedCharacter.transform);
          }
          else
          {
            if (oldCharacterNo != selectedCharacter.ordering) 
            {
              RemoveMapHighlight ();
              previousSelectedCharacter.transform.position = originPos;
              previousSelectedCharacter.gridPosition = originGrid;
              previousSelectedCharacter = selectedCharacter;
              oldCharacterNo = selectedCharacter.ordering;
              originGrid = selectedCharacter.gridPosition;
              originPos = selectedCharacter.transform.position;
              HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
              HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().AttackTile, selectedCharacter.characterStatus.normalAttack.range, selectedCharacter.characterStatus.normalAttack.ability.rangeType);
              HighlightTargetInRange ();
              Destroy (chaSelector);
              chaSelector = null;
              chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
              chaSelector.transform.SetParent (selectedCharacter.transform);
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
    CameraManager.GetInstance ().MoveCameraToTarget (new Vector3(_mapSize/2,0,-_mapSize/2),true);
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
      player.characterStatus = TemporaryData.GetInstance ().playerData.characters.Where (x => x.partyOrdering == i).First();
      player.name = player.characterStatus.basicStatus.characterName;
      player.gridPosition = startPlayer[i].gridPosition;
      character.Add (player);
      player.ordering = character.Count - 1;
    }

    foreach (Tile a in startEnemy) 
    {
      GameObject aiPlayerObj = Instantiate (PrefabHolder.GetInstance ().AIPlayer, new Vector3 (a.transform.position.x, 2.175f, a.transform.position.z), Quaternion.Euler (new Vector3 (0, -90, 0)));
      AICharacter aiPlayer = aiPlayerObj.GetComponent<AICharacter> ();
      aiPlayer.gridPosition = a.gridPosition;
      character.Add (aiPlayer);
      aiPlayer.ordering = character.Count - 1;
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
      map [(int)originLocation.x] [(int)originLocation.z].canMove = true;
      highlightedTiles = TileHighLight.FindHighLight (map [(int)originLocation.x] [(int)originLocation.z], distance, character.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());

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

    if(selectedCharacter.characterStatus.normalAttack.ability.rangeType == "cross")
    {
      for(int i = 0;i<highlightTileMovement.Count;i++)
      {
        List<Tile> canAttacking = TileHighLight.FindHighLight (highlightTileMovement[i].transform.parent.GetComponent<Tile>(), selectedCharacter.characterStatus.normalAttack.ability.range, true, true);
        if (canAttacking.Where (x => x.transform.position.x == target.position.x && x.transform.position.z == target.position.z).Count () > 0)
        {
          targetTile.Add(highlightTileMovement [i].transform.parent.GetComponent<Tile>());
        }
      }
    }
    else if (selectedCharacter.characterStatus.normalAttack.ability.rangeType == "both") 
    {
      for (int i = 0; i < highlightTileMovement.Count; i++)
      {
        List<Tile> canAttacking = new List<Tile> ();
        foreach (Tile t in TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), selectedCharacter.characterStatus.normalAttack.ability.range, true, true)) 
        {
          canAttacking.Add (t);
        }
        foreach (Tile t in TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), selectedCharacter.characterStatus.normalAttack.ability.range, true, false)) 
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
        List<Tile> canAttacking = TileHighLight.FindHighLight (highlightTileMovement [i].transform.parent.GetComponent<Tile> (), selectedCharacter.characterStatus.normalAttack.ability.range, true, false);
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
          for (int i = 0; i < selectedCharacter.characterStatus.normalAttack.hitAmount; i++) 
          {
            int amountOfDamage = Mathf.Max (0, (int)Mathf.Floor (selectedCharacter.characterStatus.attack * selectedCharacter.characterStatus.normalAttack.power)) - target.characterStatus.defense;
            if (amountOfDamage <= 0) amountOfDamage = 0;
            target.currentHP -= amountOfDamage;
            FloatingTextController (amountOfDamage, target.transform);
            StartCoroutine (WaitDamageFloating ());
          }
          selectedCharacter.played = true;
          NextTurn ();
        }
        break;
      }
    }
  }

  private IEnumerator WaitDamageFloating()
  {
    Animator anim = GameObject.Find ("PopupTextParent(Clone)").GetComponentInChildren<Animator> ();

    yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed);
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

      yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed);

      break;
    }

    while (character [currentCharacterIndex].played) 
    {
      int playAbleAmount = 0;
      int playAbleCharacter = 0;
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
          currentCharacterIndex = playAbleCharacter;
        else
          currentCharacterIndex++;
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
          RemoveDead ();
          currentCharacterIndex = 0;
        } 
        else
        {
          currentCharacterIndex++;
        }
      }

      ShowPlayerUI (isPlayerTurn);
    }
    if (character [currentCharacterIndex] != null && character [currentCharacterIndex].currentHP > 0)
    {
      selectedCharacter = character [currentCharacterIndex];
      previousSelectedCharacter = selectedCharacter;
      oldCharacterNo = selectedCharacter.ordering;
      originGrid = selectedCharacter.gridPosition;
      originPos = selectedCharacter.transform.position;
      RemoveMapHighlight ();
      HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().MovementTile, selectedCharacter.characterStatus.movementPoint);
      HighlightTileAt (selectedCharacter.gridPosition, PrefabHolder.GetInstance ().AttackTile, selectedCharacter.characterStatus.normalAttack.range, selectedCharacter.characterStatus.normalAttack.ability.rangeType);
      HighlightTargetInRange ();
      Destroy (chaSelector);
      chaSelector = null;
      chaSelector = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (selectedCharacter.transform.position.x, 0.51f, selectedCharacter.transform.position.z), Quaternion.Euler (90, 0, 0));
      chaSelector.transform.SetParent (selectedCharacter.transform);
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

  public void HighlightTargetInRange()
  {
    if (selectedCharacter.played) return;

    foreach (GameObject obj in targetInRange) 
    {
      Destroy (obj);
    }
    targetInRange.Clear ();

    List<Tile> highlighted = new List<Tile> ();

    foreach (Tile t in TileHighLight.FindHighLight(map[(int)selectedCharacter.gridPosition.x][(int)selectedCharacter.gridPosition.z],selectedCharacter.characterStatus.movementPoint, character.Where (x => x.gridPosition != selectedCharacter.gridPosition).Select (x => x.gridPosition).ToArray ()))
    {
      if (selectedCharacter.characterStatus.normalAttack.ability.rangeType == "both")
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, selectedCharacter.characterStatus.normalAttack.range, true))
        {
          highlighted.Add (a);
        }
        foreach (Tile b in TileHighLight.FindHighLight (t, selectedCharacter.characterStatus.normalAttack.range, true, true)) 
        {
          highlighted.Add (b);
        }
      } 
      else if (selectedCharacter.characterStatus.normalAttack.ability.rangeType == "plus")
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, selectedCharacter.characterStatus.normalAttack.range, true))
        {
          highlighted.Add (a);
        }
      }
      else
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, selectedCharacter.characterStatus.normalAttack.range, true, true))
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
        if (c != null && c.tag != selectedCharacter.tag) 
        {
          inRange = Instantiate (PrefabHolder.GetInstance ().Selected_TilePrefab, new Vector3 (c.transform.position.x, 0.51f, c.transform.position.z), Quaternion.Euler (90, 0, 0));
          inRange.GetComponent<Renderer> ().material.color = Color.red;
          inRange.transform.SetParent (c.transform);

          targetInRange.Add (inRange);
        }
      }
    }
  }

  public void RemoveDead()
  {
    for(int i = 0 ; i < character.Count ; i++) 
    {
      if (character[i].currentHP <= 0 || character[i] == null) 
      {
        character.Remove (character[i]);
        break;
      }
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

  public void Auto()
  {
    if (character.Where (x => x.GetType () == typeof(PlayerCharacter) && x.isAI).Count () <= 0)
    {
      foreach (Character c in character)
      {
        c.isAI = true;
        ShowPlayerUI (false);
      }
      RemoveMapHighlight ();

      if (previousSelectedCharacter != null) 
      {
        previousSelectedCharacter.transform.position = originPos;
        previousSelectedCharacter.gridPosition = originGrid;
      }

      if (currentCharacterIndex + 1 < character.Where(x=> x.GetType() == typeof(PlayerCharacter)).Count())
      {
        if (!character [0].played) 
        {
          currentCharacterIndex = 0;
        }
        else
        {
          currentCharacterIndex ++;
        } 
      } 
      else
      {
        foreach (Character c in character) 
        {
          c.played = false;
        }
        currentCharacterIndex = 0;
      }

      character [currentCharacterIndex].TurnUpdate ();
    }

    else
    {
      foreach (Character c in character.Where(x=>x.GetType() == typeof(PlayerCharacter)).ToList())
      {
        c.isAI = false;
      }
    }
  }
}
