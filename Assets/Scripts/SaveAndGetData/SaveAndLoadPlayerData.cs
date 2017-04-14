using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class SaveAndLoadPlayerData
{
  public static void SaveData(object data,int saveID)
  {
    List<int> DataID = new List<int>();

    IDbCommand dbcmd =  GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM SaveAndLoad" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      DataID.Add (reader.GetInt32 (0));
    }
      
    if (DataID.Contains(saveID)) 
    {
      IDbCommand ucmd = GetDataFromSql.dbconn.CreateCommand ();
      string updateData = "UPDATE SaveAndLoad SET Data = '" + EncodeAndDeCode.Encode (data) + "' where ID = " + saveID + "; SELECT * " + "FROM SaveAndLoad"; 
      ucmd.CommandText = updateData;
      IDataReader update = ucmd.ExecuteReader ();

      update.Close ();
      ucmd.Dispose ();
    }
    else 
    {
      IDbCommand icmd = GetDataFromSql.dbconn.CreateCommand ();
      string insertQuery = "INSERT INTO SaveAndLoad(ID,Name,Data)" + "VALUES (" + saveID + " ,'" + "Geng" + "'" + ", '" + EncodeAndDeCode.Encode (data) + "' );"; 
      icmd.CommandText = insertQuery;
      IDataReader insert = icmd.ExecuteReader ();

      insert.Close ();
      icmd.Dispose ();
    }

    reader.Close ();
    dbcmd.Dispose ();
    reader = null;
    dbcmd = null;
  }

  public static object LoadData(int ID)
  {
    object data = new object();

    IDbCommand dbcmd =  GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM SaveAndLoad" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
      {
        data = EncodeAndDeCode.Decode (reader.GetString (2));
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;

    return data;
  }
  
  public static bool CheckingSave(int ID)
  {
    IDbCommand dbcmd =  GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM SaveAndLoad" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetInt32 (0) == ID)
      {
        reader.Close ();
        reader = null;
        dbcmd.Dispose ();
        dbcmd = null;
        return true;
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    return false;
  }
}
