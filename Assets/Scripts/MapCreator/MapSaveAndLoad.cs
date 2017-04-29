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
public class MapStory
{
  public int ID;
  public int storyTypes;
  public string storiesName;
  public List<int> mapID = new List<int>();
}

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

[System.Serializable]
public class PlayerInMapData
{
  public int locX;
  public int locZ;
}

[System.Serializable]
public class ObstacleInMap
{
  public int objs;
  public int locX;
  public int locZ;
}


public class MapDatabaseContainner
{
  public int[] size = new int[]{1,1};
  public List<TileData> tiles = new List<TileData>();
  public List<EnemyInMapData> enemies = new List<EnemyInMapData> ();
  public List<PlayerInMapData> players = new List<PlayerInMapData> ();
  public List<ObstacleInMap> objs = new List<ObstacleInMap> ();
  
  public int mapObjective;
  public List<Vector3> objectivePos = new List<Vector3>();
}

public class MapSaveAndLoad 
{
  public static ArrayList list = new ArrayList();

  public static MapDatabaseContainner CreateMapContainer(List<List<Tile>> map, List<EnemyInMapData> enemy, List<PlayerInMapData> player, List<ObstacleInMap> obj, int _mapObjective, List<Vector3> mapObjectivePos)
  {
    List<TileData> tiles = new List<TileData> ();

    for (int i = 0; i < map.Count; i++) 
    {
      for (int j = 0; j < map[i].Count; j++) 
      {
        tiles.Add (MapSaveAndLoad.CreateTileData(map[i][j]));
      }
    }

    return new MapDatabaseContainner () 
    {
      size = new int[]{map.Count,map[0].Count},
      tiles = tiles,
      enemies = enemy,
      players = player,
      objs = obj,
      mapObjective = _mapObjective,
      objectivePos = mapObjectivePos
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
      string updateMapSize = "UPDATE MapData SET mapsize = '" + data.size[0] + "," + data.size[1] + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData"; 
      string updateMapData = "UPDATE MapData SET mapdata = '" + EncodeAndDeCode.Encode (data.tiles) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updateEnemyInMap = "UPDATE MapData SET enemyInMap = '" + EncodeAndDeCode.Encode (data.enemies) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updatePlayerInMap = "UPDATE MapData SET playerInMap = '" + EncodeAndDeCode.Encode (data.players) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updateObstacleInMap = "UPDATE MapData SET obstacleInMap = '" + EncodeAndDeCode.Encode (data.objs) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updateMapObjective = "UPDATE MapData SET mapObjective = '" + data.mapObjective + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      string updateMapObjectivePosition = "UPDATE MapData SET mapObjectivePos = '" + EncodeAndDeCode.Encode (data.objectivePos) + "' where mapno = " + mapNumber + ";" + "SELECT * " + "FROM MapData";
      ucmd.CommandText = updateMapSize;
      IDataReader update = ucmd.ExecuteReader ();
      update.Close ();

      ucmd.CommandText = updateMapData;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updateEnemyInMap;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updatePlayerInMap;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updateObstacleInMap;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updateMapObjective;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.CommandText = updateMapObjectivePosition;
      update = ucmd.ExecuteReader ();
      update.Close ();
      
      ucmd.Dispose ();
    }
    else 
    {
      IDbCommand icmd = GetDataFromSql.dbconn.CreateCommand ();
      string insertQuery = "INSERT INTO MapData(mapno,mapsize,mapdata,enemyInMap,playerInMap,obstacleInMap,mapObjective,mapObjectivePos)" + "VALUES (" + mapNumber + ", '" + data.size[0] + "," + data.size[1] + 
        "', '" + EncodeAndDeCode.Encode (data.tiles) + "', '" + EncodeAndDeCode.Encode(data.enemies) + "', '" + EncodeAndDeCode.Encode(data.players) + "', '" + EncodeAndDeCode.Encode(data.objs) + "','" + data.mapObjective
        + "','" + EncodeAndDeCode.Encode (data.mapObjective) + "');"; 
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
    List<PlayerInMapData> player = new List<PlayerInMapData> ();
    List<ObstacleInMap> obj = new List<ObstacleInMap> ();
    int[] size = new int[]{ 1, 1};
    int _mapObjective = 0;
    List<Vector3> mapObjectivePos = new List<Vector3> ();
    
    IDbCommand dbcmd = GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == mapNumber)
      {
        string[] _size = reader.GetString(1).Split(","[0]);
        size = new int[]{ int.Parse (_size [0]), int.Parse (_size [1]) };
        
        if(!string.IsNullOrEmpty(reader.GetString(2)))
          data = EncodeAndDeCode.Decode (reader.GetString (2)) as List<TileData>;
        if(!string.IsNullOrEmpty(reader.GetString(3)))
          enemy = EncodeAndDeCode.Decode (reader.GetString (3)) as List<EnemyInMapData>;
        if(!string.IsNullOrEmpty(reader.GetString(4)))
          player = EncodeAndDeCode.Decode (reader.GetString (4)) as List<PlayerInMapData>;
        if(!string.IsNullOrEmpty(reader.GetString(5)))
          obj = EncodeAndDeCode.Decode (reader.GetString (5)) as List<ObstacleInMap>;
        if (!string.IsNullOrEmpty (reader.GetInt32 (6).ToString ()))
          _mapObjective = reader.GetInt32 (6);
        if (!string.IsNullOrEmpty (reader.GetString (7).ToString ()))
          mapObjectivePos = EncodeAndDeCode.Decode (reader.GetString (7)) as List<Vector3>;
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
      enemies = enemy,
      players = player,
      objs = obj,
      mapObjective = _mapObjective,
      objectivePos = mapObjectivePos
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
