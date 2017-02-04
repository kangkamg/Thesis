using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class EncodeAndDeCode

{
  static readonly string PasswordHash = "AJDVD";
  static readonly string SaltKey = "BurgurMeat";
  static readonly string VIKey = "@1B2c3D4e5f6G7HB";

  public static string Encode (string data)
  {
    byte[] dataBytes = Encoding.UTF32.GetBytes (data);
    byte[] keyBytes = new Rfc2898DeriveBytes (PasswordHash, Encoding.ASCII.GetBytes (SaltKey)).GetBytes (256 / 8);

    var symmetricKey = new RijndaelManaged () { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
    var encrypter = symmetricKey.CreateEncryptor (keyBytes, Encoding.ASCII.GetBytes (VIKey));

    byte[] cipherBytes;

    using (var memoryStream = new MemoryStream ()) 
    {
      using (var cryptoStream = new CryptoStream (memoryStream, encrypter, CryptoStreamMode.Write)) 
      {
        cryptoStream.Write (dataBytes, 0, dataBytes.Length);
        cryptoStream.FlushFinalBlock ();
        cipherBytes = memoryStream.ToArray ();
        cryptoStream.Close ();
      }
      memoryStream.Close ();
    }
    return Convert.ToBase64String (cipherBytes);
  }

  public static string Decode (string encrypt)
  {
    byte[] cipherBytes = Convert.FromBase64String (encrypt);
    byte[] keyBytes = new Rfc2898DeriveBytes (PasswordHash, Encoding.ASCII.GetBytes (SaltKey)).GetBytes (256 / 8);

    var symmetricKey = new RijndaelManaged () { Mode = CipherMode.CBC, Padding = PaddingMode.None };
    var decrypter = symmetricKey.CreateDecryptor (keyBytes, Encoding.ASCII.GetBytes (VIKey));
    var memoryStream = new MemoryStream (cipherBytes);
    var cryptoStream = new CryptoStream (memoryStream, decrypter, CryptoStreamMode.Read);

    byte[] dataBytes = new byte[cipherBytes.Length];

    int decryptedByteCount = cryptoStream.Read (dataBytes, 0, dataBytes.Length);

    memoryStream.Close ();
    cryptoStream.Close ();

    return Encoding.UTF32.GetString (dataBytes, 0, decryptedByteCount).TrimEnd ("\0".ToCharArray ());
  }
}
