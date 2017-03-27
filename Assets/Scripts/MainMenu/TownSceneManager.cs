using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownSceneManager : MonoBehaviour

{
  public GameObject shop;
  public GameObject library;

  public GameObject shopMain;
  public GameObject shopSup;
  public GameObject shopBg;

  public GameObject libraryMain;
  public GameObject libraryQuest;
  public GameObject libraryQuestDetail;

  public void Start()
  {
    if (PlayerPrefs.GetInt (Const.InTownScene, 1) == 1)
    {
      shop.SetActive (true);
      shopMain.SetActive (true);
      shopSup.SetActive (false);
      shopBg.SetActive (false);
    } 
    else
    {
      library.SetActive (true);
      libraryMain.SetActive (true);
      libraryQuest.SetActive (false);
      libraryQuestDetail.SetActive (false);
    }
  }

  public void BuyAndSellItem(string option)
  {
    shopMain.SetActive (false);
    shopSup.SetActive (true);
    shopBg.SetActive (true);
    if (option == "Buy") 
    {
      ItemManager.GetInstance ().GenerateBuyingItems ("Weapon");
    }
    else 
    {
    }
  }

  public void AcceptAndReportQuest(string option)
  {
    libraryMain.SetActive (false);
    libraryQuest.SetActive (true);
    libraryQuestDetail.SetActive (true);

    if (option == "Accept") 
    {
      
    }
    else 
    {
      
    }
  }

  public void ExitToMainScene()
  {
    SceneManager.LoadScene ("MainMenuScene");
  }

  public void HideSupMenu()
  {
    if (PlayerPrefs.GetInt (Const.InTownScene, 1) == 1)
    {
      shopMain.SetActive (true);
      shopSup.SetActive (false);
      shopBg.SetActive (false);
      libraryMain.SetActive (false);
      libraryQuest.SetActive (false);
      libraryQuestDetail.SetActive (false);
    }
    else 
    {
      libraryMain.SetActive (true);
      libraryQuest.SetActive (false);
      libraryQuestDetail.SetActive (false);
      shopMain.SetActive (false);
      shopSup.SetActive (false);
      shopBg.SetActive (false);
    }
  }
}
