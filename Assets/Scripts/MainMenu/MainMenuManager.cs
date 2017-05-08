using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
  public GameObject playerMenu;
  public GameObject bgMenu;
  public GameObject mainMenu;
  
  private int storiesPages = 0;
  private int storyTypes = -1;
  private int mapPages = 0;
  private int storyInBookTypes = -1;
  private string storiesName;
  
  public int tutorialNo = 0;
  
  private List<MapStory> newMapStory = new List<MapStory> ();
  
  public void Start()
  {
    StartCoroutine (SystemManager.IncreasePlayedHrs ());
    
     TemporaryData.GetInstance ().playerData.characters [0].characterLevel = 10;
    
      for(int i= 0; i < TemporaryData.GetInstance ().playerData.characters [0].basicStatus.learnAbleAbility.Count;i++)
      {
        AbilityStatus learning = new AbilityStatus ();
        string[] learnAbleAb = TemporaryData.GetInstance ().playerData.characters [0].basicStatus.learnAbleAbility [i].Split (" " [0]);
        for(int j = 0; j < learnAbleAb.Length; j=j+2)
        {
           if (int.Parse (learnAbleAb [j + 1]) <= TemporaryData.GetInstance ().playerData.characters [0].characterLevel && TemporaryData.GetInstance ().playerData.characters [0].learnedAbility.Where(x=>x.ability.ID == int.Parse(learnAbleAb[j])).Count() <= 0) 
          {
            learning.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
            learning.level = 1;
            learning.exp = 0;
            TemporaryData.GetInstance ().playerData.characters [0].learnedAbility.Add (learning);
           }
        }
      }
      
    playerMenu.SetActive (false);
    bgMenu.SetActive (false);
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene ().name);
    
    if (!TemporaryData.GetInstance ().isTutorialDone)
    {
      playerMenu.transform.GetChild (3).gameObject.SetActive (false);
      
      if (TemporaryData.GetInstance ().playerData.passedMap.Where (x => x == 1).Count () <= 0)
        ShowingTutorial1 ();
      else if (TemporaryData.GetInstance ().playerData.passedMap.Where (x => x == 1).Count () == 1)
        ShowingTutorial2 ();
    } 
    else 
    {
      SystemManager.SaveGameData ();
      if (PlayerPrefs.GetString (Const.WhatOpenInMenuScene, "") == "Informations") 
      {
        playerMenu.SetActive (true);
        foreach (Transform child in playerMenu.transform.GetChild (2)) 
        {
          child.gameObject.SetActive (false);
        }
        playerMenu.transform.GetChild (2).GetChild (1).gameObject.SetActive (true);
        playerMenu.transform.GetChild (1).GetComponent<Image> ().color = mainMenu.transform.FindChild (PlayerPrefs.GetString (Const.WhatOpenInMenuScene)).GetComponent<Image> ().color;
      } 
      else if (PlayerPrefs.GetString (Const.WhatOpenInMenuScene, "") == "Adventures")
      {
        playerMenu.SetActive (true);
        foreach (Transform child in playerMenu.transform.GetChild (2))
        {
          child.gameObject.SetActive (false);
        }
        playerMenu.transform.GetChild (2).GetChild (0).gameObject.SetActive (true);
        playerMenu.transform.GetChild (1).GetComponent<Image> ().color = mainMenu.transform.FindChild (PlayerPrefs.GetString (Const.WhatOpenInMenuScene)).GetComponent<Image> ().color;
      }
      else if (PlayerPrefs.GetString (Const.WhatOpenInMenuScene, "") == "Shop") 
      {
        playerMenu.SetActive (true);
        foreach (Transform child in playerMenu.transform.GetChild (2))
        {
          child.gameObject.SetActive (false);
        }
        playerMenu.transform.GetChild (2).GetChild (2).gameObject.SetActive (true);
        playerMenu.transform.GetChild (1).GetComponent<Image> ().color = mainMenu.transform.FindChild (PlayerPrefs.GetString (Const.WhatOpenInMenuScene)).GetComponent<Image> ().color;
      } 
      else if (PlayerPrefs.GetString (Const.WhatOpenInMenuScene, "") == "FinishedStories") 
      {
        playerMenu.SetActive (true);
        foreach (Transform child in playerMenu.transform.GetChild (2))
        {
          child.gameObject.SetActive (false);
        }
        playerMenu.transform.GetChild (2).FindChild("FinishedStories").gameObject.SetActive (true);
        playerMenu.transform.GetChild (1).GetComponent<Image> ().color = mainMenu.transform.FindChild (PlayerPrefs.GetString (Const.WhatOpenInMenuScene)).GetComponent<Image> ().color;
        
        GenerateFinishedStories (PlayerPrefs.GetInt(Const.StoryType,0),playerMenu.transform.GetChild (2).FindChild ("FinishedStories"));
      } 
      else playerMenu.SetActive (false);
      bgMenu.SetActive (false);
    }
  }
  
  private void ShowingTutorial1()
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
    
    tutorialNo = 1;
  }
  
  private void ShowingTutorial2()
  {
    mainMenu.transform.GetChild (1).GetComponent<Button> ().interactable = false;
    mainMenu.transform.GetChild (3).GetComponent<Button> ().interactable = false;
    mainMenu.transform.GetChild (4).GetComponent<Button> ().interactable = false;

    GameObject handTouch = Instantiate(Resources.Load<GameObject> ("TutorialHand"));
    handTouch.name = "tutorialhand";
    handTouch.transform.SetParent (mainMenu.transform);
    handTouch.transform.localScale = Vector3.one;
    handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
    handTouch.transform.localPosition = new Vector2 (-30, -71);
    
    tutorialNo = 2;
  }
  
  public void ShowMenuStoriesBook(string name)
  {
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    playerMenu.transform.GetChild(2).FindChild (name).gameObject.SetActive (true);
    PlayerPrefs.SetString (Const.WhatOpenInMenuScene, name);
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
    PlayerPrefs.SetString (Const.WhatOpenInMenuScene, name);
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      if (tutorialNo == 1)
      {
        playerMenu.transform.GetChild (2).GetChild (0).GetChild (1).GetComponent<Button> ().interactable = false;
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
      else if (tutorialNo == 2)
      {
        playerMenu.transform.GetChild (2).GetChild (1).GetChild (1).GetComponent<Button> ().interactable = false;
        playerMenu.transform.GetChild (2).GetChild (1).GetChild (2).GetComponent<Button> ().interactable = false;
        playerMenu.transform.GetChild (2).GetChild (1).GetChild (3).GetComponent<Button> ().interactable = false;
        
        if (GameObject.Find ("tutorialhand") == null)
        {
          GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
          handTouch.transform.SetParent (playerMenu.transform);
          handTouch.transform.localScale = Vector3.one;
          handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
          handTouch.transform.localPosition = new Vector2 (160, 400);
        } 
        else
        {
          GameObject handTouch = GameObject.Find ("tutorialhand");
          handTouch.transform.SetParent (playerMenu.transform);
          handTouch.transform.localScale = Vector3.one;
          handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
          handTouch.transform.localPosition = new Vector2 (160, 400);
        }
      }
    }
  }

  public void ShowTownMenu(int townSceneNumber)
  {
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
  
  public void ShowStoriesBookMenu(int storyType)
  {
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    playerMenu.transform.GetChild(2).FindChild ("FinishedStories").gameObject.SetActive (true);
    PlayerPrefs.SetInt (Const.StoryType, storyType);
    storyInBookTypes = storyType;
    storiesPages = 0;
    GenerateFinishedStories (storyType, playerMenu.transform.GetChild(2).FindChild ("FinishedStories"));
  }
  
  public void GenerateFinishedStories(int storyType, Transform _parent)
  {
    foreach (Transform child in _parent.GetChild(0)) 
    {
      Destroy (child.gameObject);
    }
    newMapStory = new List<MapStory> ();
    newMapStory = GetDataFromSql.GetMapOfType (storyType);
    
    int countPassed = 0;
    
    for (int i = 0; i < newMapStory.Count; i++) 
    {
      foreach(int a in newMapStory[i].mapID)
      {
        foreach (int b in TemporaryData.GetInstance().playerData.passedMap)
        {
          if(b == a)
          {
            countPassed += 1;
          }
        }
      }
      
      if (i+(storiesPages*4) > newMapStory.Count-1 || i > 4) break;
      
      if (countPassed == newMapStory [i+ (storiesPages * 4)].mapID.Count) 
      {
        GameObject storiesButton = Instantiate (Resources.Load<GameObject> ("MainMenuScene/STORIESBUTTON"));
        storiesButton.transform.SetParent (_parent.GetChild (0));
        storiesButton.transform.GetChild (0).GetComponent<LanguageTextChanged>().SetLanguage(newMapStory [i + (storiesPages * 4)].storiesName);
        storiesButton.transform.localScale = Vector3.one;
        storiesButton.GetComponent<Button> ().onClick.AddListener (() => GoToStoriesBook (storiesButton.transform.GetChild (0).GetComponent<LanguageTextChanged>().keyLanguage));
      }
    }
    
    if (_parent.GetChild (0).childCount == 0) _parent.GetChild (3).gameObject.SetActive (true);
    else _parent.GetChild (3).gameObject.SetActive (false);
    
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
  
  private void GoToStoriesBook(string storiesName)
  {
    foreach (Transform child in playerMenu.transform.GetChild(2)) 
    {
      child.gameObject.SetActive (false);
    }
    
    playerMenu.transform.GetChild(2).FindChild ("StoriesInBook").gameObject.SetActive (true);
    playerMenu.transform.GetChild (2).FindChild ("StoriesInBook").GetComponent<MainMenuStoriesDialogueManager> ().InitDialogue (storiesName, playerMenu.transform.FindChild("BackButton"));
  }
  
  public void GenerateStories(int storyType, Transform _parent)
  {
    foreach (Transform child in _parent.GetChild(0)) 
    {
      Destroy (child.gameObject);
    }
    newMapStory = new List<MapStory> ();
    newMapStory = GetDataFromSql.GetMapOfType (storyType);
    
    for (int i = 0; i < newMapStory.Count; i++) 
    {
      if (i+(storiesPages*4) > newMapStory.Count-1 || i > 4) break;
      GameObject storiesButton = Instantiate(Resources.Load<GameObject>("MainMenuScene/STORIESBUTTON"));
      storiesButton.transform.SetParent (_parent.GetChild(0));
      storiesButton.transform.GetChild (0).GetComponent<LanguageTextChanged>().SetLanguage(newMapStory [i + (storiesPages * 4)].storiesName);
      storiesButton.transform.localScale = Vector3.one;
      storiesButton.GetComponent<Button>().onClick.AddListener(()=>ShowMaps(  storiesButton.transform.GetChild (0).GetComponent<LanguageTextChanged>().keyLanguage));
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
    foreach (Transform child in _parent.GetChild(0)) 
    {
      Destroy (child.gameObject);
    }
    
    List<int> mapInStories = newMapStory.Where (x => x.storiesName == storyName).First ().mapID; 
    
    for (int i = 0; i < mapInStories.Count; i++) 
    {
      if (i+(mapPages*4) > mapInStories.Count - 1 || i > 4) break;
      GameObject mapButton = Instantiate(Resources.Load<GameObject>("MainMenuScene/STORIESBUTTON"));
      mapButton.transform.SetParent (_parent.GetChild(0));
      mapButton.transform.GetChild (0).name = mapInStories [i+(mapPages*4)].ToString();
      mapButton.transform.GetChild (0).GetComponent<LanguageTextChanged> ().SetLanguage(mapInStories [i+(mapPages*4)].ToString());
      mapButton.transform.localScale = Vector3.one;
      mapButton.GetComponent<Button>().onClick.AddListener(()=>ShowDialogBox(int.Parse( mapButton.transform.GetChild (0).GetComponent<LanguageTextChanged>().keyLanguage)));
      
      if (i != 0)
      {
        if (TemporaryData.GetInstance ().playerData.passedMap.Where (x => x == (i + (mapPages * 4))).Count () <= 0)
          mapButton.GetComponent<Button> ().interactable = false;
      }
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
    
    GameObject dialogBox = GameObject.Instantiate (DialogBoxManager.GetInstance ().GenerateDialogBox ("Enter this map ?", true));

    dialogBox.transform.SetParent (GameObject.Find("MenuCanvas").transform);
    dialogBox.transform.localScale = Vector3.one;
    dialogBox.transform.localPosition = Vector2.zero;
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => GoPlayScene ());
    dialogBox.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => CancelDialogBox (dialogBox));
    
    foreach (Transform child in playerMenu.transform.GetChild(2).FindChild ("Map").GetChild(0))
    {
      if (int.Parse(child.GetChild (0).name) != mapID)
      {
        Destroy (child.gameObject);
      }
      else
      {
        child.GetComponent<Button> ().interactable = false;
      }
    }
    
    if (!TemporaryData.GetInstance ().isTutorialDone) 
    {
      dialogBox.transform.GetChild (2).GetComponent<Button>().interactable = false;

      
      if (GameObject.Find ("tutorialhand") == null) 
      {
        GameObject handTouch = Instantiate (Resources.Load<GameObject> ("TutorialHand"));
        handTouch.transform.SetParent (dialogBox.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (290, -125);
      }
      else
      {
        GameObject handTouch = GameObject.Find ("tutorialhand");
        handTouch.transform.SetParent (dialogBox.transform);
        handTouch.transform.localScale = Vector3.one;
        handTouch.transform.localRotation = new Quaternion (0, 0, -0.35f, -0.7f);
        handTouch.transform.localPosition = new Vector2 (290, -125);
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
  }

  public void ExitGame()
  {
    Application.Quit ();
  }

  public void Exit()
  {
    bgMenu.SetActive (false);
  }
  
  public void SaveGame()
  {
    SystemManager.SaveGameData ();
  }
  
  public void BackButton()
  {
    if (playerMenu.transform.GetChild (2).FindChild ("Adventures").gameObject.activeSelf) 
    {
      playerMenu.SetActive (false);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Informations").gameObject.activeSelf) 
    {
      playerMenu.SetActive (false);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Shop").gameObject.activeSelf) 
    {
      playerMenu.SetActive (false);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Options").gameObject.activeSelf) 
    {
      playerMenu.SetActive (false);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Stories").gameObject.activeSelf) 
    {
      playerMenu.transform.GetChild (2).FindChild ("Stories").gameObject.SetActive (false);
      playerMenu.transform.GetChild (2).FindChild ("Adventures").gameObject.SetActive (true);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Map").gameObject.activeSelf) 
    {
      playerMenu.transform.GetChild (2).FindChild ("Map").gameObject.SetActive (false);
      ShowMapMenu (storyTypes);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("StoriesBook").gameObject.activeSelf) 
    {
      playerMenu.transform.GetChild (2).FindChild ("StoriesBook").gameObject.SetActive (false);
      playerMenu.transform.GetChild (2).FindChild ("Informations").gameObject.SetActive (true);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("FinishedStories").gameObject.activeSelf) 
    {
      playerMenu.transform.GetChild (2).FindChild ("FinishedStories").gameObject.SetActive (false);
      playerMenu.transform.GetChild (2).FindChild ("StoriesBook").gameObject.SetActive (true);
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("StoriesInBook").gameObject.activeSelf) 
    {
      playerMenu.transform.GetChild (2).FindChild ("StoriesInBook").gameObject.SetActive (false);
      playerMenu.transform.GetChild (2).FindChild ("FinishedStories").gameObject.SetActive (true);
      GenerateFinishedStories (storyInBookTypes, playerMenu.transform.GetChild (2).FindChild ("FinishedStories"));
    }
  }
  
  public void NextPageStoriesBook(int i)
  {
    storiesPages += i;
    GenerateFinishedStories (PlayerPrefs.GetInt(Const.StoryType), playerMenu.transform.GetChild (2).FindChild ("FinishedStories"));
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

  public void CancelDialogBox(GameObject dialogBox)
  {
    Destroy (dialogBox);
    if (playerMenu.transform.GetChild (2).FindChild ("Stories").gameObject.activeSelf) 
    {
      GenerateStories (storyTypes, playerMenu.transform.GetChild (2).FindChild ("Stories"));
    }
    else if (playerMenu.transform.GetChild (2).FindChild ("Map").gameObject.activeSelf) 
    {
      GenerateMapData (storiesName, playerMenu.transform.GetChild (2).FindChild ("Map"));
    }
  }
}
