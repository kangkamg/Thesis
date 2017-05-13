using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StartSceneManager : MonoBehaviour 

{
  public Text touchText;
  public Image Book;
  public GameObject openBook;
  
  float alphaColor;

  public bool bookOpen = false;

  private static StartSceneManager _instance;
  public static StartSceneManager GetInstance()
  {
    return _instance;
  }
  
  private void Awake()
  {
    _instance = this;
    
    if (string.IsNullOrEmpty (PlayerPrefs.GetString (Const.Language))) 
    {
      PlayerPrefs.SetString (Const.Language, Application.systemLanguage.ToString ());
    } 

    TemporaryData.GetInstance().choosenLanguage = PlayerPrefs.GetString(Const.Language,"Thai");
    
    TemporaryData.GetInstance().version = PlayerPrefs.GetString (Const.Version, "");
    
    PlayerPrefs.DeleteAll ();
    
    touchText.text = "Touch To Start";
    Book.sprite = Resources.Load<Sprite> ("StartSceneImage/Book");
    bookOpen = false;
    openBook.SetActive (bookOpen);
    openBook.transform.GetChild (0).gameObject.SetActive (false);
    openBook.transform.GetChild (1).gameObject.SetActive (false);
    PlayerPrefs.SetString (Const.PreviousScene, SceneManager.GetActiveScene().name);
    
    if (!TemporaryData.GetInstance ().firstTimeOpenGame) 
    {
      GetDataFromSql.OpenDB ("ThesisDatabase.db");
      TemporaryData.GetInstance ().firstTimeOpenGame = true;
    }
  }

  public static void InitFirstData()
  {
    PlayerData data = new PlayerData ();

    data.name = SystemInfo.deviceName;
    data.gold = 500;
    data.id = 0;
    data.acceptedQuest = new List<Quest> ();

    CharacterStatus adding = new CharacterStatus ();
    Item equipedItem = new Item ();
    AbilityStatus equiped = new AbilityStatus ();
    AbilityStatus learning = new AbilityStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter (1001);
    adding.characterLevel = 1;
    adding.isInParty = true;

    for(int i= 0; i < adding.basicStatus.learnAbleAbility.Count;i++)
    {
      string[] learnAbleAb = adding.basicStatus.learnAbleAbility [i].Split (" " [0]);
      for(int j = 0; j < learnAbleAb.Length; j=j+2)
      {
        if(int.Parse(learnAbleAb[j+1]) <= adding.characterLevel)
        {
          learning = new AbilityStatus ();
          learning.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          learning.level = 1;
          learning.exp = 0;
          adding.learnedAbility.Add (learning);
        }
      }
    }
    
    equiped = new AbilityStatus ();
    equiped = adding.learnedAbility [0];
    adding.equipedAbility.Add (equiped);

    equiped = new AbilityStatus ();
    equiped = adding.learnedAbility [1];
    adding.equipedAbility.Add (equiped);
    
    adding.partyOrdering = 0;
    adding.experience = 0;

    equipedItem.item = GetDataFromSql.GetItemFromID (1000);
    SetUpEquipment (equipedItem, adding, data);

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (2000);
    SetUpEquipment (equipedItem, adding, data);
    
    data.characters.Add (adding);
    TemporaryData.GetInstance ().playerData = data;
  }
  
  private static void SetUpEquipment(Item checking, CharacterStatus adding, PlayerData data, bool equiped = true)
  {
    if (checking.item != null)
    {
      checking.equiped = equiped;
      if(equiped) adding.equipItem.Add (checking);
      data.inventory.Add (checking);
      checking.ordering = data.inventory.Count - 1;
    }
  }
  
  private void Update()
  {
    BlinkText ();
    if(Input.GetMouseButtonDown(0)/*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
    {
      if (!bookOpen) 
      {
        OpenTheBook ();
      } 
    }
    
    if (Input.GetKeyDown (KeyCode.Escape)) 
    {
      if (!bookOpen)
      {
        Application.Quit ();
      } 
      else 
      {
        if (openBook.transform.GetChild (1).gameObject.activeSelf)
        {
          if(GameObject.Find("DialogBox(Clone)") != null)
          {
            Destroy(GameObject.Find("DialogBox(Clone)"));
            CreateSaveIndex(openBook.transform.GetChild (1).GetChild (1).GetChild (0).GetComponent<SaveData> ()._isNewgame);
          }
          else
          {
            openBook.transform.GetChild (0).gameObject.SetActive (true);
            openBook.transform.GetChild (1).gameObject.SetActive (false);
          }
        }
        else if (openBook.transform.GetChild (0).gameObject.activeSelf)
        {
          CloseTheBook ();
        } 
      }
    }
  }
  
  public void BackButton()
  {
    if (openBook.transform.GetChild (1).gameObject.activeSelf)
    {
      openBook.transform.GetChild (0).gameObject.SetActive (true);
      openBook.transform.GetChild (1).gameObject.SetActive (false);
    }
    else if (openBook.transform.GetChild (0).gameObject.activeSelf)
    {
      CloseTheBook ();
    } 
  }

  private void BlinkText()
  {
    if (touchText.color.a <= 0)
      alphaColor = Time.deltaTime;
    else if (touchText.color.a >= 1)
      alphaColor = -Time.deltaTime;

    touchText.color = new Color (touchText.color.r, touchText.color.g, touchText.color.b, touchText.color.a + alphaColor);
  }

  private void CloseTheBook()
  {
    Book.sprite = Resources.Load<Sprite> ("StartSceneImage/Book");
    touchText.gameObject.SetActive (true);
    bookOpen = false;
    openBook.SetActive (bookOpen);
    openBook.transform.GetChild (0).gameObject.SetActive (false);
    openBook.transform.GetChild (1).gameObject.SetActive (false);
  }
  
  private void OpenTheBook()
  {
    Book.sprite = Resources.Load<Sprite> ("StartSceneImage/Openbook");
    touchText.gameObject.SetActive (false);
    bookOpen = true;
    openBook.SetActive (bookOpen);
    openBook.transform.GetChild (0).gameObject.SetActive (true);
    openBook.transform.GetChild (1).gameObject.SetActive (false);
  }
  
  public void CreateSaveIndex(int isNewGame)
  {
    foreach (Transform child in openBook.transform.GetChild (1).GetChild(1)) 
    {
      Destroy (child.gameObject);
    }
    
    for(int i = 0;i<4;i++)
    {
      GameObject save = Instantiate (Resources.Load<GameObject> ("StartSceneImage/SavePrefabs"));
      save.transform.SetParent (openBook.transform.GetChild (1).GetChild(1));
      save.transform.localScale = Vector3.one;
      save.GetComponent<SaveData> ().SetSaveData (i, isNewGame);
    }
  }
  
  public void SelectedStartGame(int isNewGame)
  {
    openBook.transform.GetChild (0).gameObject.SetActive (false);
    openBook.transform.GetChild (1).gameObject.SetActive (true);
    if (isNewGame == 0) 
    {
      CreateSaveIndex (0);
    }
    else 
    {
      CreateSaveIndex (1);
    }
  }
}
