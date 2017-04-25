using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Character : MonoBehaviour
{
  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 0.25f;

  public Vector3 gridPosition = Vector3.zero;
  
  public int currentHP;
  public int ordering;
  public int ID;

  public bool played = false;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();

  public virtual void MoveToDesTile()
  {

  }
  
  public virtual void TurnUpdate()
  {
   
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
