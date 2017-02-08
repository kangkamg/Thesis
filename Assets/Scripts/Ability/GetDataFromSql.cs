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
        n.defIncrease = reader.GetFloat (4);
        n.atkIncrease = reader.GetFloat (5);
        n.criRateIncrease = reader.GetFloat (6);
        n.movementIncrease = reader.GetFloat (7);
        n.dodgeIncrease = reader.GetFloat (8);
        n.guardRateIncrease = reader.GetFloat (9);
        n.range = reader.GetInt32 (10);
        n.usingAround = reader.GetBoolean (11);
        n.active = reader.GetBoolean (12);
        n.rangeType = reader.GetString (13);
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

    string sqlQuery = "SELECT *" + "FROM Character" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        if (reader.GetString (11) == "Lancer") 
        {
          n.ID = reader.GetInt32 (0);
          n.characterName = reader.GetString (1);
          n.level = reader.GetInt32 (2);
          n.experience = reader.GetInt32 (3);
          n.maxHP = (int)reader.GetFloat (4) * n.level;
          n.attack = (int)reader.GetFloat (5) * n.level;
          n.defense = (int)reader.GetFloat (6) * n.level ;
          n.criRate = (int)reader.GetFloat (7) * n.level ;
          n.dogdeRate = (int)reader.GetFloat (8) * n.level;
          n.guardRate = (int)reader.GetFloat (9) * n.level;
          n.job = reader.GetString (11);
          n.type = reader.GetString (12);
          string ability = reader.GetString (10);
          string[] ab = ability.Split (" "[0]);
          for (int i = 0; i < ab.Length - 1; i = i + 2) 
          {
            if (int.Parse(ab [i]) == n.level)
            {
              n.characterAbility.Add (GetAbility (ab [i + 1]));
            }
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
}
