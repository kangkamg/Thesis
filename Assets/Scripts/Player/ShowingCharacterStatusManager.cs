using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShowingCharacterStatusManager : MonoBehaviour 

{
  public Image chaImg;
  public Text characterName;
  public Transform status;
  public Transform equipment;
  public Transform attack;
  public Transform skill;
  
  public List<AbilityInformation> abilityInSlots;
  

  public void UpdateStatus()
  {
    chaImg.sprite = Resources.Load<Sprite> ("Image/Character/" + TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName);
    characterName.text = TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName.ToString ();

    status.GetChild(0).GetChild(0).GetComponent<Text>().text = TemporaryData.GetInstance ().selectedCharacter.characterLevel.ToString();
    status.GetChild(1).GetChild(0).GetComponent<Text>().text = (TemporaryData.GetInstance ().selectedCharacter.nextLevelExp - TemporaryData.GetInstance ().selectedCharacter.experience).ToString();
    status.GetChild(2).GetChild(0).GetComponent<Text>().text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
    status.GetChild(3).GetChild(0).GetComponent<Text>().text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString();
    status.GetChild(4).GetChild(0).GetComponent<Text>().text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString();
    status.GetChild(5).GetChild(0).GetComponent<Text>().text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString();
    
    SetUpAbility ();
    SetUpEquipment ();
  }
  
  private void SetUpAbility()
  {
    foreach (Transform child in attack.GetChild(0).GetChild(0))
    {
      Destroy (child.gameObject);
    }
    foreach (Transform child in attack.GetChild(1).GetChild(0))
    {
      Destroy (child.gameObject);
    }
    abilityInSlots.Clear ();
    
    List<AbilityStatus> normalAttacks = TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.abilityType == 1 || x.ability.abilityType == -1).ToList ();
    List<AbilityStatus> specialAttacks = new List<AbilityStatus> ();
    
    if(TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.abilityType == 3 || x.ability.abilityType == -3).Count() > 0)
      specialAttacks = TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.abilityType == 3 || x.ability.abilityType == -3).ToList ();
    
    List<AbilityStatus> skills = TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.abilityType == 2 || x.ability.abilityType == -2 || x.ability.abilityType == 0).ToList ();
    
    for (int i = 0; i < 2; i++)
    {
      GameObject normalAtkObj = Instantiate (Resources.Load<GameObject> ("SupMenu/CharacterStatusPrefabs/Ability"));
      normalAtkObj.transform.SetParent (attack.GetChild (0).GetChild(0));
      normalAtkObj.transform.localScale = Vector3.one;
      normalAtkObj.GetComponent<AbilityInformation> ().ordering = i;
      if (i <= normalAttacks.Count - 1) 
      {
        AbilityStatus equipedStatus = normalAttacks [i];
        if(Resources.Load<Sprite> ("Ability/Normal/" +  equipedStatus.ability.ID) != null)
          normalAtkObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/" + equipedStatus.ability.ID);
        else
          normalAtkObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Normal/" + equipedStatus.ability.abilityEff);
        normalAtkObj.GetComponent<AbilityInformation> ().SetUpAbilityStatus (equipedStatus);
        abilityInSlots.Add (normalAtkObj.GetComponent<AbilityInformation> ());
      }
      else
      {
        normalAtkObj.GetComponent<AbilityInformation> ().SetUpAbilityStatus (1);
      }
    }
    
    GameObject specialAtkObj = Instantiate (Resources.Load<GameObject> ("SupMenu/CharacterStatusPrefabs/Ability"));
    specialAtkObj.transform.SetParent (attack.GetChild (1).GetChild(0));
    specialAtkObj.transform.localScale = Vector3.one;
    if(Resources.Load<Sprite> ("Ability/Special/" +  specialAttacks [0].ability.ID) != null)
      specialAtkObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Special/" +  specialAttacks [0].ability.ID);
    else
      specialAtkObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/Special/" +  specialAttacks [0].ability.abilityEff);
    
    specialAtkObj.GetComponent<AbilityInformation> ().SetUpAbilityStatus (specialAttacks [0]);
    specialAtkObj.GetComponent<AbilityInformation> ().ordering = 2;
    abilityInSlots.Add (specialAtkObj.GetComponent<AbilityInformation> ());
    
    SortingAbility ();
  }
  
  private void SortingAbility()
  {
    TemporaryData.GetInstance().selectedCharacter.equipedAbility.Sort(delegate(AbilityStatus a, AbilityStatus b)
      {
        int ao = -1;
        int bo = -1;
        if(abilityInSlots.Where(x=>x.abilityStatus.ability.ID == a.ability.ID).Count()>0)
        {
          ao = abilityInSlots.Where(x=>x.abilityStatus.ability.ID == a.ability.ID).First().ordering;
        }
        if(abilityInSlots.Where(x=>x.abilityStatus.ability.ID == b.ability.ID).Count()>0)
        {
          bo = abilityInSlots.Where(x=>x.abilityStatus.ability.ID == b.ability.ID).First().ordering;
        }
        return (ao.CompareTo(bo));
    });
  }
  
  private void SetUpEquipment()
  {
    if (TemporaryData.GetInstance ().selectedCharacter.equipItem.Count > 0) 
    {
      CheckingEquipment (TemporaryData.GetInstance ().selectedCharacter.equipItem);
    } 
    else
    {
      equipment.GetChild(0).GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Heart"));
      equipment.GetChild (1).GetChild (0).gameObject.SetActive (false);
      equipment.GetChild(0).GetChild (1).GetComponent<Text> ().text = "Empty";
      equipment.GetChild(1).GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Heart"));
      equipment.GetChild (1).GetChild (0).gameObject.SetActive (false);
      equipment.GetChild(1).GetChild (1).GetComponent<Text> ().text = "Empty";
      equipment.GetChild(2).GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Heart"));
      equipment.GetChild (1).GetChild (0).gameObject.SetActive (false);
      equipment.GetChild(2).GetChild (1).GetComponent<Text> ().text = "Empty";
    }
  }
  
  private void CheckingEquipment(List<Item> equipItem)
  {
    for (int i = 0; i < equipment.childCount; i++)
    {
      if (i < equipItem.Count) 
      {
        Item equiped = equipItem [i];
        
        equipment.GetChild (i).GetChild (0).gameObject.SetActive (true);
        if (Resources.Load<Sprite> ("Item/Texture/" + equiped.item.name) != null) 
        {
          equipment.GetChild (i).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + equiped.item.name);
        }
        else
        {
          equipment.GetChild (i).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/BookOf" + equiped.item.itemType1);
        }
        equipment.GetChild (i).GetChild (1).GetComponent<Text> ().text = equiped.item.name;
        
        equipment.GetChild(i).GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage (equiped));
      }
      else
      {
        equipment.GetChild (i).GetChild (0).gameObject.SetActive (false);
        equipment.GetChild (i).GetChild (1).GetComponent<Text> ().text = "Empty";
        equipment.GetChild(i).GetChild(1).localPosition = Vector2.zero;
        
        equipment.GetChild(i).GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Heart"));
      } 
    }
  }

  private void GoToEquipmentPage(Item selectedItem)
  {
    CharacterStatusSceneManager.GetInstance ().equipmentPage.SetActive (true);
    CharacterStatusSceneManager.GetInstance ().statusPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().mainPage.SetActive (false);

    CharacterStatusSceneManager.GetInstance ().equipmentPage.GetComponent<ChangeEquipmentManager>().ChangingItem (selectedItem);
  }

  private void GoToEquipmentPage(string itemtype1)
  {
    CharacterStatusSceneManager.GetInstance ().equipmentPage.SetActive (true);
    CharacterStatusSceneManager.GetInstance ().statusPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().mainPage.SetActive (false);

    CharacterStatusSceneManager.GetInstance ().equipmentPage.GetComponent<ChangeEquipmentManager>().ChangingItem (itemtype1);
  }
}
