using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownSceneManager : MonoBehaviour

{
  public GameObject shop;

  public GameObject shopSup;
  public GameObject shopBg;

  public void Start()
  {
    shop.SetActive (true);
    shopSup.SetActive (false);
    shopBg.SetActive (false);
    if (PlayerPrefs.GetInt (Const.InTownScene, 0) == 0)
    {
      BuyAndSellItem ("Buy");
    } 
    else
    {
      BuyAndSellItem ("Sell");
    }
  }

  public void BuyAndSellItem(string option)
  {
    shopSup.SetActive (true);
    shopBg.SetActive (true);
    if (option == "Buy") 
    {
      ItemManager.GetInstance ().GenerateBuyingItems ("Heart");
      ItemManager.GetInstance ().isBuying = true;
    }
    else 
    {
      ItemManager.GetInstance ().GenerateInventoryItems ("Heart");
      ItemManager.GetInstance ().isBuying = false;
    }
  }

  public void ExitToMainScene()
  {
    SceneManager.LoadScene ("MainMenuScene");
  }
}
