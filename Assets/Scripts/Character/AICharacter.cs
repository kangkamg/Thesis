using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AICharacter : Character
{
  private void Update()
  {
    if (positionQueue.Count > 0)
    {
      transform.position = Vector3.MoveTowards (transform.position, positionQueue [0], moveSpeed*Time.deltaTime);
      if (Vector3.Distance (positionQueue [0], transform.position) < 0.1f) 
      {
        transform.position = positionQueue [0];
        positionQueue.RemoveAt (0);
        if (positionQueue.Count == 0)
        {
          
        }
      }
    }
  }
  public override void TurnUpdate()
  {
    List<Tile> attackTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], setupAbility [0].range,true,false); 
    List<Tile> movementTilesInRange = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint + 99999);

    if (attackTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP  > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
    {
      var opponentsInRange = attackTilesInRange.Select(x => GameManager.GetInstance().character.Where (z => z.GetType() != typeof(AICharacter) && z.currentHP  > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance().character.Where(z => z.gridPosition == x.gridPosition).First() : null).ToList();
      Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).First ();

      GameManager.GetInstance().RemoveMapHighlight();
      GameManager.GetInstance().HighlightTileAt(gridPosition, Color.red, setupAbility [0].range, setupAbility[0].rangeType);

      GameManager.GetInstance ().AttackWithCurrentCharacter (GameManager.GetInstance ().map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.z]);
    }

    else if (movementTilesInRange.Where (x => GameManager.GetInstance ().character.Where (z => z.GetType () != typeof(AICharacter) && z.currentHP  > 0 && z.gridPosition == x.gridPosition).Count () > 0).Count () > 0) 
    {
      var opponentsInRange = movementTilesInRange.Select(x => GameManager.GetInstance().character.Where (z => z.GetType() != typeof(AICharacter) && z.currentHP  > 0 && z != this && z.gridPosition == x.gridPosition).Count () > 0 ? GameManager.GetInstance().character.Where(z => z.gridPosition == x.gridPosition).First() : null).ToList();
      Character opponent = opponentsInRange.OrderBy (x => x != null ? -x.currentHP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(GameManager.GetInstance().map[(int)gridPosition.x][(int)gridPosition.z],GameManager.GetInstance().map[(int)x.gridPosition.x][(int)x.gridPosition.z]).Count() : 1000).First ();

      GameManager.GetInstance().RemoveMapHighlight();
      GameManager.GetInstance().HighlightTileAt(gridPosition, Color.blue, characterStatus.movementPoint);

      List<Tile> path = TilePathFinder.FindPathPlus (GameManager.GetInstance().map[(int)gridPosition.x][(int)gridPosition.z],GameManager.GetInstance().map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.z], GameManager.GetInstance().character.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
      if (path.Count () > 1) 
      {
        List<Tile> actualMovement = TileHighLight.FindHighLight (GameManager.GetInstance ().map [(int)gridPosition.x] [(int)gridPosition.z], characterStatus.movementPoint, GameManager.GetInstance().character.Where (x => x.gridPosition != gridPosition).Select (x => x.gridPosition).ToArray ());
        path.Reverse ();
          if (path.Where (x => actualMovement.Contains (x)).Count () > 0) GameManager.GetInstance ().MoveCurrentCharacter (path.Where (x => actualMovement.Contains (x)).First ());
      }
    }
  }
}
