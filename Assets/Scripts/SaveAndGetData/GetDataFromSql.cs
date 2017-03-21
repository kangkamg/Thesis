using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class GetDataFromSql
{
  public static IDbConnection dbconn;

  public static void OpenDB(string Databasename)
  {
    #if UNITY_EDITOR
    var dbPath = string.Format(@"Assets/StreamingAssets/{0}",Databasename);
    #else
    var filepath = string.Format("{0}/{1}",Application.persistentDataPath, Databasename);

    if(!File.Exists(filepath))
    {
      #if UNITY_ANDROID
      var loadDb = new WWW(Application.streamingAssetsPath + "/" + Databasename);
      while(!loadDb.isDone) {Debug.Log("Error");}
      File.WriteAllBytes(filepath, loadDb.bytes);
      #elif UNITY_STANDALONE
      var loadDb = Application.streamingAssetsPath + "/" + Databasename;
      File.Copy(loadDb, filepath);
      #endif
    }
    var dbPath = filepath;
    #endif
  
    string conn = "URI=file:" + dbPath;

    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();
  }

  public static Ability GetAbility(string name)
  {
    Ability n = new Ability ();

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
        n.power = reader.GetFloat (2);
        n.powerGrowth = reader.GetFloat (3);
        n.hitAmount = (int)reader.GetFloat (4);
        n.hitAmountGrowth = (int)reader.GetFloat (5);
        n.range = (int)reader.GetFloat (6);
        n.rangeGrowth = (int)reader.GetFloat (7);
        n.usingAround = reader.GetBoolean (8);
        n.rangeType = reader.GetString (9);
        n.abilityType = reader.GetString (10);
        n.gaugeUse = reader.GetInt32 (11);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }

  public static CharacterBasicStatus GetCharacter(string name)
  {
    CharacterBasicStatus n = new CharacterBasicStatus ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM CharacterStatus" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        n.ID = reader.GetInt32 (0);
        n.characterName = reader.GetString (1);
        n.maxHP = (int)reader.GetFloat (2); 
        n.maxHpGrowth= (int)reader.GetFloat (3) ;
        n.attack = (int)reader.GetFloat (4);
        n.attackGrowth = (int)reader.GetFloat (5);
        n.defense = (int)reader.GetFloat (6);
        n.defenseGrowth = (int)reader.GetFloat (7);
        n.criRate = (int)reader.GetFloat (8);
        n.criRateGrowth = (int)reader.GetFloat (9);
        n.movementPoint = (int)reader.GetFloat (10);
        n.normalAttack = GetAbility (reader.GetString (11));
        if (reader.GetString (12) != "None") 
        {
          n.specialAttack = GetAbility (reader.GetString (12));
        }
        n.weaponEff = reader.GetString (13);
        n.armorEff = reader.GetString (14);
        n.type = reader.GetString (15);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }
    
  public static ItemStatus GetItemFromName(string name)
  {
    ItemStatus n = new ItemStatus ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
      {
        n.ID = reader.GetInt32 (0);
        n.name = reader.GetString (1);
        n.price = reader.GetInt32 (2);
        n.increaseHP = (int)reader.GetFloat (3);
        n.increaseAttack = (int)reader.GetFloat (4);
        n.increaseDefense = (int)reader.GetFloat (5);
        n.increaseCriRate = (int)reader.GetFloat (6);
        n.increaseMovementPoint = (int)reader.GetFloat (7);
        n.itemType1 = reader.GetString(8);
        n.itemType2 = reader.GetString(9);
        string sellMap = reader.GetString (10);
        string[] sm = sellMap.Split ("," [0]);
        for(int i = 0; i < sm.Length; i++)
        {
          n.sellMap.Add (sm[i]);
        }
        n.stackable = reader.GetBoolean (11);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }

  public static List<ItemStatus> GetItemFromMap(string mapNumber)
  {
    List<ItemStatus> list = new List<ItemStatus> ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      ItemStatus n = new ItemStatus ();
      List<string> s = new List<string> ();

      string sellMap = reader.GetString (10);
      string[] sm = sellMap.Split ("," [0]);
      for(int i = 0; i < sm.Length; i++)
      {
        s.Add (sm[i]);
      }

      if (s.Contains(mapNumber))
      {
        n.name = reader.GetString (1);
        n.price = reader.GetInt32 (2);
        n.increaseHP = (int)reader.GetFloat (3);
        n.increaseAttack = (int)reader.GetFloat (4);
        n.increaseDefense = (int)reader.GetFloat (5);
        n.increaseCriRate = (int)reader.GetFloat (6);
        n.increaseMovementPoint = (int)reader.GetFloat (7);
        n.itemType1 = reader.GetString(8);
        n.itemType2 = reader.GetString(9);
        for(int i = 0; i < sm.Length; i++)
        {
          n.sellMap = s;
        }
        n.stackable = reader.GetBoolean (11);
      }
      list.Add (n);
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return list;
  }

  public static StoryDialogue storyDialogue(int ID)
  {
    StoryDialogue dialogue = new StoryDialogue ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM StoryData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32(0) == ID)
      {
        string allDialogue = reader.GetString (1);
        string[] allDialogueSplit = allDialogue.Split ("," [0]);
        for (int i = 0; i < allDialogueSplit.Length; i++)
        {
          dialogue.allDialogue.Add (allDialogueSplit [i]);
        }
        string characterName = reader.GetString (2);
        string[] characterNameSplit = characterName.Split ("," [0]);
        for (int i = 0; i < characterNameSplit.Length; i++)
        {
          dialogue.characterName.Add (characterNameSplit [i]);
        }

        break;
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return dialogue;
  }
}
