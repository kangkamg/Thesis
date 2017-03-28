using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
  public GameObject playerMenu;
  public GameObject dungeonMenu;
  public GameObject townMenu;
  public GameObject bgMenu;
  public GameObject optionMenu;

  public Button showMenuBT;
  public bool menuOn = false;

  public void Start()
  {
    playerMenu.SetActive (false);
    dungeonMenu.SetActive (false);
    townMenu.SetActive (false);
    bgMenu.SetActive (false);
    optionMenu.SetActive (false);
    menuOn = false;
  }
  public void ShowMenu()
  {
    if (menuOn) 
    {
      playerMenu.SetActive (false);
      menuOn = false;
      showMenuBT.transform.localPosition = new Vector3 (0, -50, 0);
    } 
    else
    {
      playerMenu.SetActive(true);
      menuOn = true;
      showMenuBT.transform.localPosition = new Vector3 (0, 88, 0);
    }
  }

  public void ShowTownMenu(int townSceneNumber)
  {
    townMenu.SetActive (true);
    bgMenu.SetActive (true);
    PlayerPrefs.SetInt (Const.TownSceneNo, townSceneNumber);
    playerMenu.SetActive (false);
    menuOn = false;
    showMenuBT.transform.localPosition = new Vector3 (0, -50, 0);
  }

  public void ShowDungeonMenu(int mapNumber)
  {
    dungeonMenu.SetActive (true);
    bgMenu.SetActive (true);
    PlayerPrefs.SetInt (Const.MapNo, mapNumber);
    playerMenu.SetActive (false);
    menuOn = false;
    showMenuBT.transform.localPosition = new Vector3 (0, -50, 0);
  }
    
  public void ShowOptionMenu()
  {
    optionMenu.SetActive (true);
    bgMenu.SetActive (true);
  }

  public void GoPlayScene()
  {
    SceneManager.LoadScene ("GamePlayScene");
  }

  public void GoInTownScene(int inTownSceneNumber)
  {
    SceneManager.LoadScene ("InTownScene");
    PlayerPrefs.SetInt (Const.InTownScene, inTownSceneNumber);
  }

  public void GoToStartScene()
  {
    SceneManager.LoadScene ("StartScene");
  }

  public void GoToSupMenuScene(string supmenu)
  {
    SceneManager.LoadScene ("SupMenuScene");
    PlayerPrefs.SetString (Const.OpenSupMenuScene, supmenu);
  }

  public void ExitGame()
  {
    Application.Quit ();
  }

  public void Exit()
  {
    townMenu.SetActive (false);
    dungeonMenu.SetActive (false);
    optionMenu.SetActive (false);
    bgMenu.SetActive (false);
  }
}
