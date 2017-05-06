using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

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
  public List<string> dialogueWord = new List<string>();
  public List<string> dialogueSpeaker = new List<string>();
  public List<string> dialogueBG = new List<string>();
  public List<string> addingCharacter = new List<string>();
  public List<int> addItem = new List<int>();

  public float SecondBetweenCharacters = 0.01f;
  public float CharacterRateMultiplier = 0.05f;

  private bool _isName = false;
  private bool _isStringBeingReveled = false;
  private bool _isDialoguePlaying = false;
  private bool _isEndOfDialogue = false;

  public GameObject LeftCharacter;
  public GameObject RightCharacter;

  public GameObject talkingCharacter;

  public string oldLeft = null;
  public string oldRight = null;

  private void Start()
  { 
    StartCoroutine (SystemManager.IncreasePlayedHrs ());
    
    _textComponent.text = "";

    string _dialoguePath = TemporaryData.GetInstance ().storyPlayingName;
    
    storyDialogue = GetTextAssetFile.GetInstance().LoadText(_dialoguePath);
      
    dialogueWord = storyDialogue.allDialogue;
    dialogueSpeaker = storyDialogue.characterName;
    dialogueBG = storyDialogue.bgName;
    
    for(int i = 0; i < dialogueWord.Count;i++)
    {
      if (dialogueWord[i] == "AddCharacter")
      {
        string party = "";
        
        if (dialogueWord [i + 4] == "false") 
        {
          party = " To Your Party";
        }
        else
        {
          party = " To Your Team";
        }
        
        addingCharacter.Add (dialogueWord [i + 2]);
        addingCharacter.Add (dialogueWord [i + 3]);
        addingCharacter.Add (dialogueWord [i + 4]);
        addingCharacter.Add (dialogueWord [i + 5]);
        addingCharacter.Add (dialogueWord[i + 6]);
        
        dialogueWord.Add ("Adding " +  dialogueWord [i + 1] + party);
        dialogueSpeaker.Add ("");
        dialogueBG.Add ("");
        
        
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        break;
      }
      else if (dialogueWord [i] == "AddCharacter(FirstTime)")
      {
        string name = dialogueWord [i + 1];
        
        addingCharacter.Add (dialogueWord [i + 2]);
        addingCharacter.Add (dialogueWord [i + 3]);
        addingCharacter.Add (dialogueWord [i + 4]);
        addingCharacter.Add (dialogueWord [i + 5]);
        addingCharacter.Add (dialogueWord [i + 6]);

        dialogueWord.Add ("Adding " + dialogueWord[i+1] + " In your teams go to add to your party in informations->party");
        dialogueSpeaker.Add ("");
        dialogueBG.Add ("");
        
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        dialogueWord.Remove (dialogueWord [i]);
        break;
      }
      else if (dialogueWord [i] == "AddItem")
      {
        dialogueWord.Remove (dialogueWord [i]);
        
        addItem = GetDataFromSql.GetReward (_dialoguePath);
        dialogueWord.Add ("You recieve new items");
        dialogueSpeaker.Add ("");
        dialogueBG.Add ("");
        break;
      }
      else if (dialogueWord [i] == "Ending")
      {
        
        dialogueWord.Remove (dialogueWord [i]);
        
        dialogueWord.Add ("You recieve new story book");
        dialogueSpeaker.Add ("");
        dialogueBG.Add ("");
        break;
      }
      else if (dialogueWord [i] == "Ending(FirstTime)")
      {
        
        dialogueWord.Remove (dialogueWord [i]);
        
        dialogueWord.Add ("You recieve new story book go to look in informations->storybook");
        dialogueSpeaker.Add ("");
        dialogueBG.Add ("");
        break;
      }
    }    
     
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene ().name);
  }

  private void Update()
  {
    if (!_isDialoguePlaying && !_isEndOfDialogue)
    {
      _isDialoguePlaying = true;
      StartCoroutine (StartDialogue ());
    }
  }
  
  public void EndingDialogue()
  {
    if(PlayerPrefs.GetString(Const.PreviousScene) == "GamePlayScene") PlayerPrefs.SetInt (Const.MapNo, 0); 
    SceneManager.LoadScene("LoadScene");
    if(TemporaryData.GetInstance().storyPlayingName.Contains("D")) TemporaryData.GetInstance ().playerData.storyID ++;
    TemporaryData.GetInstance ().storyPlayingName = null;
    
    if (addingCharacter.Count > 0) 
    {
      List<int> equipItemID = new List<int> ();
      equipItemID.Add (int.Parse(addingCharacter [3]));
      equipItemID.Add (int.Parse(addingCharacter [4]));
      
      bool isInParty = false;

      if (addingCharacter [2] == "false") 
      {
        isInParty = false;
      }
      else
      {
        isInParty = true;
      }
      
      SystemManager.AddCharacterToParty (int.Parse (addingCharacter [0]), equipItemID, int.Parse (addingCharacter [1]), isInParty);
    }
    
    if (addItem.Count > 0) 
    {
      for (int i = 0; i < addItem.Count; i++) 
      {
        Item adding = new Item ();
        adding.item = GetDataFromSql.GetItemFromID (addItem [i]);
        adding.equiped = false;
        adding.ordering = TemporaryData.GetInstance ().playerData.inventory.Count - 1;
        TemporaryData.GetInstance ().playerData.inventory.Add (adding);
      }
    }
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
      if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !_isStringBeingReveled) 
      {
        break;
      }

      yield return 0;
    }

    _isEndOfDialogue = true;
    _isDialoguePlaying = false;
    EndingDialogue ();
  }

  private IEnumerator DisplayString(string stringToDisplay, int currentDialogueIndex)
  {
    int stringLength = stringToDisplay.Length;
    int currentCharacterIndex = 0;

    _textComponent.text = "";
   
    ShowCha (false, false, null, null);
    LeftCharacter.GetComponent<Image> ().color = Color.white;
    RightCharacter.GetComponent<Image> ().color = Color.white;
    talkingCharacter.SetActive (false);

    if (currentDialogueIndex == 0) 
    {
      if (dialogueBG.Count > 0) 
      {
        if (dialogueBG [currentDialogueIndex] != "None" || !string.IsNullOrEmpty(dialogueBG [currentDialogueIndex]) )
        {
          BG.sprite = Resources.Load<Sprite> ("Image/StoryScene/" + dialogueBG [currentDialogueIndex]);
        }
      }
      if(dialogueSpeaker.Count > 0)
      {
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
    }
    else
    {
      if (dialogueBG.Count > 0) 
      {
        if (dialogueBG [currentDialogueIndex] != "None" || !string.IsNullOrEmpty(dialogueBG [currentDialogueIndex]))
        {
          BG.sprite = Resources.Load<Sprite> ("Image/StoryScene/" + dialogueBG [currentDialogueIndex]);
        }
      }
      
      if (dialogueSpeaker.Count > 0) 
      {
        if (dialogueSpeaker[currentDialogueIndex].Contains ("(Alone)")) 
        {
          if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace ("(Alone)", null)) != null) 
          {
            ShowCha (true, false, dialogueSpeaker [currentDialogueIndex].Replace ("(Alone)", null), null);
          }
          oldLeft = dialogueSpeaker [currentDialogueIndex].Replace ("(Alone)", null);
          oldRight = null;
          talkingCharacter.SetActive (true);
          talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex].Replace ("(Alone)", null);
        } 
        else 
        {
          if (string.IsNullOrEmpty (dialogueSpeaker [currentDialogueIndex])) 
          {
            ShowCha (false, false, null, null);
            talkingCharacter.SetActive (false);
            oldLeft = "";
            oldRight = "";
          } 
          else
          {
            if (dialogueSpeaker [currentDialogueIndex].Contains("Left")) 
            {
              if (dialogueSpeaker [currentDialogueIndex].Replace("Left",null) == oldLeft) 
              {
                if (string.IsNullOrEmpty (oldRight)) 
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null)
                  {
                    ShowCha (true, false, oldLeft, null);
                  } 
                  else
                  {
                    ShowCha (false, false, oldLeft, null);
                  }
                }
                else
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (true, true, oldLeft, oldRight);
                  } 
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (false, true, oldLeft, oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (true, false, oldLeft, oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (false, false, oldLeft, oldRight);
                  }
                }
                LeftCharacter.GetComponent<Image> ().color = Color.white;
                RightCharacter.GetComponent<Image> ().color = Color.gray;
                talkingCharacter.SetActive (true);
                talkingCharacter.GetComponentInChildren<Text> ().text = oldLeft;
              }
              else
              {
                if (string.IsNullOrEmpty (oldRight)) 
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Left",null)) != null)
                  {
                    ShowCha (true, false, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , null);
                  } 
                  else
                  {
                    ShowCha (false, false, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , null);
                  }
                }
                else
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Left",null)) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (true, true, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , oldRight);
                  } 
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Left",null)) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (false, true, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Left",null)) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (true, false, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Left",null)) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (false, false, dialogueSpeaker [currentDialogueIndex].Replace("Left",null) , oldRight);
                  }
                }
                LeftCharacter.GetComponent<Image> ().color = Color.white;
                RightCharacter.GetComponent<Image> ().color = Color.gray;
                oldLeft = dialogueSpeaker [currentDialogueIndex].Replace ("Left", null);
                talkingCharacter.SetActive (true);
                talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex].Replace("Left",null) ;
              }
            } 
            else if (dialogueSpeaker [currentDialogueIndex].Contains("Right")) 
            {
              if (dialogueSpeaker [currentDialogueIndex].Replace("Right",null) == oldRight) 
              {
                if (string.IsNullOrEmpty (oldLeft)) 
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + oldRight) != null)
                  {
                    ShowCha (false, true, null, oldRight);
                  } 
                  else
                  {
                    ShowCha (false, false, null, oldRight);
                  }
                }
                else
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (true, true, oldLeft, oldRight);
                  } 
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) != null )
                  {
                    ShowCha (false, true, oldLeft, oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (true, false, oldLeft, oldRight);
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null && Resources.Load<Sprite> ("Image/Character/" + oldRight) == null )
                  {
                    ShowCha (false, false, oldLeft, oldRight);
                  }
                }
                LeftCharacter.GetComponent<Image> ().color = Color.gray;
                RightCharacter.GetComponent<Image> ().color = Color.white;
                talkingCharacter.SetActive (true);
                talkingCharacter.GetComponentInChildren<Text> ().text = oldRight;
              }
              else
              {
                if (string.IsNullOrEmpty (oldLeft)) 
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Right",null)) != null)
                  {
                    ShowCha (false, true, null, dialogueSpeaker [currentDialogueIndex].Replace("Right",null));
                  } 
                  else
                  {
                    ShowCha (false, false, null, dialogueSpeaker [currentDialogueIndex].Replace("Right",null));
                  }
                }
                else
                {
                  if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Right",null)) != null && Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null )
                  {
                    ShowCha (true, true, oldLeft, dialogueSpeaker [currentDialogueIndex].Replace("Right",null) );
                  } 
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Right",null)) == null && Resources.Load<Sprite> ("Image/Character/" + oldLeft) != null )
                  {
                    ShowCha (true, false, oldLeft , dialogueSpeaker [currentDialogueIndex].Replace("Right",null) );
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Right",null)) != null && Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null )
                  {
                    ShowCha (false, true, oldLeft , dialogueSpeaker [currentDialogueIndex].Replace("Right",null) );
                  }
                  else if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex].Replace("Right",null)) == null && Resources.Load<Sprite> ("Image/Character/" + oldLeft) == null )
                  {
                    ShowCha (false, false, oldLeft, dialogueSpeaker [currentDialogueIndex].Replace("Right",null) );
                  }
                }
                LeftCharacter.GetComponent<Image> ().color = Color.gray;
                RightCharacter.GetComponent<Image> ().color = Color.white;
                oldRight = dialogueSpeaker [currentDialogueIndex].Replace ("Right", null);
                talkingCharacter.SetActive (true);
                talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex].Replace("Right",null) ;
              }
              }
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
        if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
          _textComponent.text = stringToDisplay;
          currentCharacterIndex = stringLength;
          yield return new WaitForSeconds (0.5f);
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
      if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && currentCharacterIndex == stringLength) 
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
      if(character1 == "???" || Resources.Load<Sprite> ("Image/Character/" + character1) == null )
      {
        LeftCharacter.SetActive (false);
        RightCharacter.SetActive (showRight);
      }
      else
      {
        LeftCharacter.SetActive (true);
        RightCharacter.SetActive (showRight);
        LeftCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + character1);
      }
    } 
    else
    {
      LeftCharacter.SetActive (false);
    }

    if (showRight) 
    {
      if(character2 == "???" || Resources.Load<Sprite> ("Image/Character/" + character2) == null )
      {
        LeftCharacter.SetActive (showLeft);
        RightCharacter.SetActive (false);
      }
      else
      {
        RightCharacter.SetActive (true);
        LeftCharacter.SetActive (showLeft);
        RightCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + character2);
      }
    } 
    else
    {
      RightCharacter.SetActive (false);
    }
  }
}
