using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoadingSceneManager : MonoBehaviour
{
  public Text _textComponent;
  public string loadingWord;
  private AsyncOperation async = null;
  
  private void Start()
  {
    StartCoroutine (LoadScene (loadingWord));
  }
  
  private IEnumerator LoadScene(string stringToDisplay)
  {    
    int stringLength = stringToDisplay.Length;
    int currentCharacterIndex = 7;

    _textComponent.text = "Loading";
    
    if (PlayerPrefs.GetString (Const.PreviousScene) == "MainMenuScene") 
    {
      if (GetTextAssetFile.GetInstance ().Load (Application.dataPath + "/Resources/DialogueFile/" + "D" + TemporaryData.GetInstance ().playerData.storyID + "M" + PlayerPrefs.GetInt (Const.MapNo, 0) + ".txt")) 
      {
        async = SceneManager.LoadSceneAsync ("StoryScene");
      } 
      else 
      {
        async = SceneManager.LoadSceneAsync ("GamePlayScene");
      }
    } 
    else if (PlayerPrefs.GetString (Const.PreviousScene) == "StoryScene")
    {
      if (PlayerPrefs.GetInt(Const.MapNo,0) == 0)
      {
        PlayerPrefs.SetInt (Const.MapNo, 1);
      }
      async = SceneManager.LoadSceneAsync ("GamePlayScene");
    } 
    else if (PlayerPrefs.GetString (Const.PreviousScene) == "GamePlayScene")
    {
      if (GetTextAssetFile.GetInstance ().Load (Application.dataPath + "/Resources/DialogueFile/" + "D" + TemporaryData.GetInstance ().playerData.storyID + "M" + PlayerPrefs.GetInt (Const.MapNo, 0) + ".txt")) 
      {
        async = SceneManager.LoadSceneAsync ("StoryScene");
      }
      else
      {
        async = SceneManager.LoadSceneAsync ("MainMenuScene");
      }
    } 

    while (!async.isDone) 
    {
      _textComponent.text += stringToDisplay [currentCharacterIndex];
      currentCharacterIndex++;

      if (currentCharacterIndex < stringLength) 
      {
        yield return new WaitForSeconds (1f);
      } 
      else 
      {
        currentCharacterIndex = 7;
        _textComponent.text = "Loading";
      }
    }
  }
}
