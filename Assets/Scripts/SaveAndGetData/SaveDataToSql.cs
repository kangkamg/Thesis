using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class SaveDataToSql
{
  public static void SaveData(object data)
  {
    string conn = "URI=file:" + Application.dataPath + "/Database/ThesisDatabase.db";

    IDbConnection dbconn;
    dbconn = new SqliteConnection (conn) as IDbConnection;
    dbconn.Open ();

    IDbCommand icmd = dbconn.CreateCommand ();
    string insertQuery = "INSERT INTO SaveAndLoad(ID,Name,Data)" + "VALUES (" + "1 ,'" + "Geng" + "'" + ", '" + EncodeAndDeCode.Encode (data) + "' );"; 
    icmd.CommandText = insertQuery;
    IDataReader insert = icmd.ExecuteReader ();

    insert.Close ();
    icmd.Dispose ();
    insert = null;
    icmd = null;
    dbconn.Close ();
    dbconn = null;
  }

  public static object LoadData(string name)
  {
    object data = new object();

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
      if (reader.GetString (1) == name)
      {
        data = EncodeAndDeCode.Decode (reader.GetString (2));
      }
    }
    reader.Close ();
    reader = null;
    dbcmd.Dispose ();
    dbcmd = null;
    dbconn.Close ();
    dbconn = null;

    return data;
  }
}
