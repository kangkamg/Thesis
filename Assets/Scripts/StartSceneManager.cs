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

  public Text lvAv;
  public Text playHrs;
  public Text chapter;

  float alphaColor;

  public bool bookOpen = false;

  private void Awake()
  {
    if (!TemporaryData.GetInstance ().firstTimeOpenGame) 
    {
      GetDataFromSql.OpenDB ("ThesisDatabase.db");
      TemporaryData.GetInstance ().firstTimeOpenGame = true;
    }
  }

  private void InitFirstData()
  {
    PlayerData data = new PlayerData ();

    data.name = SystemInfo.deviceName;
    data.gold = 500;
    data.id = 0.ToString();
    data.chapter = 1;
    data.acceptedQuest = new List<Quest> ();

    CharacterStatus adding = new CharacterStatus ();
    Item equipedItem = new Item ();
    adding.basicStatus = GetDataFromSql.GetCharacter ("Kadkaew");
    adding.characterLevel = 1;
    adding.isInParty = true;
    adding.normalAttack.ability = adding.basicStatus.normalAttack;
    adding.normalAttack.level = 1;
    adding.specialAttack.ability = adding.basicStatus.specialAttack;
    adding.specialAttack.level = 1;
    adding.partyOrdering = 0;
    adding.experience = 0;

    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortSword");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodArmor");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodRing");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("RedPotion");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    adding = new CharacterStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter ("Maprang");
    adding.characterLevel = 1;
    adding.isInParty = true;
    adding.normalAttack.ability = adding.basicStatus.normalAttack;
    adding.normalAttack.level = 1;
    adding.specialAttack.ability = adding.basicStatus.specialAttack;
    adding.specialAttack.level = 1;
    adding.partyOrdering = 1;
    adding.experience = 0;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortBow");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodArmor");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodRing");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    adding = new CharacterStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter ("N");
    adding.characterLevel = 1;
    adding.isInParty = false;
    adding.normalAttack.ability = adding.basicStatus.normalAttack;
    adding.normalAttack.level = 1;
    adding.specialAttack.ability = adding.basicStatus.specialAttack;
    adding.specialAttack.level = 1;
    adding.partyOrdering = -1;
    adding.experience = 0;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortBow");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodArmor");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodRing");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    adding = new CharacterStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter ("A");
    adding.characterLevel = 1;
    adding.isInParty = false;
    adding.normalAttack.ability = adding.basicStatus.normalAttack;
    adding.normalAttack.level = 1;
    adding.specialAttack.ability = adding.basicStatus.specialAttack;
    adding.specialAttack.level = 1;
    adding.partyOrdering = -1;
    adding.experience = 0;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortBow");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodArmor");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodRing");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    adding = new CharacterStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter ("B");
    adding.characterLevel = 1;
    adding.isInParty = false;
    adding.normalAttack.ability = adding.basicStatus.normalAttack;
    adding.normalAttack.level = 1;
    adding.specialAttack.ability = adding.basicStatus.specialAttack;
    adding.specialAttack.level = 1;
    adding.partyOrdering = -1;
    adding.experience = 0;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortBow");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodArmor");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("WoodRing");
    equipedItem.equiped = true;
    adding.equipItem.Add (equipedItem);
    data.inventory.Add (equipedItem);
    equipedItem.ordering = data.inventory.Count - 1;

    data.characters.Add (adding);

    equipedItem = new Item ();
    equipedItem.item = GetDataFromSql.GetItemFromName ("ShortSword");
    equipedItem.equiped = false;
    data.inventory.Add (equipedItem);

    TemporaryData.GetInstance ().playerData = data;
  }

  private void Update()
  {
    BlinkText ();
    if(Input.GetMouseButtonDown(0) /*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
    {
      if (!bookOpen) 
      {
        OpenTheBook ();
      } 
      else
      {
        /*if (PlayerPrefs.GetInt (Const.NewGame, 1) == 1)
        {
          InitFirstData ();
          PlayerPrefs.SetInt (Const.MapNo, 1);
          SceneManager.LoadScene ("GamePlayScene");
          SceneManager.LoadScene ("StoryScene");
        }
        else
        {*/
          InitFirstData ();
          SceneManager.LoadScene ("MainMenuScene");
        //}
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

  private void OpenTheBook()
  {
    PlayerPrefs.SetInt (Const.NewGame, 1);
    Book.sprite = Resources.Load<Sprite> ("StartSceneImage/Openbook");
    touchText.text = "Touch To Start NewGame";
    bookOpen = true;
    openBook.SetActive (bookOpen);

    if (PlayerPrefs.GetInt (Const.SaveAmount, 0) <= 0)
    {
      lvAv.text = "xx";
      playHrs.text = "xx:xx:xx";
      chapter.text = "xx";
    }
  }
}
