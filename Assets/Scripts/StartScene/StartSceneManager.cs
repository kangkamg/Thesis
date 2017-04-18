using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour 

{
  public Text touchText;
  public Image Book;
  public GameObject openBook;
  
  float alphaColor;

  public bool bookOpen = false;

  private void Awake()
  {
    touchText.text = "Touch To Start";
    Book.sprite = Resources.Load<Sprite> ("StartSceneImage/Book");
    bookOpen = false;
    openBook.SetActive (bookOpen);
    openBook.transform.GetChild (0).gameObject.SetActive (false);
    openBook.transform.GetChild (1).gameObject.SetActive (false);
    
    if (!TemporaryData.GetInstance ().firstTimeOpenGame) 
    {
      GetDataFromSql.OpenDB ("ThesisDatabase.db");
      TemporaryData.GetInstance ().firstTimeOpenGame = true;
      TemporaryData.GetInstance ().allStory = GetDataFromSql.GetAllStoryDialogue ();
    }
  }

  public static void InitFirstData()
  {
    PlayerData data = new PlayerData ();

    data.name = SystemInfo.deviceName;
    data.gold = 500;
    data.id = 0;
    data.chapter = 1;
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

          equiped = new AbilityStatus ();
          equiped.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          equiped.level = 1;
          equiped.exp = 0;
          adding.equipedAbility.Add (equiped);
        }
      }
    }
    adding.partyOrdering = 0;
    adding.experience = 0;

    equipedItem.item = GetDataFromSql.GetItemFromID (1100);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (2100);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (3100);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    adding = new CharacterStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter (1002);
    adding.characterLevel = 1;
    adding.isInParty = true;
    for(int i= 0; i < adding.basicStatus.learnAbleAbility.Count;i++)
    {
      string[] learnAbleAb = adding.basicStatus.learnAbleAbility [i].Split (" " [0]);
      for(int j = 0; j < learnAbleAb.Length; j=j+2)
      {
        if(int.Parse(learnAbleAb[j+1]) == adding.characterLevel)
        {
          learning = new AbilityStatus ();
          learning.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          learning.level = 1;
          learning.exp = 0;
          adding.learnedAbility.Add (learning);

          equiped = new AbilityStatus ();
          equiped.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          equiped.level = 1;
          equiped.exp = 0;
          adding.equipedAbility.Add (equiped);
        }
      }
    }
    adding.partyOrdering = 1;
    adding.experience = 0;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (1200);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (2100);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (3100);
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);
    
    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromID (1100);
    equipedItem.equiped = false;
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    TemporaryData.GetInstance ().playerData = data;
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
      /*else
      {
        /*if (PlayerPrefs.GetInt (Const.NewGame, 1) == 1)
        {
          InitFirstData ();
          PlayerPrefs.SetInt (Const.MapNo, 1);
          //SceneManager.LoadScene ("GamePlayScene");
          SceneManager.LoadScene ("StoryScene");
        }
        else
        {
          InitFirstData ();
          SceneManager.LoadScene ("MainMenuScene");
        //}  
      }*/
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
          openBook.transform.GetChild (0).gameObject.SetActive (true);
          openBook.transform.GetChild (1).gameObject.SetActive (false);
        }
        else if (openBook.transform.GetChild (0).gameObject.activeSelf)
        {
          CloseTheBook ();
        } 
      }
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
  
  private void CreateSaveIndex(int isNewGame)
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
