using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCharacter : Character 
{
  public override void Awake()
  {
    characterStatus.movementPoint = 5;

    base.Awake ();
  }
	
  public override void Update()
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
              GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile, setupAbility [0].range, setupAbility [0].rangeType);

              GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)target.gridPosition.x] [(int)target.gridPosition.z]);
              target = null;
            } 
            else 
            {
              GameManager.GetInstance().HighlightTileAt(gridPosition, PrefabHolder.GetInstance().AttackTile, setupAbility [0].range, setupAbility[0].rangeType);
            }
            if (isAI) 
            {
              played = true;
              GameManager.GetInstance ().NextTurn ();
            }
          }
        }
      }
    }
    base.Update ();
  }

  public void UpdateUI()
  {
    
  }

  public override void TurnUpdate()
  {
    if (isAI && currentHP > 0) 
    {
      List<Tile> targetTilesInRange = new List<Tile> ();

      foreach (Tile t in TileHighLight.FindHighLight(GameManager.GetInstance().map[(int)gridPosition.x][(int)gridPosition.z],characterStatus.movementPoint, GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ()))
      {
        if (setupAbility [0].rangeType == "both")
        {
          foreach (Tile a in TileHighLight.FindHighLight (t, setupAbility [0].range, true, false))
          {
            targetTilesInRange.Add (a);
          }
          foreach (Tile b in TileHighLight.FindHighLight (t, setupAbility [0].range, true, true)) 
          {
            targetTilesInRange.Add (b);
          }
        } 
        else if (setupAbility [0].rangeType == "plus")
        {
          targetTilesInRange = TileHighLight.FindHighLight (t, setupAbility [0].range, true, false);
        }
        else
        {
          targetTilesInRange = TileHighLight.FindHighLight (t, setupAbility [0].range, true, true);
        }
      }

      List<Tile> attackTilesInRange = new List<Tile> ();

      if (setupAbility [0].rangeType == "both") 
      {
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance().map  [(int)gridPosition.x] [(int)gridPosition.z], setupAbility[0].range, true, true)) 
        {
          attackTilesInRange.Add (t);
        }
        foreach (Tile t in TileHighLight.FindHighLight (GameManager.GetInstance().map  [(int)gridPosition.x] [(int)gridPosition.z], setupAbility[0].range, true, false)) 
        {
          attackTilesInRange.Add (t);
        }
      } 
      else if (setupAbility [0].rangeType == "cross")
      {
        attackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], setupAbility [0].range, true, true);
      } 
      else 
      {
        attackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], setupAbility [0].range, true, false);
      }

      List<Tile> movementToAttackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint,  GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ());
      List<Tile> movementTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint + 99999);

      if (attackTilesInRange.Where (x => GameManager.GetInstance().character.Where(z=> z.GetType() != typeof(PlayerCharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0)
      {
        var opponentsInRange = attackTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
        Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).First ();

        GameManager.GetInstance ().RemoveMapHighlight ();
        GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().AttackTile, setupAbility [0].range, setupAbility [0].rangeType);

        GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.z]);

        played = true;
        GameManager.GetInstance ().NextTurn ();
      }
      else if (targetTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
      {
        var opponentsInRange = targetTilesInRange.Select (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(PlayerCharacter) && z.currentHP > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance ().character.Where (z => z.gridPosition == x.gridPosition).First () : null).ToList ();
        Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], GameManager.GetInstance ().map [(int)x.gridPosition.x] [(int)x.gridPosition.z]).Count () : 1000).First ();

        movementToAttackTilesInRange.Sort (delegate(Tile a, Tile b) 
          {
            return(Vector3.Distance (a.gridPosition, opponent.gridPosition).CompareTo (Vector3.Distance (b.gridPosition, opponent.gridPosition)));
          });

        GameManager.GetInstance ().RemoveMapHighlight ();
        GameManager.GetInstance ().HighlightTileAt (gridPosition, PrefabHolder.GetInstance ().MovementTile, characterStatus.movementPoint);

        GameManager.GetInstance ().MoveCurrentCharacter (movementToAttackTilesInRange[0]);

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
