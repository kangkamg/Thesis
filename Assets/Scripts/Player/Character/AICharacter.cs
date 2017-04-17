using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AIInformation
{
  public List<string> droppedItem = new List<string>();
  public int givenExp;
  public int givenGold;
  public int effectiveAttack;
  public int element;
}

public class AICharacter : Character
{
  public AIInformation aiInfo;
  public int rageGuage;
  
  public override void SetStatus(int ID)
  {
    aiInfo = GetDataFromSql.GetAiInfomation (ID);
    base.SetStatus (ID);
  }
  
  public override void MoveToDesTile()
  {
    if (positionQueue.Count > 0)
    {
      if (Vector3.Distance (positionQueue [0], transform.position) > 0.1f)
      {
        transform.position = Vector3.MoveTowards (transform.position, positionQueue [0], moveSpeed*Time.deltaTime);
        if (Vector3.Distance (positionQueue [0], transform.position) < 0.1f) 
        {
          transform.position = positionQueue [0];
          positionQueue.RemoveAt (0);
          if (positionQueue.Count == 0)
          {
            if (target != null)
            {
              GameManager.GetInstance ().RemoveMapHighlight ();
              GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile, GameManager.GetInstance ().usingAbility.range, GameManager.GetInstance ().usingAbility.ability.rangeType);

              GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)target.gridPosition.x] [(int)target.gridPosition.z]);
              target = null;
            } 
            else
            {
              played = true;
              GameManager.GetInstance ().NextTurn ();
            }
          }
        }
      }
    }
  }

  public override void TurnUpdate()
  {
    if (currentHP > 0)
    {
      List<AbilityStatus> readyAbility = characterStatus.equipedAbility.Where (x => x.ability.gaugeUse <= rageGuage).ToList();
      List<Tile> targetTilesInRange = new List<Tile> ();
      
      readyAbility.Sort (delegate(AbilityStatus a, AbilityStatus b)
      {
          return a.power.CompareTo(b.power);
      }
      );
      
      readyAbility.Reverse ();
      
      foreach (AbilityStatus a in readyAbility)
      {
        if(CheckingAbilityCanPerform(a,out targetTilesInRange))
        {
          GameManager.GetInstance ().usingAbility = a;
          break;
        }
      }
      
      List<Tile> movementToAttackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint, GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ());
      List<Tile> movementTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint + 99999);
    
      if (targetTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
      {
        var opponentsInRange = targetTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
        Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], GameManager.GetInstance ().map [(int)x.gridPosition.x] [(int)x.gridPosition.z]).Count () : 1000).First ();

        GameManager.GetInstance ().RemoveMapHighlight ();
        GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().MovementTile, characterStatus.movementPoint);

        if (GameManager.GetInstance ().CheckingMovementToAttackTarget (opponent.transform).gridPosition != gridPosition)
        {
          GameManager.GetInstance ().MoveCurrentCharacter (GameManager.GetInstance ().CheckingMovementToAttackTarget (opponent.transform));
        }
        else 
        {
          GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile, GameManager.GetInstance ().usingAbility.range, GameManager.GetInstance ().usingAbility.ability.rangeType);
          GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.z]);
        }
        
        target = opponent;
      } 
      else if (movementTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
      {
        if (movementToAttackTilesInRange.Count > 0) 
        {
          var opponentsInRange = movementTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
          Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], GameManager.GetInstance ().map [(int)x.gridPosition.x] [(int)x.gridPosition.z]).Count () : 1000).First ();

          GameManager.GetInstance ().RemoveMapHighlight ();
          GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().MovementTile, characterStatus.movementPoint);

          List<Tile> path = TilePathFinder.FindPath (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], GameManager.GetInstance ().map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.z]);

          if (path == null)
          {
            played = true;
            GameManager.GetInstance ().NextTurn ();
          }
          else 
          {
            if (path.Count () > 1) 
            {
              List<Tile> actualMovement = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint, GameManager.GetInstance ().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ());
              path.Reverse ();
              if (path.Where (x => actualMovement.Contains (x)).Count () > 0) 
              {
                GameManager.GetInstance ().MoveCurrentCharacter (path.Where (x => actualMovement.Contains (x)).First ());
              } 
              else 
              {
                played = true;
                GameManager.GetInstance ().NextTurn ();
              }
            } 
            else 
            {
              GameManager.GetInstance ().MoveCurrentCharacter (path [0]);
            }
          }
        } 
        else 
        {
          played = true;
          GameManager.GetInstance ().NextTurn ();
        }
      }
    }
  }
}
