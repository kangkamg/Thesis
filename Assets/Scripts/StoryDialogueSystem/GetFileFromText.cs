﻿using System.Text;
using System.IO;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    TextAsset newTextAsset = Resources.Load<TextAsset> ("DialogueFile/" + filename);
    
    if (newTextAsset == null)
      return false;
    else 
    {
      List<string> line = newTextAsset.text.Split ("\n" [0]).ToList ();
      do 
      {
        string[] entry = line [0].Split ("," [0]);
        for (int i = 0; i < entry.Length; i++) 
        {
          newStoryDialogue.allDialogue.Add (entry [i]);
        }
        
        if(entry.Length > 1)
        {
          newStoryDialogue.characterName.Add (entry [1]);
          newStoryDialogue.bgName.Add (entry [2]);
        }
        
        line.RemoveAt(0);
      } while(line.Count > 0);
    
      return true;
    }
  }
}
