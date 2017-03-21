using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class SaveAndLoadPlayerData
{
  public static void SaveData(object data)
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
      
    if (DataID.Contains(1)) 
    {
      IDbCommand ucmd = GetDataFromSql.dbconn.CreateCommand ();
      string updateData = "UPDATE SaveAndLoad SET Data = '" + EncodeAndDeCode.Encode (data) + "' where ID = 1;" + "SELECT * " + "FROM SaveAndLoad"; 
      ucmd.CommandText = updateData;
      IDataReader update = ucmd.ExecuteReader ();

      update.Close ();
      ucmd.Dispose ();
    }
    else 
    {
      IDbCommand icmd = GetDataFromSql.dbconn.CreateCommand ();
      string insertQuery = "INSERT INTO SaveAndLoad(ID,Name,Data)" + "VALUES (" + "1 ,'" + "Geng" + "'" + ", '" + EncodeAndDeCode.Encode (data) + "' );"; 
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

  public static object LoadData(string name)
  {
    object data = new object();

    IDbCommand dbcmd =  GetDataFromSql.dbconn.CreateCommand ();

    string sqlQuery = "SELECT *" + "FROM SaveAndLoad" ; 
    dbcmd.CommandText = sqlQuery;
    IDataReader reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) 
    {
      if (reader.GetString (1) == name)
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
}
