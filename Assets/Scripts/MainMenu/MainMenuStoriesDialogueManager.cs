using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class MainMenuStoriesDialogueManager : MonoBehaviour 
{
  public Text _textComponent;
  Transform BackButton;
  
  public StoryDialogue storyDialogue;
  public string _storiesName;
  
  private bool _isStringBeingReveled = false;
  private bool _isDialoguePlaying = false;
  private bool _isEndOfDialogue = false;
  
  public float SecondBetweenCharacters = 0.01f;
  
  int currentDialogueIndex = 0;
  int dialogueLength = 0;
  
  bool isClickNext;
  
  public void InitDialogue(string storiesName, Transform backButton)
  {
    BackButton = backButton;
    
    BackButton.gameObject.SetActive (false);
    _textComponent.text = "";
    
    _storiesName = storiesName;
    storyDialogue = GetTextAssetFile.GetInstance().LoadText(_storiesName);
    
    _isEndOfDialogue = false;
    _isDialoguePlaying = false;
    
    CheckingDialogueEnding ();
  }
  
  private void CheckingDialogueEnding()
  {
    if (!_isDialoguePlaying && !_isEndOfDialogue) 
    {
      _isDialoguePlaying = true;
      StartCoroutine (StartDialogue (storyDialogue.allDialogue));
    }
  }
  
  private IEnumerator StartDialogue(List<string> dialogueWord)
  {
    dialogueLength = dialogueWord.Count;
    
    currentDialogueIndex = 0;
    
    while (currentDialogueIndex < dialogueLength || !_isStringBeingReveled)
    {
      if(dialogueWord[currentDialogueIndex] == "AddItem")
      {
        break;
      }
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
        if (TemporaryData.GetInstance ().playerData.readedStories.Where (x => x == _storiesName).Count () <= 0) 
        {
          AddItem (GetDataFromSql.GetReward (_storiesName));
          break;
        }
        else
        {
          BackButton.gameObject.SetActive (true);
          break;
        }
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

    while (currentCharacterIndex < stringLength)
    {
      _textComponent.text += stringToDisplay [currentCharacterIndex];
      currentCharacterIndex++;

      if (currentCharacterIndex < stringLength) 
      {
        if (/*Input.GetMouseButtonDown (0)*/Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
          yield return new WaitForSeconds (SecondBetweenCharacters * 0.1f);
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
  
  private void AddItem(List<int> addItem)
  {
    TemporaryData.GetInstance ().playerData.readedStories.Add (_storiesName);
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
      
      GameObject dialogBox = GameObject.Instantiate(DialogBoxManager.GetInstance ().GenerateDialogBox ("You recieve new items", false));
      dialogBox.transform.SetParent (transform.parent);
      dialogBox.transform.localScale = Vector3.one;
      dialogBox.transform.localPosition = Vector2.zero;
      dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => OkDialogBox(dialogBox));
    }
  }
  
  private void OkDialogBox(GameObject dialogBox)
  {
    dialogBox.SetActive (false);
    BackButton.gameObject.SetActive (true);
  }
}
