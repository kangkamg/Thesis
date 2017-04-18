using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileHighLight : MonoBehaviour
{
  public TileHighLight()
  {
    
  }

  public static List<Tile>FindHighLight(Tile originTile, int movementPoints, bool staticRange = false, bool cross = false)
  {
    if (!cross)
      return FindHighLightPlus (originTile, movementPoints, new Vector3 [0], staticRange);
    else
      return FindHighLightCross (originTile, movementPoints, new Vector3 [0]);
  }

  public static List<Tile>FindHighLight(Tile originTile, int movementPoints, Vector3[] occupied, bool staticRange = false, bool cross = false)
  {
    if (!cross)
      return FindHighLightPlus (originTile, movementPoints, occupied, staticRange);
    else
      return FindHighLightCross (originTile, movementPoints, occupied);
  }

  public static List<Tile>FindHighLightPlus (Tile originTile, int movementPoints, Vector3[] occupied, bool staticRange)
  {
    List<Tile> closed = new List<Tile> ();
    List<TilePath> path = new List<TilePath> ();

    TilePath originPath = new TilePath ();
    if (staticRange) originPath.AddStaticTile (originTile);
    else originPath.AddTile (originTile);

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
        if (!staticRange)
        {
          if (t.impassible || occupied.Contains (t.gridPosition))
            continue;
        }

        TilePath newTilePath = new TilePath (current);
        if (staticRange) newTilePath.AddStaticTile (t);
        else newTilePath.AddTile (t);
        path.Add (newTilePath);
      }
    }
    if (staticRange) 
    {
      for(int i = closed.Count-1 ; i >= 0;i--) 
      {
        if (closed [i].impassible) 
        {
          closed.RemoveAt (i);
        }
      }
    }
    if (staticRange) closed.Remove (originTile);
    closed.Distinct ();
    return closed;
  }

  public static List<Tile>FindHighLightCross (Tile originTile, int movementPoints, Vector3[] occupied)
  {
    List<Tile> closed = new List<Tile> ();
    List<TilePath> path = new List<TilePath> ();

    TilePath originPath = new TilePath ();
    originPath.AddStaticTile (originTile);

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
        newTilePath.AddStaticTile (t);
        path.Add (newTilePath);
      }
    }
    for(int i = closed.Count-1 ; i >= 0;i--) 
    {
      if (closed [i].impassible) 
      {
        closed.RemoveAt (i);
      }
    }
    closed.Remove (originTile);
    closed.Distinct ();
    return closed;
  }
}
