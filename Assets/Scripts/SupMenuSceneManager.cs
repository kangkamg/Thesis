using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SupMenuSceneManager : MonoBehaviour 

{
  public GameObject selectedCharacterStatus;
  public GameObject allCharacterStatus;
  public GameObject changingEquip;
  public GameObject changingAbility;
  public GameObject party;
  public GameObject item;
  
  private bool isClicked = false;
  private int isClick = 0;

  private static SupMenuSceneManager _instance;
  public static SupMenuSceneManager GetInstance()
  {
    return _instance;
  }
  private void Awake()
  {
    _instance = this;
    if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "CharacterStatus")
    {
      allCharacterStatus.SetActive (true);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (false);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (true);
      item.SetActive (false);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Item") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (true);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Quest") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (false);
      changingAbility.SetActive (false);
    }
  }
  
  private void Start()
  {
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
      {
        StartCoroutine(ShowTutorial ());
      }
    }
  }

  public void ShowSelectedCharacterStatus()
  {
    selectedCharacterStatus.SetActive (true);
    allCharacterStatus.SetActive (false);
  }

  public void BackButton()
  {
    if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "CharacterStatus")
    {
      if (changingEquip.activeSelf)
      {
        selectedCharacterStatus.SetActive (true);
        changingEquip.SetActive (false);
        changingAbility.SetActive (false);
        CharacterStatusSceneManager.GetInstance().statusPage.GetComponent<ShowingCharacterStatusManager> ().UpdateStatus ();
      }
      else if (selectedCharacterStatus.activeSelf)
      {
        allCharacterStatus.SetActive (true);
        selectedCharacterStatus.SetActive (false);
        changingAbility.SetActive (false);
        CharacterStatusSceneManager.GetInstance().GenerateCharacter ();
      }
      else if (changingAbility.activeSelf)
      {
        selectedCharacterStatus.SetActive (true);
        changingAbility.SetActive (false);
        changingEquip.SetActive (false);
        CharacterStatusSceneManager.GetInstance().statusPage.GetComponent<ShowingCharacterStatusManager> ().UpdateStatus ();
      } 
      else if (allCharacterStatus.activeSelf)
      {
        SceneManager.LoadScene ("MainMenuScene");
      }
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
    {
      SceneManager.LoadScene ("MainMenuScene");
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Item") 
    {
      SceneManager.LoadScene ("MainMenuScene");
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Quest") 
    {
      SceneManager.LoadScene ("MainMenuScene");
    }
  }

  public void ChangingEquip()
  {
    selectedCharacterStatus.SetActive (false);
    changingEquip.SetActive (true);
  }
  
  public IEnumerator ShowTutorial()
  {
    GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
    handTouch.transform.SetParent (party.transform);
    handTouch.transform.localScale = Vector3.one;
    handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
    handTouch.transform.localPosition = new Vector2 (-150, 100);
    
    GameObject.Find ("Canvas").transform.GetChild (7).gameObject.SetActive (false);
    
    do 
    {
      if(isClick == 1)
      {
        handTouch.transform.localPosition = new Vector2 (-150, 50);
      }
      yield return null;
    } while(!isClicked);
    
    GameObject.Find ("Canvas").transform.GetChild (7).gameObject.SetActive (true);
    handTouch.transform.SetParent (GameObject.Find ("Canvas").transform.GetChild (7));
    handTouch.transform.localScale = Vector3.one;
    handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
    handTouch.transform.localPosition = new Vector2 (46.5f, -20.5f);
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      TemporaryData.GetInstance ().isTutorialDone = true;
    }
  }
      
  
  public void Clicked()
  {
    isClick+=1;
    
    
    if(isClick ==2)
    isClicked = true;
  }
}
