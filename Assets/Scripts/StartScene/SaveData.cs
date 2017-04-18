using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour

{
  public void SetSaveData(int i, int isNewGame)
  {
    if (isNewGame == 1)
    {
      if (SaveAndLoadPlayerData.CheckingSave (i))
      {
        PlayerData playedData = (PlayerData)SaveAndLoadPlayerData.LoadData (i);
        transform.GetChild (0).GetComponent<Text> ().text = "Continue This Save";
        transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = i.ToString ();
        transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = string.Format ("{0}:{1:00}:{2:00}", Mathf.RoundToInt (playedData.playedHrs / 3600), Mathf.RoundToInt ((playedData.playedHrs / 60) % 60), Mathf.RoundToInt ((int)playedData.playedHrs % 60));
        transform.GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = playedData.gold.ToString ();
      
        int averageLevel = 0;
        foreach (CharacterStatus a in playedData.characters.Where(x=>x.isInParty).ToList())
        {
          averageLevel += a.characterLevel;
        }
      
        transform.GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = (averageLevel / playedData.characters.Where (x => x.isInParty).Count ()).ToString ();
      
        transform.GetComponent<Button> ().onClick.AddListener (() => SelectedThisSave (playedData));
      } 
      else 
      {
        transform.GetChild (0).GetComponent<Text> ().text = "No Data";
        transform.GetChild (0).GetChild(0).GetComponent<Text> ().text = i.ToString();

        transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = "??:??:??";
        transform.GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = "??";
        transform.GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = "??";
        
        transform.GetComponent<Button> ().interactable = false;
      }
    }
    
    else
    {
      transform.GetChild (0).GetComponent<Text> ().text = "StartNewGame";
      transform.GetChild (0).GetChild(0).GetComponent<Text> ().text = i.ToString();
      
      transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = "??:??:??";
      transform.GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = "??";
      transform.GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = "??";
      
      transform.GetComponent<Button> ().onClick.AddListener (() => StartThisSave(i));
    }
  }
  
  public void SelectedThisSave(PlayerData data)
  {
    TemporaryData.GetInstance ().playerData = data;
    SceneManager.LoadScene ("MainMenuScene");
  }
  
  public void StartThisSave(int saveID)
  {
    StartSceneManager.InitFirstData ();
    TemporaryData.GetInstance ().playerData.id = saveID;
    SceneManager.LoadScene ("MainMenuScene");
  }
}
