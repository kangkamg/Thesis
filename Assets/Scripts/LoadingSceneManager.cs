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
      if (GetTextAssetFile.GetInstance ().Load ("D" + TemporaryData.GetInstance().playerData.storyID + "M" + PlayerPrefs.GetInt(Const.MapNo,0))) 
      {
        TemporaryData.GetInstance ().storyPlayingName = "D" + TemporaryData.GetInstance ().playerData.storyID + "M" + PlayerPrefs.GetInt (Const.MapNo, 0);
        async = SceneManager.LoadSceneAsync ("StoryScene");
      } 
      else 
      {
        if (string.IsNullOrEmpty (TemporaryData.GetInstance ().storyPlayingName)) 
        {
          async = SceneManager.LoadSceneAsync ("GamePlayScene");
        } 
        else 
        {
          if (GetTextAssetFile.GetInstance ().Load ( TemporaryData.GetInstance ().storyPlayingName)) 
          {
            async = SceneManager.LoadSceneAsync ("StoryScene");
          } 
        }
      }
    } 
    
    else if (PlayerPrefs.GetString (Const.PreviousScene) == "StoryScene")
    {
      if (PlayerPrefs.GetInt(Const.MapNo,0) == 0)
      {
        async = SceneManager.LoadSceneAsync ("MainMenuScene");
      }
      else
      {
        async = SceneManager.LoadSceneAsync ("GamePlayScene");
      } 
    } 
    
    else if (PlayerPrefs.GetString (Const.PreviousScene) == "GamePlayScene")
    {
      if (TemporaryData.GetInstance ().playerData.passedMap.Where (x => x == PlayerPrefs.GetInt (Const.MapNo)).Count () > 0) 
      {
        if (GetTextAssetFile.GetInstance ().Load ("D" + TemporaryData.GetInstance ().playerData.storyID + "M" + PlayerPrefs.GetInt (Const.MapNo, 0))) 
        {
          TemporaryData.GetInstance().storyPlayingName = "D" + TemporaryData.GetInstance ().playerData.storyID + "M" + PlayerPrefs.GetInt (Const.MapNo, 0);
          PlayerPrefs.SetInt (Const.MapNo, 0);
          async = SceneManager.LoadSceneAsync ("StoryScene");
        }
        else
        {
          async = SceneManager.LoadSceneAsync ("MainMenuScene");
        } 
      }
      else
      {
        async = SceneManager.LoadSceneAsync ("MainMenuScene");
      }
    } 

    else if (PlayerPrefs.GetString (Const.PreviousScene) == "StartScene")
    {
      if (TemporaryData.GetInstance().storyPlayingName == "Tutorial") 
      {
        async = SceneManager.LoadSceneAsync ("StoryScene");
      }
      else
      {
        async = SceneManager.LoadSceneAsync ("MainMenuScene");
      }
    } 

    async.allowSceneActivation = false;

    while (!async.isDone) 
    {
      if (async.progress <= 0.8f) 
      {
        _textComponent.text += stringToDisplay [currentCharacterIndex];
        currentCharacterIndex++;
        if (currentCharacterIndex < stringLength) 
        {
          yield return new WaitForSeconds (0.125f);
        } 
        else 
        {
          currentCharacterIndex = 7;
          _textComponent.text = "Loading";
        }
      } 
      else 
      {
        _textComponent.text = "Complete";
        yield return new WaitForSeconds (0.5f);
        async.allowSceneActivation = true;
      }
    }
  }
}
