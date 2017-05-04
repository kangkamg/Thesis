using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour

{
  public int _isNewgame;
  
  public void SetSaveData(int i, int isNewGame)
  {
    _isNewgame = isNewGame;
    if (_isNewgame == 1)
    {
      if (SaveAndLoadPlayerData.CheckingSave (i))
      {
        PlayerData playedData = (PlayerData)SaveAndLoadPlayerData.LoadData (i);
        transform.GetChild (0).GetComponent<Text> ().text = i.ToString ();
        transform.GetChild (1).GetChild (0).GetComponent<Text> ().text = string.Format ("{0}:{1:00}:{2:00}", Mathf.RoundToInt (playedData.playedHrs / 3600), Mathf.RoundToInt ((playedData.playedHrs / 60) % 60), Mathf.RoundToInt ((int)playedData.playedHrs % 60));
        transform.GetChild (2).GetChild (0).GetComponent<Text> ().text = playedData.gold.ToString ();
      
        int averageLevel = 0;
        foreach (CharacterStatus a in playedData.characters.Where(x=>x.isInParty).ToList())
        {
          averageLevel += a.characterLevel;
        }
      
        transform.GetChild (3).GetChild (0).GetComponent<Text> ().text = (averageLevel / playedData.characters.Where (x => x.isInParty).Count ()).ToString ();
      
        transform.GetComponent<Button> ().onClick.AddListener (() => SelectedThisSave (playedData));
      } 
      else 
      {
        transform.GetChild (4).gameObject.SetActive (true);
        transform.GetChild (4).GetComponent<Text> ().text = "No Data.";
        
        transform.GetChild (0).gameObject.SetActive (false);
        transform.GetChild (1).gameObject.SetActive (false);
        transform.GetChild (2).gameObject.SetActive (false);
        transform.GetChild (3).gameObject.SetActive (false);
        
        transform.GetComponent<Button> ().interactable = false;
      }
    }
    
    else
    {
      if (SaveAndLoadPlayerData.CheckingSave (i))
      {
        PlayerData playedData = (PlayerData)SaveAndLoadPlayerData.LoadData (i);
        transform.GetChild (0).GetComponent<Text> ().text = i.ToString ();
        transform.GetChild (1).GetChild (0).GetComponent<Text> ().text = string.Format ("{0}:{1:00}:{2:00}", Mathf.RoundToInt (playedData.playedHrs / 3600), Mathf.RoundToInt ((playedData.playedHrs / 60) % 60), Mathf.RoundToInt ((int)playedData.playedHrs % 60));
        transform.GetChild (2).GetChild (0).GetComponent<Text> ().text = playedData.gold.ToString ();

        int averageLevel = 0;
        foreach (CharacterStatus a in playedData.characters.Where(x=>x.isInParty).ToList())
        {
          averageLevel += a.characterLevel;
        }

        transform.GetChild (3).GetChild (0).GetComponent<Text> ().text = (averageLevel / playedData.characters.Where (x => x.isInParty).Count ()).ToString ();

        transform.GetComponent<Button> ().onClick.AddListener (() => StartThisSave(i));
      } 
      else 
      {
        transform.GetChild (4).gameObject.SetActive (true);
        transform.GetChild (4).GetComponent<Text> ().text = "No Data.";

        transform.GetChild (0).gameObject.SetActive (false);
        transform.GetChild (1).gameObject.SetActive (false);
        transform.GetChild (2).gameObject.SetActive (false);
        transform.GetChild (3).gameObject.SetActive (false);

        transform.GetComponent<Button> ().onClick.AddListener (() => StartThisSave(i));
      }
    }
  }
  
  public void SelectedThisSave(PlayerData data)
  {
    TemporaryData.GetInstance ().isTutorialDone = true;
    transform.SetAsFirstSibling ();
    foreach (Transform child in transform.parent) 
    {
      if (child != transform) 
      {
        Destroy (child.gameObject);
      }
    }
    
    transform.parent.GetChild (0).GetComponent<Button> ().interactable = false;
    transform.parent.parent.GetChild (2).gameObject.SetActive (false);
    
    GameObject dialogBox = GameObject.Instantiate (DialogBoxManager.GetInstance ().GenerateDialogBox ("Continue this save ?", true));

    dialogBox.transform.SetParent (transform.parent.parent);
    dialogBox.transform.localScale = Vector3.one;
    dialogBox.transform.localPosition = Vector2.zero;
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => DialogBoxManager.GetInstance().AddChangeScene ("MainMenuScene"));
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => OkDialogBox(data));
    dialogBox.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => CancelDialogBox (dialogBox));
  }
  
  public void StartThisSave(int saveID)
  {
    TemporaryData.GetInstance ().isTutorialDone = false;
    
    StartSceneManager.InitFirstData ();
    TemporaryData.GetInstance ().playerData.id = saveID;
    TemporaryData.GetInstance ().playerData.storyID = 0;
    TemporaryData.GetInstance ().storyPlayingName = "Tutorial";
    
    if (SaveAndLoadPlayerData.CheckingSave (saveID))
    {
      transform.SetAsFirstSibling ();
      foreach (Transform child in transform.parent) 
      {
        if (child != transform) 
        {
          Destroy (child.gameObject);
        }
      }
      
      transform.parent.GetChild (0).GetComponent<Button> ().interactable = false;
      transform.parent.parent.GetChild (2).gameObject.SetActive (false);
      
      GameObject dialogBox = GameObject.Instantiate (DialogBoxManager.GetInstance ().GenerateDialogBox ("Overwrite this save ?", true));
      
      dialogBox.transform.SetParent (transform.parent.parent);
      dialogBox.transform.localScale = Vector3.one;
      dialogBox.transform.localPosition = Vector2.zero;
      dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => DialogBoxManager.GetInstance().AddChangeScene ("StoryScene"));
      dialogBox.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => CancelDialogBox (dialogBox));
    } 
    else
    {
      SceneManager.LoadScene ("StoryScene");
    }
  }
  
  public void OkDialogBox(PlayerData data)
  {
    TemporaryData.GetInstance().playerData = data;
    TemporaryData.GetInstance ().storyPlayingName = null;
  }
  
  public void CancelDialogBox(GameObject dialogBox)
  {
    transform.parent.parent.GetChild (2).gameObject.SetActive (true);
    Destroy (dialogBox);
    StartSceneManager.GetInstance().CreateSaveIndex(_isNewgame);
  }
}
