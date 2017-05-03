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
  public List<string> dialogueWord;
  public List<string> dialogueSpeaker;
  public List<string> dialogueBG;
  public List<string> addingCharacter;
  public List<int> addItem;

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
    _textComponent.text = "";
     
    string _dialoguePath = TemporaryData.GetInstance ().storyPlayingName;
    
    storyDialogue = GetTextAssetFile.GetInstance().LoadText(_dialoguePath);
      
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
      
      if (storyDialogue.allDialogue [i] == "AddCharacter")
      {
        string party = "";
        
        if (storyDialogue.allDialogue [i + 4] == "false") 
        {
          party = " To Your Party";
        }
        else
        {
          party = " To Your Team";
        }
        
        addingCharacter.Add (storyDialogue.allDialogue [i + 2]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 3]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 4]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 5]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 6]);
        
        dialogueWord.Add ("Adding " + storyDialogue.allDialogue [i + 1] + party);
        
        break;
      }
      else if (storyDialogue.allDialogue [i] == "AddCharacter(FirstTime)")
      {
        string party = "";

        if (storyDialogue.allDialogue [i + 4] == "false") 
        {
          party = " To Your Party";
        }
        else
        {
          party = " To Your Team";
        }

        addingCharacter.Add (storyDialogue.allDialogue [i + 2]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 3]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 4]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 5]);
        addingCharacter.Add (storyDialogue.allDialogue [i + 6]);

        dialogueWord.Add ("Adding " + storyDialogue.allDialogue [i + 1] + " In your teams go to add to your party in informations->party");

        break;
      }
      else if (storyDialogue.allDialogue [i] == "AddItem")
      {
        addItem = GetDataFromSql.GetReward (_dialoguePath);
        dialogueWord.Add ("You recieve new items");

        continue;
      }
      else if (storyDialogue.allDialogue [i] == "Ending")
      {
        dialogueWord.Add ("You recieve new story book");

        continue;
      }
      else if (storyDialogue.allDialogue [i] == "Ending(FirstTime)")
      {
        dialogueWord.Add ("You recieve new story book go to look in informations->storybook");

        continue;
      }
      

      dialogueWord.Add(storyDialogue.allDialogue [i]);
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
    talkingCharacter.SetActive (false);
    BG.sprite = null;

    if (currentDialogueIndex == 0) 
    {
      if (dialogueBG.Count > 0) 
      {
        string bg = dialogueBG [currentDialogueIndex];
        
        if (bg != "None" || !string.IsNullOrEmpty(bg) )
        {
          
          if(Resources.Load<Sprite> ("Image/StoryScene/" + bg)!=null) Debug.Log("hello");
          
          BG.sprite = Resources.Load<Sprite> ("Image/StoryScene/" + bg);
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
        string bg = dialogueBG [currentDialogueIndex];
        
        if (bg != "None" || !string.IsNullOrEmpty(bg))
        {
          BG.sprite = Resources.Load<Sprite> ("Image/StoryScene/" + bg);
        }
      }
      
      if (dialogueSpeaker.Count > 0) {
        if (dialogueSpeaker [currentDialogueIndex] == oldLeft || dialogueSpeaker [currentDialogueIndex] == oldRight) 
        {
          if (dialogueSpeaker [currentDialogueIndex] == oldLeft && dialogueSpeaker [currentDialogueIndex] != oldRight) 
          {
            if (!string.IsNullOrEmpty (oldRight)) {
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
          } else if (dialogueSpeaker [currentDialogueIndex] != oldLeft && dialogueSpeaker [currentDialogueIndex] == oldRight) 
          {
            if (!string.IsNullOrEmpty (oldLeft))
            {
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
              if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex]) != null) 
              {
                ShowCha (true, true, oldLeft, dialogueSpeaker [currentDialogueIndex]);
                oldRight = dialogueSpeaker [currentDialogueIndex];
              }
              talkingCharacter.SetActive (true);
              talkingCharacter.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
            }
          } 
          else
          {
            if (Resources.Load<Sprite> ("Image/Character/" + dialogueSpeaker [currentDialogueIndex]) != null)
            {
              ShowCha (true, false, dialogueSpeaker [currentDialogueIndex], null);
              oldLeft = dialogueSpeaker [currentDialogueIndex];
            }
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
