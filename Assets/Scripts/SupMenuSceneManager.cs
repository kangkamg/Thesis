using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SupMenuSceneManager : MonoBehaviour 

{
  public GameObject selectedCharacterStatus;
  public GameObject allCharacterStatus;
  public GameObject changingEquip;
  public GameObject party;
  public GameObject changingParty;
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
      changingParty.SetActive (false);
      item.SetActive (false);
      quest.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (true);
      changingParty.SetActive (false);
      item.SetActive (false);
      quest.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Item") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      changingParty.SetActive (false);
      item.SetActive (true);
      quest.SetActive (false);
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Quest") 
    {
      allCharacterStatus.SetActive (false);
      selectedCharacterStatus.SetActive (false);
      changingEquip.SetActive (false);
      party.SetActive (false);
      changingParty.SetActive (false);
      item.SetActive (false);
      quest.SetActive (true);
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
        CharacterStatusSceneManager.GetInstance().statusPage.GetComponent<ShowingCharacterStatusManager> ().UpdateStatus ();
      }
      else if (selectedCharacterStatus.activeSelf)
      {
        allCharacterStatus.SetActive (true);
        selectedCharacterStatus.SetActive (false);
      }
      else if (allCharacterStatus.activeSelf)
      {
        SceneManager.LoadScene ("MainMenuScene");
      }
    }
    else if (PlayerPrefs.GetString (Const.OpenSupMenuScene, "CharacterStatus") == "Party") 
    {
      if (changingParty.activeSelf)
      {
        party.SetActive (true);
        changingParty.SetActive (false);
      }
      else
      {
        SceneManager.LoadScene ("MainMenuScene");
      }
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
    
  public void ChangingParty()
  {
    party.SetActive (false);
    changingParty.SetActive (true);
  }
}
