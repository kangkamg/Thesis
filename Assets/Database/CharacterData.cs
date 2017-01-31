using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data; 
using System;

[System.Serializable]
public class Character
{
  public int ID;
  public string name;
  public int level;
  public int speed;
  public int movementslot;
  public int exp;
}
  
public class CharacterData
{
  private static Dictionary<string, Character> _table;

  static CharacterData()
  {
    string conn = "URI=file:" + Application.dataPath + "/Database/CharacterStatusData.db"; //Path to database.
    IDbConnection dbconn;
    dbconn = (IDbConnection) new SqliteConnection(conn);
    dbconn.Open(); //Open connection to the database.
    IDbCommand dbcmd = dbconn.CreateCommand();
    string sqlQuery = "SELECT ID,Name, Level, Speed, MovementSlot, EXP " + "FROM Character";
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader();
    _table = new Dictionary<string,Character> ();
   
    while (reader.Read())
    {
      Character newData = new Character()
      {
        ID = reader.GetInt32(0),
        name = reader.GetString(1),
        level = reader.GetInt32(2),
        speed = reader.GetInt32 (3),
        movementslot = reader.GetInt32 (4),
        exp = reader.GetInt32 (5),
      };

      _table.Add (newData.name, newData);
    }
    reader.Close();
    reader = null;
    dbcmd.Dispose();
    dbcmd = null;
    dbconn.Close();
    dbconn = null;
  }

  public static Character GetName(string name)
  {
    Character temp = null;
    if (_table.TryGetValue(name, out temp))
    {
      return temp;
    }
    else
    {
      return null;
    }
  }
}
