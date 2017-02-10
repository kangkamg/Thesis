using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class EncodeAndDeCode

{
  public static string Encode (object data)
  {
    IFormatter f = new BinaryFormatter();
    MemoryStream m = new MemoryStream();
    f.Serialize(m, data);
    byte[] dataBytes = m.ToArray();
    return Convert.ToBase64String(dataBytes);
  }

  public static object Decode (string data)
  {
    byte[] b = Convert.FromBase64String(data);
    Stream m = new MemoryStream(b);
    IFormatter f = new BinaryFormatter();
    return f.Deserialize(m);
  }
}
