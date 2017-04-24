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
    
    if (TemporaryData.GetInstance ().allStory.Where (x => x.ID == TemporaryData.GetInstance ().playerData.storyID && x.mapNo == PlayerPrefs.GetInt (Const.MapNo, 0)).Count() > 0)
    {
      async = SceneManager.LoadSceneAsync ("StoryScene");
    } 
    else 
    {
      async = SceneManager.LoadSceneAsync ("GamePlayScene");
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
