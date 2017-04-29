using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
  public GameObject playerMenu;
  public GameObject dungeonMenu;
  public GameObject townMenu;
  public GameObject bgMenu;
  public GameObject mainMenu;
  
  private int storiesPages = 0;
  private int storyTypes = -1;
  private int mapPages = 0;
  private string storiesName;
  
  private List<MapStory> newMapStory = new List<MapStory> ();
  
  public void Start()
  {
    if (!TemporaryData.GetInstance ().isTutorialDone) ShowingTutorial ();
    if(PlayerPrefs.GetString (Const.WhatOpenInMenuScene, "") == "PlayerMenu") playerMenu.SetActive (true);
    else playerMenu.SetActive (false);
    dungeonMenu.SetActive (false);
    townMenu.SetActive (false);
    bgMenu.SetActive (false);
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene ().name);
  }
  
  private void ShowingTutorial()
  {
    mainMenu.transform.GetChild (2).GetComponent<Button> ().interactable = false;
    mainMenu.transform.GetChild (3).GetComponent<Button> ().interactable = false;
    mainMenu.transform.GetChild (4).GetComponent<Button> ().interactable = false;
    
    GameObject handTouch = Instantiate(Resources.Load<GameObject> ("TutorialHand"));
    handTouch.name = "tutorialhand";
    handTouch.transform.SetParent (mainMenu.transform);
    handTouch.transform.localScale = Vector3.one;
    handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
    handTouch.transform.localPosition = new Vector2 (-308, -71);
  }
  
  public void ShowMenu(string name)
  {
    playerMenu.SetActive (true);
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    playerMenu.transform.GetChild(2).FindChild (name).gameObject.SetActive (true);
    playerMenu.transform.GetChild (1).GetComponent<Image> ().color = mainMenu.transform.FindChild (name).GetComponent<Image> ().color;
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      playerMenu.transform.GetChild (2).GetChild (0).GetChild(1).GetComponent<Button> ().interactable = false;
      playerMenu.transform.GetChild (2).GetChild (0).GetChild (2).GetComponent<Button> ().interactable = false;

      if (GameObject.Find ("tutorialhand") == null) 
      {
        GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (160, 280);
      }
      else
      {
        GameObject handTouch = GameObject.Find ("tutorialhand");
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (160, 280);
      }
    }
  }

  public void ShowTownMenu(int townSceneNumber)
  {
    townMenu.SetActive (true);
    bgMenu.SetActive (true);
    PlayerPrefs.SetInt (Const.TownSceneNo, townSceneNumber);
    playerMenu.SetActive (false);
  }

  public void ShowMapMenu(int storyType)
  {
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    playerMenu.transform.GetChild(2).FindChild ("Stories").gameObject.SetActive (true);
    storyTypes = storyType;
    storiesPages = 0;
    GenerateStories (storyType, playerMenu.transform.GetChild(2).FindChild ("Stories"));
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      foreach (Transform child in playerMenu.transform.GetChild(2).FindChild("Stories").GetChild(0)) 
      {
        child.GetComponent<Button>().interactable = false;
      }

      playerMenu.transform.GetChild (2).FindChild ("Stories").GetChild (0).GetChild (0).GetComponent<Button>().interactable = true;
      
      if (GameObject.Find ("tutorialhand") == null) 
      {
        GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (100, 450);
      }
      else
      {
        GameObject handTouch = GameObject.Find ("tutorialhand");
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (100, 450);
      }
    }
  }
  
  public void GenerateStories(int storyType, Transform _parent)
  {
    newMapStory = new List<MapStory> ();
    newMapStory = GetDataFromSql.GetMapOfType (storyType);
    
    for (int i = 0; i < newMapStory.Count; i++) 
    {
      if (i > newMapStory.Count-1 || i > 4) break;
      GameObject storiesButton = Instantiate(Resources.Load<GameObject>("MainMenuScene/STORIESBUTTON"));
      storiesButton.transform.SetParent (_parent.GetChild(0));
      storiesButton.transform.GetChild (0).GetComponent<Text> ().text = newMapStory [i+(storiesPages*4)].storiesName;
      storiesButton.transform.localScale = Vector3.one;
      storiesButton.GetComponent<Button>().onClick.AddListener(()=>ShowMaps(storiesButton.transform.GetChild (0).GetComponent<Text> ().text.ToString()));
    }
    
    if (storiesPages >= Mathf.FloorToInt (newMapStory.Count / 4))
    {
      if (storiesPages == 0) 
      {
        if (newMapStory.Count <= 4)
        {
          _parent.GetChild (1).gameObject.SetActive (false);
          _parent.GetChild (2).gameObject.SetActive (false);
        }
        else 
        {
          _parent.GetChild (1).gameObject.SetActive (true);
          _parent.GetChild (2).gameObject.SetActive (false);
        }
      } 
      else 
      {
        _parent.GetChild (1).gameObject.SetActive (false);
        _parent.GetChild (2).gameObject.SetActive (true);
      }
    } 
    else
    {
      _parent.GetChild (1).gameObject.SetActive (true);
      _parent.GetChild (2).gameObject.SetActive (true);
    }
  }
  
  public void GenerateMapData(string storyName, Transform _parent)
  {
    List<int> mapInStories = newMapStory.Where (x => x.storiesName == storyName).First ().mapID; 
    
    for (int i = 0; i < mapInStories.Count; i++) 
    {
      if (i > mapInStories.Count - 1 || i > 4) break;
      GameObject mapButton = Instantiate(Resources.Load<GameObject>("MainMenuScene/STORIESBUTTON"));
      mapButton.transform.SetParent (_parent.GetChild(0));
      mapButton.transform.GetChild (0).GetComponent<Text> ().text = mapInStories [i+(mapPages*4)].ToString();
      mapButton.transform.localScale = Vector3.one;
      mapButton.GetComponent<Button>().onClick.AddListener(()=>ShowDialogBox(int.Parse(mapButton.transform.GetChild (0).GetComponent<Text> ().text)));
    }

    if (mapPages >= Mathf.FloorToInt (newMapStory.Count / 4))
    {
      if (mapPages == 0) 
      {
        if (newMapStory.Count <= 4)
        {
          _parent.GetChild (1).gameObject.SetActive (false);
          _parent.GetChild (2).gameObject.SetActive (false);
        }
        else 
        {
          _parent.GetChild (1).gameObject.SetActive (true);
          _parent.GetChild (2).gameObject.SetActive (false);
        }
      } 
      else 
      {
        _parent.GetChild (1).gameObject.SetActive (false);
        _parent.GetChild (2).gameObject.SetActive (true);
      }
    } 
    else
    {
      _parent.GetChild (1).gameObject.SetActive (true);
      _parent.GetChild (2).gameObject.SetActive (true);
    }
  }
  
  public void ShowMaps(string storyName)
  {
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    playerMenu.transform.GetChild(2).FindChild ("Map").gameObject.SetActive (true);
    storiesName = storyName;
    mapPages = 0;
    GenerateMapData (storiesName, playerMenu.transform.GetChild(2).FindChild ("Map"));
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      foreach (Transform child in playerMenu.transform.GetChild(2).FindChild("Map").GetChild(0)) 
      {
        child.gameObject.SetActive (false);
      }

      playerMenu.transform.GetChild (2).FindChild ("Map").GetChild (0).GetChild (0).gameObject.SetActive (true);

      if (GameObject.Find ("tutorialhand") == null) 
      {
        GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (100, 450);
      }
      else
      {
        GameObject handTouch = GameObject.Find ("tutorialhand");
        handTouch.transform.SetParent (playerMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (100, 450);
      }
    }
  }
  
  public void ShowDialogBox(int mapID)
  {
    PlayerPrefs.SetInt (Const.MapNo, mapID);
    dungeonMenu.SetActive (true);
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      foreach (Transform child in dungeonMenu.transform.GetChild(0)) 
      {
        child.GetComponent<Button>().interactable = false;
      }

      dungeonMenu.transform.GetChild(0).GetChild (0).GetComponent<Button>().interactable = true;

      if (GameObject.Find ("tutorialhand") == null) 
      {
        GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
        handTouch.transform.SetParent (dungeonMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (90, 130);
      }
      else
      {
        GameObject handTouch = GameObject.Find ("tutorialhand");
        handTouch.transform.SetParent (dungeonMenu.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (90, 130);
      }
    }
  }

  public void GoPlayScene()
  {
    if (MapSaveAndLoad.CheckingMap (PlayerPrefs.GetInt (Const.MapNo, 0))) 
    {
      SceneManager.LoadScene ("LoadScene");
    } 
    else
    {
      Debug.Log ("Invalid Map");
    }
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
    PlayerPrefs.SetString (Const.WhatOpenInMenuScene, "PlayerMenu");
  }

  public void ExitGame()
  {
    Application.Quit ();
  }

  public void Exit()
  {
    townMenu.SetActive (false);
    dungeonMenu.SetActive (false);
    bgMenu.SetActive (false);
  }
  
  public void SaveGame()
  {
    SystemManager.SaveGameData ();
  }
  
  public void BackButton()
  {
    playerMenu.SetActive (false);
  }
  
  public void NextPageStories(int i)
  {
    storiesPages += i;
    GenerateStories (storyTypes, playerMenu.transform.GetChild (2).FindChild ("Stories"));
  }
  
  public void NextPageMap(int i)
  {
    mapPages += i;
    GenerateMapData (storiesName, playerMenu.transform.GetChild (2).FindChild ("Map"));
  }
}
