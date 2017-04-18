using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveAndLoadPlayerData
{
  public static void SaveData(object data,int saveID)
  {
    string saveGame = EncodeAndDeCode.Encode (data);
    
    BinaryFormatter bf = new BinaryFormatter ();
    FileStream file = File.Create (Application.persistentDataPath + "/Save" + saveID);
    
    bf.Serialize (file, saveGame);
    file.Close ();
  }

  public static object LoadData(int ID)
  {
    object data = new object();

    if (CheckingSave (ID))
    {
      BinaryFormatter bf = new BinaryFormatter ();
      FileStream file = File.Open (Application.persistentDataPath + "/Save" + ID, FileMode.Open);
      string stringData = (string)bf.Deserialize (file);
      file.Close ();
      
      data = EncodeAndDeCode.Decode (stringData);
    }
    
    return data;
  }
  
  public static bool CheckingSave(int ID)
  {
    if (File.Exists (Application.persistentDataPath + "/Save" + ID)) 
    {
      return true;
    } 
    else
    {
      return false;
    }
  }
}
