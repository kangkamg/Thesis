﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class StoryDialogue
{
  public List<string> characterName = new List<string>();
  public List<string> allDialogue =  new List<string>();
  public List<string> bgName = new List<string>();
}

public class DialogueManager : MonoBehaviour 
{
  public Text _textComponent;
  public GameObject dialogue;
  public Image BG;

  public StoryDialogue storyDialogue;
  public List<string> dialogueWord;
  public List<string> dialogueSpeaker;
  public List<string> dialogueBG;

  public float SecondBetweenCharacters = 0.01f;
  public float CharacterRateMultiplier = 0.05f;

  private bool _isName = false;
  private bool _isStringBeingReveled = false;
  private bool _isDialoguePlaying = false;
  private bool _isEndOfDialogue = false;

  public GameObject LeftCharacter;
  public GameObject RightCharacter;

  public GameObject talkingCharacter;

  public string oldLeft;
  public string oldRight;

  private void Start()
  {
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene ().name);
    
    _textComponent.text = "";
     
    storyDialogue = GetTextAssetFile.GetInstance().LoadText(Application.dataPath + "/Resources/DialogueFile/" + "D" + TemporaryData.GetInstance().playerData.storyID +"M" + PlayerPrefs.GetInt(Const.MapNo,0)  +".txt");
      
    for (int i = 0; i < storyDialogue.allDialogue.Count; i++)
    {
      foreach (string speaker in storyDialogue.characterName) 
      {
        if (storyDialogue.allDialogue [i] == speaker) 
        {
          dialogueSpeaker.Add (storyDialogue.allDialogue [i]);
          _isName = true;
          break;
        }
      }
      
      foreach (string bg in storyDialogue.bgName)
      {
        if (storyDialogue.allDialogue [i] == bg)
        {
          dialogueBG.Add (storyDialogue.allDialogue [i]);
          _isName = true;
          break;
        }
      }
      
      if (_isName) 
      {
        _isName = false;
        continue;
      }

      dialogueWord.Add(storyDialogue.allDialogue [i]);
    }
  }

  private void Update()
  {
    if (!_isDialoguePlaying && !_isEndOfDialogue)
    {
      _isDialoguePlaying = true;
      StartCoroutine (StartDialogue ());
    }

    if(_isEndOfDialogue)
    {
      if(/*Input.GetMouseButtonDown(0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
          EndingDialogue ();
        }
    }
  }
  
  public void EndingDialogue()
  {
    SceneManager.LoadScene("LoadScene");
    TemporaryData.GetInstance ().playerData.storyID ++;
  }

  private IEnumerator StartDialogue()
  {
    int dialogueLength = dialogueWord.Count;
    int currentDialogueIndex = 0;

    while (currentDialogueIndex < dialogueLength || !_isStringBeingReveled)
    {
      if (!_isStringBeingReveled) 
      {
        _isStringBeingReveled = true;
        StartCoroutine (DisplayString (dialogueWord [currentDialogueIndex++], currentDialogueIndex-1));

        if (currentDialogueIndex <= dialogueLength) 
        {
          _isEndOfDialogue = false;
        }
      }
      yield return 0;
    }
      
    while (true) 
    {
      if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
      {
        break;
      }

      yield return 0;
    }

    _isEndOfDialogue = true;
    _isDialoguePlaying = false;
  }

  private IEnumerator DisplayString(string stringToDisplay, int currentDialogueIndex)
  {
    int stringLength = stringToDisplay.Length;
    int currentCharacterIndex = 0;

    _textComponent.text = "";
   
    if (currentDialogueIndex == 0) 
    {
      if(dialogueBG[currentDialogueIndex] != "None")
      {
        BG.sprite = Resources.Load<Sprite> ("Image/StorySceneBG/" + dialogueBG[currentDialogueIndex]);
      }
      
      if(!string.IsNullOrEmpty(dialogueSpeaker [currentDialogueIndex]))
      {
        ShowCha (true, false, dialogueSpeaker [currentDialogueIndex], null);
        talkingCharacter.SetActive (true);
        talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
      }
      else
      {
        ShowCha (false, false, null, null);
        talkingCharacter.SetActive (false);
      }
      oldLeft = dialogueSpeaker [currentDialogueIndex];
    }
      
    else
    {
      if(dialogueBG[currentDialogueIndex] != "None")
      {
        BG.sprite = Resources.Load<Sprite> ("Image/StorySceneBG/" + dialogueBG[currentDialogueIndex]);
      }
      
      if (dialogueSpeaker [currentDialogueIndex] == oldLeft || dialogueSpeaker [currentDialogueIndex] == oldRight) 
      {
        if (dialogueSpeaker [currentDialogueIndex] == oldLeft && dialogueSpeaker [currentDialogueIndex] != oldRight)
        {
          if (!string.IsNullOrEmpty (oldRight)) 
          {
            ShowCha (true, true, oldLeft, oldRight);
            talkingCharacter.SetActive (true);
            talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          }
          else 
          {
            ShowCha (true, false, oldLeft, null);
            talkingCharacter.SetActive (true);
            talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          }
        }
        else if (dialogueSpeaker [currentDialogueIndex] != oldLeft && dialogueSpeaker [currentDialogueIndex] == oldRight)
        {
          if (!string.IsNullOrEmpty (oldLeft)) {
            ShowCha (true, true, oldLeft, oldRight);
            talkingCharacter.SetActive (true);
            talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          } 
          else
          {
            ShowCha (false, true, null, oldRight);
            talkingCharacter.SetActive (true);
            talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          }
        } 
      } 
      else if (dialogueSpeaker [currentDialogueIndex] != oldLeft && dialogueSpeaker [currentDialogueIndex] != oldRight) 
      {
        if (!string.IsNullOrEmpty (oldLeft))
        {
          if (!string.IsNullOrEmpty (oldRight)) 
          {
            ShowCha (false, false, null, null);
            oldLeft = null;
            oldRight = null;
            talkingCharacter.SetActive (false);
          } 
          else 
          {
            if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex]) != null) {
              ShowCha (true, true, oldLeft, dialogueSpeaker [currentDialogueIndex]);
              oldRight = dialogueSpeaker [currentDialogueIndex];
              talkingCharacter.SetActive (true);
              talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
            }
          }
        } 
        else 
        {
          if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex]) != null) {
            ShowCha (true, false, dialogueSpeaker [currentDialogueIndex], null);
            oldLeft = dialogueSpeaker [currentDialogueIndex];
            talkingCharacter.SetActive (true);
            talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          }
        }
      }
    }

    while (currentCharacterIndex < stringLength)
    {
      _textComponent.text += stringToDisplay [currentCharacterIndex];
      currentCharacterIndex++;

      if (currentCharacterIndex < stringLength) 
      {
        if (/*Input.GetMouseButton (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
          yield return new WaitForSeconds (SecondBetweenCharacters * CharacterRateMultiplier);
        }
        else
        {
          yield return new WaitForSeconds (SecondBetweenCharacters);
        }
      } 
      else
      {
        break;
      }
    }

    while (true)
    {
      if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
      {
        break;
      }

      yield return 0;
    }

    _isStringBeingReveled = false;
  }

  private void HideCha()
  {
    LeftCharacter.SetActive (false);
    RightCharacter.SetActive (false);
  }

  private void ShowCha(bool showLeft, bool showRight, string character1, string character2)
  {
    if (showLeft) 
    {
      LeftCharacter.SetActive (true);
      RightCharacter.SetActive (showRight);
      LeftCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + character1);
    } 
    else
    {
      LeftCharacter.SetActive (false);
    }

    if (showRight) 
    {
      RightCharacter.SetActive (true);
      LeftCharacter.SetActive (showLeft);
      RightCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + character2);
    } 
    else
    {
      RightCharacter.SetActive (false);
    }
  }
}
