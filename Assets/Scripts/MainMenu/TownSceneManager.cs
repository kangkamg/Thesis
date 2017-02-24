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

  public void Start()
  {
    if (PlayerPrefs.GetInt (Const.InTownScene, 1) == 1) shop.SetActive (true);
    else library.SetActive (true);
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

  public void ExitToMainScene()
  {
    SceneManager.LoadScene ("MainMenuScene");
  }

  public void HideSupMenu()
  {
    shopMain.SetActive (true);
    shopSup.SetActive (false);
    shopBg.SetActive (false);
  }
}
