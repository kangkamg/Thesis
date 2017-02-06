using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileHighLight : MonoBehaviour
{
  public static List<Tile>FindHighLight (Tile originTile, int movementPoints, bool cross = false)
  {
    if (!cross)
    return FindHighLightPlus (originTile, movementPoints, new Vector3 [0]);
    else
    return FindHighLightCross (originTile, movementPoints, new Vector3 [0]);
  }
  
  public static List<Tile>FindHighLightPlus (Tile originTile, int movementPoints, Vector3[] occupied)
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
      if (current.costOfPath > movementPoints + 1)
      {
        continue;
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
      
    return closed;
  }

  public static List<Tile>FindHighLightCross (Tile originTile, int movementPoints, Vector3[] occupied)
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
      if (current.costOfPath > movementPoints + 1)
      {
        continue;
      }
      closed.Add (current.lastTile);

      foreach (Tile t in current.lastTile.neighborsCross)
      {
        if (t.impassible || occupied.Contains(t.gridPosition)) continue;
        TilePath newTilePath = new TilePath (current);
        newTilePath.AddTile (t);
        path.Add (newTilePath);
      }
    }
    return closed;
  }
}
