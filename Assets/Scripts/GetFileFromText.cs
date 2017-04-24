using System.Text;
using System.IO;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GetTextAssetFile
{
  
  private static GetTextAssetFile _instance;
  
  public static GetTextAssetFile GetInstance()
  {
    if (_instance == null)
      _instance = new GetTextAssetFile ();
    return _instance;
  }
  
  public StoryDialogue newStoryDialogue = new StoryDialogue();
  
  public StoryDialogue LoadText(string filename)
  {
    newStoryDialogue = new StoryDialogue ();
    if (Load (filename)) 
    {
      return newStoryDialogue;
    }
    return null;
  }
  
  public bool Load(string filename)
  {
    try
    {
      string line;
      
      StreamReader theReader = new StreamReader(filename, Encoding.Default);
      
      using (theReader)
      {
        line = theReader.ReadLine();
        if(line!=null)
        {
          do
          {
            string[] entries = line.Split(',');
            if(entries.Length > 0)
            {
              for (int i = 0;i<entries.Length ;i+=3)
              {
                newStoryDialogue.allDialogue.Add(entries[i]);
                newStoryDialogue.allDialogue.Add(entries[i+1]);
                newStoryDialogue.allDialogue.Add(entries[i+2]);
                newStoryDialogue.characterName.Add(entries[i+1]);
                newStoryDialogue.bgName.Add(entries[i+2]);
              }
            }
            line = theReader.ReadLine();
          }
          while(line!=null);
          
        }
        
        theReader.Close();
        return true;
      }
    }
    catch(Exception e)
    {
      Debug.Log(e.Message);
      return false;
    }
  }
}
