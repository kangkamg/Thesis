using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using System;
using System.Text;

public class TileData
{
  public int type;
  public int locX;
  public int locZ;
}

public class MapDatabaseContainner
{
  public int size;
  public List<TileData> tiles = new List<TileData>();
}

public class MapSaveAndLoad 
{
  public static ArrayList list = new ArrayList();

  public static MapDatabaseContainner CreateMapContainer(List<List<Tile>> map)
  {
    List<TileData> tiles = new List<TileData> ();

    for (int i = 0; i < map.Count; i++) 
    {
      for (int j = 0; j < map.Count; j++) 
      {
        tiles.Add (MapSaveAndLoad.CreateTileData(map[i][j]));
      }
    }

    return new MapDatabaseContainner () 
    {
      size = map.Count,
      tiles = tiles
    };
  }

  public static TileData CreateTileData(Tile tile)
  {
    return new TileData () 
    {
      type = (int)tile.type,
      locX = (int)tile.gridPosition.x,
      locZ = (int)tile.gridPosition.z
    };
  }

  public static ArrayList ChangeToArrayList(MapDatabaseContainner data)
  {
    list.Clear ();
    foreach (TileData t in data.tiles)
    {
      Hashtable h = new Hashtable();

      h.Clear ();

      h.Add ("type", t.type);
      h.Add ("locX", t.locX);
      h.Add ("locZ", t.locZ);

      list.Add (h);
    }
    return list;
  }

  public static void SaveData(MapDatabaseContainner data, int mapNumber)
  {
    ArrayList list = ChangeToArrayList(data);

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "INSERT INTO MapData(mapno,mapsize,mapdata)" + "VALUES ("+ mapNumber + "," + data.size + ", '" + easy.JSON.JsonEncode(list).ToString() + "' );" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();

    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;
  }

  public static MapDatabaseContainner Load(int mapNumber)
  {
    List<TileData> data = new List<TileData> ();
    int size = 0;

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == mapNumber)
      {
        size = reader.GetInt32 (1);
        ArrayList l = easy.JSON.JsonDecode (reader.GetString (2)) as ArrayList;

        foreach (Hashtable h in l)
        {
          TileData tile = new TileData();
          tile.type = (int)h ["type"];
          tile.locX = (int)h ["locX"];
          tile.locZ = (int)h ["locZ"];

          data.Add (tile);
        }
        break;
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return new MapDatabaseContainner () 
    {
      size = size,
      tiles = data
    };
  }
}
