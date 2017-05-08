using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using System.Linq;

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
    else
    {
      if(TemporaryData.GetInstance().version != Application.version)
      {
        PlayerPrefs.SetString (Const.Version, Application.version);
        TemporaryData.GetInstance().version = Application.version;
        #if UNITY_ANDROID
        var loadDb = new WWW(Application.streamingAssetsPath + "/" + Databasename);
        while(!loadDb.isDone) {Debug.Log("Error");}
        File.WriteAllBytes(filepath, loadDb.bytes);
        #elif UNITY_STANDALONE
        var loadDb = Application.streamingAssetsPath + "/" + Databasename;
        File.Copy(loadDb, filepath, true);
        #endif
      }
    }
    
    var dbPath = filepath;
    #endif
  
    string conn = "URI=file:" + dbPath;

    dbconn = new SqliteConnection (conn) as IDbConnection;
    if (dbconn.State != ConnectionState.Open) dbconn.Open ();
  }

  public static List<MapStory> GetMapOfType(int ID)
  {
    List<MapStory> list = new List<MapStory>  ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM MapStory" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (3) == ID)
      {
        MapStory n = new MapStory ();
        
        n.ID = reader.GetInt32 (0);
        n.storiesName = reader.GetString (1);
        n.storyTypes = reader.GetInt32 (3);
        string[] split = reader.GetString (2).Split ("," [0]);
        
        for(int i = 0; i < split.Length; i++)
        {
          n.mapID.Add (int.Parse (split[i]));
        }
        
        list.Add (n);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return list;
  }
  
  public static Ability GetAbility(int ID)
  {
    Ability n = new Ability ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Ability" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
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
        n.rangeType = reader.GetInt32 (9);
        n.abilityType = reader.GetInt32 (10);
        n.abilityEff = reader.GetInt32 (11);
        n.abilityElement = reader.GetInt32 (12);
        n.gaugeUse = reader.GetInt32 (13);
        n.coolDown = (int)reader.GetFloat (14);
        n.describe = reader.GetString (15);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }
  
  public static List<int> GetReward(string StoriesName)
  {
    List<int> reward = new List<int> ();
    
    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM RewardAfterStory" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == StoriesName)
      {
        string rewardID = reader.GetString (2);
        if (!string.IsNullOrEmpty (rewardID))
        {
          string[] _rewardID = rewardID.Split ("," [0]);
          for (int i = 0; i < _rewardID.Length; i++) 
          {
            reward.Add (int.Parse(_rewardID [i]));
          }
        }
        break;
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return reward;
  }

  public static AIInformation GetAiInfomation (int ID)
  {
    AIInformation n = new AIInformation ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM EnemyData" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
      {        
        string items = reader.GetString (1);
        if(!string.IsNullOrEmpty(items))
        {
          string[] item = items.Split ("," [0]);
          for(int i = 0; i < item.Length; i++)
          {
            n.droppedItem.Add (item[i]);
          }
        }
        n.givenGold = reader.GetInt32 (2);
        n.givenExp = reader.GetInt32 (3);
        n.effectiveAttack = reader.GetInt32 (4);
        n.element = reader.GetInt32 (5);
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }
  
  public static CharacterBasicStatus GetCharacter(int ID)
  {
    CharacterBasicStatus n = new CharacterBasicStatus ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM CharacterStatus" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
      {
        n.ID = reader.GetInt32 (0);
        n.characterName = reader.GetString (1);
        n.maxHP = (int)reader.GetFloat (2); 
        n.maxHpGrowth= reader.GetFloat (3) ;
        n.attack = (int)reader.GetFloat (4);
        n.attackGrowth = reader.GetFloat (5);
        n.defense = (int)reader.GetFloat (6);
        n.defenseGrowth = reader.GetFloat (7);
        n.criRate = reader.GetFloat (8);
        n.criRateGrowth = reader.GetFloat (9);
        n.movementPoint = (int)reader.GetFloat (10);
        string learnAbleAbility = reader.GetString (11);
        string[] learnAbleAb = learnAbleAbility.Split ("," [0]);
        for(int i = 0; i < learnAbleAb.Length; i++)
        {
          n.learnAbleAbility.Add (learnAbleAb[i]);
        }
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return n;
  }

  public static ItemStatus GetItemFromID(int ID)
  {
    ItemStatus n = new ItemStatus ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
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
        if (!string.IsNullOrEmpty (reader.GetString (10))) 
        {
          string sellMap = reader.GetString (10);
          string[] sm = sellMap.Split ("," [0]);
          for (int i = 0; i < sm.Length; i++)
          {
            n.sellMap.Add (sm [i]);
          }
        }
        n.stackable = reader.GetBoolean (11);
        
        reader.Close ();
        reader = null;
        dbcmd.Dispose ();
        dbcmd = null;
        
        break;
      }
    }
    
    return n;
  }

  public static List<ItemStatus> GetShopItem(List<int> passedMap)
  {
    List<ItemStatus> list = new List<ItemStatus> ();

    IDbCommand dbcmd = dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM Item" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      ItemStatus n = new ItemStatus ();

      
      string sellMap = null;
      List<string> sm = new List<string>();
      
      if (!string.IsNullOrEmpty (reader.GetString (10))) 
      {
        sellMap = reader.GetString (10);
        sm = sellMap.Split ("," [0]).ToList();
      }

      if(sm.Count > 0)
      {
        foreach (string s in sm) 
        {
          if (passedMap.Where (x => x >= int.Parse (s)).Count() > 0) 
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
            n.stackable = reader.GetBoolean (11);
            break;
          }
        }
      }
      else
      {
        continue;
      }
      list.Add (n);
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return list;
  }
}
