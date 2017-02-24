using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class GetDataFromSql
{
  public static Ability GetAbility(string name)
  {
    Ability n = new Ability ();

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Ability" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        n.ID = reader.GetInt32 (0);
        n.abilityName = reader.GetString (1);
        n.atkMultiply = reader.GetFloat (2);
        n.healMultiply = reader.GetFloat (3);
        n.atkIncrease = reader.GetFloat (4);
        n.defIncrease = reader.GetFloat (5);
        n.criRateIncrease = reader.GetFloat (6);
        n.movementIncrease = reader.GetFloat (7);
        n.guardRateIncrease = reader.GetFloat (8);
        n.range = reader.GetInt32 (9);
        n.usingAround = reader.GetBoolean (10);
        n.abnormalStatusGiven = reader.GetString (11);
        n.rangeType = reader.GetString (12);
        n.abilityType = reader.GetString (13);
        n.gaugeUse = reader.GetInt32 (14);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return n;
  }

  public static CharacterStatus GetCharacter(string name)
  {
    CharacterStatus n = new CharacterStatus ();

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM CharacterStatusBasic" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        if (reader.GetString (11) == "SwordMan") 
        {
          n.ID = reader.GetInt32 (0);
          n.characterName = reader.GetString (1);
          n.level = reader.GetInt32 (2);
          n.experience = reader.GetInt32 (3);
          n.maxHP = (int)reader.GetFloat (4) * n.level + 25;
          n.attack = (int)reader.GetFloat (5) * n.level;
          n.defense = (int)reader.GetFloat (6) * n.level ;
          n.resistance = (int)reader.GetFloat (7) * n.level ;
          n.criRate = (int)reader.GetFloat (8) * n.level ;
          n.guardRate = (int)reader.GetFloat (9) * n.level;
          n.job = reader.GetString (11);
          n.type = reader.GetString (12);
          string weaponPro = reader.GetString (13);
          string[] wp = weaponPro.Split (","[0]);
          foreach (string w in wp) 
          {
            n.weaponProficiency.Add (w);
          }
        }
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return n;
  }

  public static Item GetItemFromName(string name)
  {
    Item n = new Item ();

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        n.name = reader.GetString (1);
        n.price = reader.GetInt32 (2);
        n.increaseHP = reader.GetInt32 (4);
        n.increaseAttack = reader.GetInt32 (5);
        n.increaseDefense = reader.GetInt32 (6);
        n.increaseCriRate = (int)reader.GetFloat (7);
        n.increaseGuardRate = (int)reader.GetFloat (8);
        n.increaseMovementPoint = (int)reader.GetInt32 (9);
        string ability = reader.GetString (10);
        string[] ab = ability.Split (" "[0]);
        for(int i = 0; i < ab.Length-1; i=i+2)
        {
          if (int.Parse(ab[i+1]) == 1)
          {
            n.itemAb.Add (GetAbility(ab[i]));
          }
        }
        n.IsRuneStone = reader.GetBoolean (11);
        n.itemType = reader.GetString(12);
        string sellMap = reader.GetString (13);
        string[] sm = sellMap.Split ("," [0]);
        for(int i = 0; i < sm.Length; i++)
        {
          n.sellMap.Add (sm[i]);
        }
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return n;
  }

  public static List<Item> GetItemFromMap(string mapNumber)
  {
    List<Item> list = new List<Item> ();

    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      Item n = new Item ();
      List<string> s = new List<string> ();

      string sellMap = reader.GetString (13);
      string[] sm = sellMap.Split ("," [0]);
      for(int i = 0; i < sm.Length; i++)
      {
        s.Add (sm[i]);
      }

      if (s.Contains(mapNumber))
      {
        n.name = reader.GetString (1);
        n.price = reader.GetInt32 (2);
        n.increaseHP = reader.GetInt32 (4);
        n.increaseAttack = reader.GetInt32 (5);
        n.increaseDefense = reader.GetInt32 (6);
        n.increaseCriRate = (int)reader.GetFloat (7);
        n.increaseGuardRate = (int)reader.GetFloat (8);
        n.increaseMovementPoint = (int)reader.GetInt32 (9);
        string ability = reader.GetString (10);
        string[] ab = ability.Split (" "[0]);
        for(int i = 0; i < ab.Length-1; i=i+2)
        {
          if (int.Parse(ab[i+1]) == 1)
          {
            n.itemAb.Add (GetAbility(ab[i]));
          }
        }
        n.IsRuneStone = reader.GetBoolean (11);
        n.itemType = reader.GetString(12);
        for(int i = 0; i < sm.Length; i++)
        {
          n.sellMap = s;
        }
      }
      list.Add (n);
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return list;
  }

}
