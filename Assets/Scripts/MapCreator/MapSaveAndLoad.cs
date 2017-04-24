using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using System;
using System.Text;

[System.Serializable]
public class TileData
{
  public int type;
  public int locX;
  public int locZ;
}

[System.Serializable]
public class EnemyInMapData
{
  public int enemyID;
  public int locX;
  public int locZ;
}


public class MapDatabaseContainner
{
  public int size;
  public List<TileData> tiles = new List<TileData>();
  public List<EnemyInMapData> enemies = new List<EnemyInMapData> ();
}

public class MapSaveAndLoad 
{
  public static ArrayList list = new ArrayList();

  public static MapDatabaseContainner CreateMapContainer(List<List<Tile>> map, List<EnemyInMapData> enemy)
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
      tiles = tiles,
      enemies = enemy
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

  public static void SaveData(MapDatabaseContainner data, int mapNumber)
  {
    List<int> MapNumber = new List<int> ();

    IDbCommand dbcmd = GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      MapNumber.Add (reader.GetInt32 (0));
    }
      
    if (MapNumber.Contains(mapNumber)) 
    {
      IDbCommand ucmd = GetDataFromSql.dbconn.CreateCommand ();
      string updateMapSize = "UPDATE MapData SET mapsize = " + data.size + " where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData"; 
      string updateMapData = "UPDATE MapData SET mapdata = '" + EncodeAndDeCode.Encode (data.tiles) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updateEnemyInMap = "UPDATE MapData SET enemyInMap = '" + EncodeAndDeCode.Encode (data.enemies) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      ucmd.CommandText = updateMapSize;
      IDataReader update = ucmd.ExecuteReader ();
      update.Close ();

      ucmd.CommandText = updateMapData;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updateEnemyInMap;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.Dispose ();
    }
    else 
    {
      IDbCommand icmd = GetDataFromSql.dbconn.CreateCommand ();
      string insertQuery = "INSERT INTO MapData(mapno,mapsize,mapdata,enemyInMap)" + "VALUES (" + mapNumber + "," + data.size + ", '" + EncodeAndDeCode.Encode (data.tiles) + "', '" + EncodeAndDeCode.Encode(data.enemies) + "');"; 
      icmd.CommandText = insertQuery;
      IDataReader insert = icmd.ExecuteReader ();
     
      insert.Close ();
      icmd.Dispose ();
    }

    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
  }

  public static MapDatabaseContainner Load(int mapNumber)
  {
    List<TileData> data = new List<TileData> ();
    List<EnemyInMapData> enemy = new List<EnemyInMapData> ();
    int size = 0;
    
    IDbCommand dbcmd = GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == mapNumber)
      {
        size = reader.GetInt32 (1);
        data = EncodeAndDeCode.Decode (reader.GetString (2)) as List<TileData>;
        enemy = EncodeAndDeCode.Decode (reader.GetString (3)) as List<EnemyInMapData>;
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    
    return new MapDatabaseContainner () 
    {
      size = size,
      tiles = data,
      enemies = enemy
    };
  }
  
  public static bool CheckingMap(int mapNumber)
  {
    IDbCommand dbcmd = GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == mapNumber)
      {
        return true;
      }
    }
    
    return false;
  }
}
