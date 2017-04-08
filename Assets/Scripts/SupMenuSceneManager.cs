using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SupMenuSceneManager : MonoBehaviour 

{
  public GameObject selectedCharacterStatus;
  public GameObject allCharacterStatus;
  public GameObject changingEquip;
  public GameObject changingAbility;
  public GameObject party;
  public GameObject item;
  public GameObject quest;

  private void Awake()
  {
    if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "CharacterStatus")
    {
      allCharacterStatus.SetActive (true);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (false);
      quest.SetActive (false);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (true);
      item.SetActive (false);
      quest.SetActive (false);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Item") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (true);
      quest.SetActive (false);
      changingAbility.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Quest") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      item.SetActive (false);
      quest.SetActive (true);
      changingAbility.SetActive (false);
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
}
