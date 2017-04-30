using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Character : MonoBehaviour
{
  public List<Vector3>positionQueue = new List<Vector3>();
  public Vector3 lookRotation = new Vector3 ();
  public bool isRotate = false;
 
  public float moveSpeed = 0.25f;

  public Vector3 gridPosition = Vector3.zero;
  
  public int currentHP;
  public int ordering;
  public int ID;

  public bool played = false;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();
  
  public bool isActioning = false;

  public virtual void MoveToDesTile()
  {

  }
  
  public virtual void TurnUpdate()
  {
   
  }
  
  public void Update()
  {
    if (isRotate)
    {
      transform.GetChild(0).rotation = Quaternion.LookRotation (Vector3.RotateTowards (transform.GetChild(0).forward, lookRotation - transform.position, 360f, 0.0f));
      if (transform.GetChild (0).rotation ==
        Quaternion.LookRotation (Vector3.RotateTowards (transform.GetChild (0).forward, lookRotation - transform.position, 360f, 0.0f))) 
      {
        isRotate = false;
      } 
    }
  }
  
  public virtual void RotateTo(Vector3 rotation)
  {
    lookRotation = rotation; 
    isRotate = true;
  }
  
  public virtual void SetStatus(int ID)
  {
    characterStatus.basicStatus = GetDataFromSql.GetCharacter (ID);

    currentHP = characterStatus.maxHp;
    this.name = characterStatus.basicStatus.characterName;
  }

  public virtual void SetStatus(CharacterStatus status)
  {
    characterStatus = status;

    currentHP = characterStatus.maxHp;
    this.name = characterStatus.basicStatus.characterName;
  }
  
  public void AttackTarget()
  {
    Animator targetAnim = target.transform.GetChild(0).GetComponent<Animator> ();
    targetAnim.SetInteger ("animatorIndex", 2);
    target.currentHP += GameManager.GetInstance().DamageResults();
    if (GameManager.GetInstance ().DamageResults () <= 0)
    {
      GameManager.GetInstance ().FloatingTextController (GameManager.GetInstance ().DamageResults () * -1, target.transform);
      GameManager.GetInstance().FloatingTextController (GameManager.GetInstance().DamageResults()*-1, GameManager.GetInstance().showingResultOfAttack.transform, new Vector2(110,6.9f));
    }
    else
    {
      GameManager.GetInstance().FloatingTextController (GameManager.GetInstance().DamageResults(), GameManager.GetInstance().showingResultOfAttack.transform, new Vector2(110,6.9f));
      GameManager.GetInstance().FloatingTextController  (GameManager.GetInstance().DamageResults(), target.transform);
    }
    if (target.GetType () == typeof(AICharacter)) FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
    
    else FinishingGaugeManager.GetInstance ().ChangeSliderValue (2.5f);
  }
  
  public void Standing()
  {
    transform.GetChild (0).GetComponent<Animator> ().SetInteger ("animatorIndex", 0);
    transform.GetChild (0).GetComponent<Animator> ().Play ("Standing");
  }
  
  public virtual bool CheckingAbilityCanPerform(AbilityStatus checking, out List<Tile> returnTiles)
  {
    List<Tile> targetTilesInRange = new List<Tile> ();
    
    foreach (Tile t in TileHighLight.FindHighLight(GameManager.GetInstance().map[(int)gridPosition.x][(int)gridPosition.z],characterStatus.movementPoint, GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ()))
    {
      if (checking.ability.rangeType == 2)
      {
        foreach (Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false))
        {
          if(!targetTilesInRange.Contains(a))
            targetTilesInRange.Add (a);
        }
        foreach (Tile b in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true)) 
        {
          if(!targetTilesInRange.Contains(b))
            targetTilesInRange.Add (b);
        }
      } 
      else if (checking.ability.rangeType == 0)
      {
        foreach(Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false))
        {
          if(!targetTilesInRange.Contains(a))
            targetTilesInRange.Add (a);
        }
      }
      else
      {
        foreach(Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true))
        {
          if(!targetTilesInRange.Contains(a))
            targetTilesInRange.Add (a);
        }
      }
    }
    
    if (targetTilesInRange.Count > 0)
    {
      returnTiles = targetTilesInRange;
      return true;
    } 
    else 
    {
      returnTiles = null;
      return false;
    }
  }
}
