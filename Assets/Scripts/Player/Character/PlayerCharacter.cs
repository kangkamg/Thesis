using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCharacter : Character 
{
  public override void MoveToDesTile()
  {
    if (positionQueue.Count > 0)
    {
      if (Vector3.Distance (positionQueue [0], transform.position) > 0.1f)
      {
        transform.position = Vector3.MoveTowards (transform.position, positionQueue [0], moveSpeed*Time.deltaTime);
        transform.GetChild(0).rotation = Quaternion.LookRotation (Vector3.RotateTowards (transform.GetChild(0).forward, positionQueue [0] - transform.position, 360f, 0.0f));
        if (Vector3.Distance (positionQueue [0], transform.position) < 0.1f) 
        {
          transform.position = positionQueue [0];
          positionQueue.RemoveAt (0);
          if (positionQueue.Count == 0)
          {
            isActioning = false;
            transform.GetChild (0).GetComponent<Animator> ().SetInteger ("animatorIndex", 0);
            GameManager.GetInstance().playerUI.transform.GetChild (0).gameObject.SetActive (true);
            GameManager.GetInstance().menu.transform.GetChild (0).gameObject.SetActive (true);
            GameManager.GetInstance().menu.transform.GetChild (1).gameObject.SetActive (true);
            if(!GameManager.GetInstance().isTouch) GameManager.GetInstance().playerUI.transform.GetChild (3).gameObject.SetActive (true);

            if (target != null)
            {
              GameManager.GetInstance ().RemoveMapHighlight ();
              if(GameManager.GetInstance().usingAbility.ability.abilityType == -1)
                GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().HealingTile, GameManager.GetInstance ().usingAbility.range, GameManager.GetInstance ().usingAbility.ability.rangeType);
              else 
                GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile,GameManager.GetInstance().usingAbility.range, GameManager.GetInstance().usingAbility.ability.rangeType);

              GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)target.gridPosition.x] [(int)target.gridPosition.z]);
            } 
            else 
            {
              if (GameManager.GetInstance().isAutoPlay) 
              {
                played = true;
                GameManager.GetInstance ().NextTurn ();
              } 
              else 
              {
                if(GameManager.GetInstance().usingAbility.ability.abilityType == -1)
                  GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().HealingTile, GameManager.GetInstance ().usingAbility.range, GameManager.GetInstance ().usingAbility.ability.rangeType);
                else 
                  GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile,GameManager.GetInstance().usingAbility.range, GameManager.GetInstance().usingAbility.ability.rangeType);
              }
            }
          }
        }
      }
    }
  }
  
  public override void TurnUpdate()
  {
    if (GameManager.GetInstance().isAutoPlay && currentHP > 0) 
    {
      if (played) GameManager.GetInstance ().NextTurn ();

      List<Tile> targetTilesInRange = new List<Tile> ();

      foreach (Tile t in TileHighLight.FindHighLight(GameManager.GetInstance().map[(int)gridPosition.x][(int)gridPosition.z],characterStatus.movementPoint, GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ()))
      {
        if (characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().ability.rangeType == 2)
        {
          foreach (Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false))
          {
            targetTilesInRange.Add (a);
          }
          foreach (Tile b in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true)) 
          {
            targetTilesInRange.Add (b);
          }
        } 
        else if (characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().ability.rangeType == 0)
        {
          foreach(Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false))
          {
            targetTilesInRange.Add (a);
          }
        }
        else
        {
          foreach(Tile a in TileHighLight.FindHighLight (t, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true))
          {
            targetTilesInRange.Add (a);
          }
        }
      }

      List<Tile> attackTilesInRange = new List<Tile> ();

      if (characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().ability.rangeType == 2) 
      {
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance().map  [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true)) 
        {
          attackTilesInRange.Add (t);
        }
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance().map  [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false)) 
        {
          attackTilesInRange.Add (t);
        }
      } 
      else if (characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().ability.rangeType == 1)
      {
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, true))
        {
          attackTilesInRange.Add (t);
        }
      } 
      else 
      {
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, true, false))
        {
          attackTilesInRange.Add (t);
        }
      }

      List<Tile> movementToAttackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint,  GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ());
      List<Tile> movementTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint + 99999);

      if (targetTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
      {
        var opponentsInRange = targetTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
        Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], GameManager.GetInstance ().map [(int)x.gridPosition.x] [(int)x.gridPosition.z]).Count () : 1000).First ();

        GameManager.GetInstance ().RemoveMapHighlight ();
        GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().MovementTile, characterStatus.movementPoint);

        if (GameManager.GetInstance ().CheckingMovementToAttackTarget (opponent.transform).gridPosition != gridPosition)
        {
          GameManager.GetInstance ().MoveCurrentCharacter (GameManager.GetInstance ().CheckingMovementToAttackTarget (opponent.transform));
        }
        else 
        {
          GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().range, characterStatus.equipedAbility.Where(x=>x.ability.abilityType == 1).First().ability.rangeType);
          GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.z]);
        }

        target = opponent;
      }
      else if (movementTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
      {
        if (movementToAttackTilesInRange.Count > 0) 
        {
          var opponentsInRange = movementTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
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
