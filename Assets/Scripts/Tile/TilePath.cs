using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TilePath 

{
  public List<Tile> listOfTiles = new List<Tile>();

  public Tile lastTile;

  public int costOfPath = 0;

  public TilePath()
  {
    
  }
  public TilePath(TilePath tp)
  {
    listOfTiles = tp.listOfTiles.ToList ();
    costOfPath = tp.costOfPath;
    lastTile = tp.lastTile;
  }

  public void AddTile(Tile t)
  {
    costOfPath += t.movementCost;
    listOfTiles.Add (t);
    lastTile = t;
  }

  public void AddStaticTile(Tile t)
  {
    costOfPath += 1;
    listOfTiles.Add (t);
    lastTile = t;
  }
}
