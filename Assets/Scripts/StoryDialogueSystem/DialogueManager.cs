using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryDialogue
{
  public List<string> characterName = new List<string>();
  public List<string> allDialogue =  new List<string>();
}

public class DialogueManager : MonoBehaviour 
{
  public Text _textComponent;
  public GameObject dialogue;

  public StoryDialogue storyDialogue;
  public List<string> dialogueWord;
  public List<string>  dialogueSpeaker;

  public float SecondBetweenCharacters = 0.01f;
  public float CharacterRateMultiplier = 0.05f;

  private bool _isName = false;
  private bool _isStringBeingReveled = false;
  private bool _isDialoguePlaying = false;
  private bool _isEndOfDialogue = false;

  public GameObject LeftCharacter;
  public GameObject RightCharacter;

  public GameObject Talking1;
  public GameObject Talking2;

  public string oldLeft;
  public string oldRight;

  private void Start()
  {
    _textComponent.text = "";

    storyDialogue = GetDataFromSql.storyDialogue (PlayerPrefs.GetInt (Const.StoryNumber, 1));
     
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
      if(Input.GetMouseButtonDown(0))
        {
          PlayerPrefs.SetInt(Const.MapNo,PlayerPrefs.GetInt(Const.StoryNumber,1));
          SceneManager.LoadScene("GamePlayScene");
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
      if (Input.GetMouseButtonDown (0)) 
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
      ShowCha (true, false, dialogueSpeaker [currentDialogueIndex], null);
      oldLeft = dialogueSpeaker [currentDialogueIndex];
      Talking1.SetActive (true);
      Talking2.SetActive (false);
      Talking1.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
    }
      
    if (dialogueSpeaker[currentDialogueIndex] == oldLeft || dialogueSpeaker[currentDialogueIndex] == oldRight) 
    {
      if (dialogueSpeaker [currentDialogueIndex] == oldLeft && dialogueSpeaker [currentDialogueIndex] != oldRight)
      {
        if (!string.IsNullOrEmpty(oldRight)) 
        {
          ShowCha (true, true, oldLeft, oldRight);
          Talking1.SetActive (true);
          Talking2.SetActive (false);
          Talking1.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
        }
        else
        {
          ShowCha (true, false, oldLeft, null);
          Talking1.SetActive (true);
          Talking2.SetActive (false);
          Talking1.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
        }
      } 
      else if (dialogueSpeaker [currentDialogueIndex] != oldLeft && dialogueSpeaker [currentDialogueIndex] == oldRight)
      {
        if (!string.IsNullOrEmpty(oldLeft)) 
        {
          ShowCha (true, true, oldLeft, oldRight);
          Talking1.SetActive (false);
          Talking2.SetActive (true);
          Talking2.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
        }
        else
        {
          ShowCha (false, true, null, oldRight);
          Talking1.SetActive (false);
          Talking2.SetActive (true);
          Talking2.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
        }
      } 
    }
    else if (dialogueSpeaker [currentDialogueIndex] != oldLeft && dialogueSpeaker [currentDialogueIndex] != oldRight)
    {
      if (!string.IsNullOrEmpty(oldLeft)) 
      {
        if (!string.IsNullOrEmpty(oldRight)) 
        {
          ShowCha (false, false, null, null);
          oldLeft = null;
          oldRight = null;
          Talking1.SetActive (false);
          Talking2.SetActive (false);
        }
        else
        {
          if (Resources.Load<Sprite>("PlayerPrefab/"+dialogueSpeaker[currentDialogueIndex]) != null)
          {
            ShowCha (true, true, oldLeft, dialogueSpeaker [currentDialogueIndex]);
            oldRight = dialogueSpeaker [currentDialogueIndex];
            Talking1.SetActive (false);
            Talking2.SetActive (true);
            Talking2.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
          }
        }
      }
      else
      {
        if (Resources.Load<Sprite>("PlayerPrefab/"+dialogueSpeaker[currentDialogueIndex]) != null)
        {
          ShowCha (true, false, dialogueSpeaker [currentDialogueIndex], null);
          oldLeft = dialogueSpeaker [currentDialogueIndex];
          Talking1.SetActive (true);
          Talking2.SetActive (false);
          Talking1.GetComponentInChildren<Text> ().text = dialogueSpeaker [currentDialogueIndex];
        }
      }
    }

    while (currentCharacterIndex < stringLength)
    {
      _textComponent.text += stringToDisplay [currentCharacterIndex];
      currentCharacterIndex++;

      if (currentCharacterIndex < stringLength) 
      {
        if (Input.GetMouseButton (0)) 
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
      if (Input.GetMouseButtonDown (0)) 
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
      LeftCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + character1);
    } 
    else
    {
      LeftCharacter.SetActive (false);
    }

    if (showRight) 
    {
      RightCharacter.SetActive (true);
      LeftCharacter.SetActive (showLeft);
      RightCharacter.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + character2);
    } 
    else
    {
      RightCharacter.SetActive (false);
    }
  }
}
