using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TilePathFinder : MonoBehaviour
{
  public static List<Tile>FindPath (Tile originTile, Tile desTile, bool cross = false)
  {
    return FindPathPlus (originTile, desTile, new Vector3 [0]);
  }

  public static List<Tile>FindPathPlus (Tile originTile, Tile desTile, Vector3[] occupied)
  {
    List<Tile> closed = new List<Tile> ();
    List<TilePath> path = new List<TilePath> ();

    TilePath originPath = new TilePath ();
    originPath.AddTile (originTile);

    path.Add (originPath);
    while (path.Count > 0) 
    {
      TilePath current = path [0];
      path.Remove (path [0]);

      if (closed.Contains (current.lastTile))
      {
        continue;
      }
      if (current.lastTile == desTile )
      {
        current.listOfTiles.Distinct ();
        current.listOfTiles.Remove (originTile);
        return current.listOfTiles;
      }
      closed.Add (current.lastTile);

      foreach (Tile t in current.lastTile.neighborsPlus)
      {
        if (t.impassible || occupied.Contains(t.gridPosition)) continue;
        TilePath newTilePath = new TilePath (current);
        newTilePath.AddTile (t);
        path.Add (newTilePath);
      }
    }
    return null;
  }
}
