using System.Text;
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
    TextAsset newTextAsset = new TextAsset ();
    if(Resources.Load<TextAsset> ("DialogueFile/" + TemporaryData.GetInstance().choosenLanguage + "/" + filename) != null)
      newTextAsset = Resources.Load<TextAsset> ("DialogueFile/" + TemporaryData.GetInstance().choosenLanguage + "/" + filename);
    else
      newTextAsset = Resources.Load<TextAsset> ("DialogueFile/Thai/" + filename);
    
    if (newTextAsset == null)
      return false;
    else 
    {
      List<string> line = newTextAsset.text.Split ("\n" [0]).ToList ();
      
      if (line.Count > 0) 
      {
        do 
        {
          string[] entry = line [0].Split ("," [0]);
        
          if (!entry [0].Contains ("AddCharacter") && !entry [0].Contains ("AddItem") && !entry [0].Contains ("Ending"))
          {
            if (entry.Length <= 1)
            {
              newStoryDialogue.allDialogue.Add ("-Not Complete-");
            }
            else
            {
              newStoryDialogue.allDialogue.Add (entry [0]);
            }
        
            if (entry.Length > 2) 
            {
              newStoryDialogue.characterName.Add (entry [1]);
              newStoryDialogue.bgName.Add (entry [2]);
            }
            else if (entry.Length > 1) 
            {
              newStoryDialogue.characterName.Add (entry [1]);
              newStoryDialogue.bgName.Add ("");
            } 
            else if (entry.Length <= 1)
            {
              newStoryDialogue.characterName.Add ("");
              newStoryDialogue.bgName.Add ("");
            }
          } 
          else 
          {
            for (int i = 0; i < entry.Length; i++)
            {
              newStoryDialogue.allDialogue.Add (entry [i]);
            }
          }
        
          line.RemoveAt (0);
        } while(line.Count > 0);
      }
      else
      {
        newStoryDialogue.allDialogue.Add ("-Not Complete-");
        newStoryDialogue.characterName.Add ("");
        newStoryDialogue.bgName.Add ("");
      } 
      return true;
    }
  }
}
