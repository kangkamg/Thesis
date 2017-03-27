using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowingCharacterStatusManager : MonoBehaviour 

{
  public Image chaImg;
  public Text characterName;
  public Text[] status;
  public Transform[] equipment;
  public Transform skill;

  public void UpdateStatus()
  {
    foreach (Transform a in skill) 
    {
      Destroy (a.gameObject);
    }

    chaImg.sprite = Resources.Load<Sprite> ("PlayerPrefab/" + TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName);
    characterName.text = TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName.ToString ();

    status [0].text = TemporaryData.GetInstance ().selectedCharacter.characterLevel.ToString();
    status [1].text = TemporaryData.GetInstance ().selectedCharacter.experience.ToString();
    status [2].text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
    status [3].text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString();
    status [4].text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString();
    status [5].text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString();

    if (TemporaryData.GetInstance ().selectedCharacter.equipItem.Count > 0) 
    {
      for (int i = 0; i < TemporaryData.GetInstance ().selectedCharacter.equipItem.Count; i++) 
      {
        CheckingEquipment (TemporaryData.GetInstance ().selectedCharacter.equipItem [i]);
      }
    } 
    else
    {
      equipment [0].GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Weapon"));
      equipment [0].GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/Weapon");
      equipment [0].GetChild (1).GetComponent<Text> ().text = "Weapon";
      equipment [1].GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Armor"));
      equipment [1].GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/Armor");
      equipment [1].GetChild (1).GetComponent<Text> ().text = "Armor";
      equipment [2].GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Accessory"));
      equipment [2].GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/Accessory");
      equipment [2].GetChild (1).GetComponent<Text> ().text = "Accessory";
      equipment [3].GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage ("Items"));
      equipment [3].GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/Items");
      equipment [3].GetChild (1).GetComponent<Text> ().text = "Items";
    }

    if (TemporaryData.GetInstance ().selectedCharacter.normalAttack!=null)
    {
      GameObject skillObj = Instantiate(CharacterStatusSceneManager.GetInstance ().skillObj);
      skillObj.transform.SetParent (skill);
      skillObj.GetComponent<SkillStatusManager> ().ability = TemporaryData.GetInstance ().selectedCharacter.normalAttack;
      skillObj.transform.localScale = new Vector3 (1, 1, 1);
    }
    if (TemporaryData.GetInstance ().selectedCharacter.specialAttack!=null)
    {
      GameObject skillObj = Instantiate(CharacterStatusSceneManager.GetInstance ().skillObj);
      skillObj.transform.SetParent (skill);
      skillObj.GetComponent<SkillStatusManager> ().ability = TemporaryData.GetInstance ().selectedCharacter.specialAttack;
      skillObj.transform.localScale = new Vector3 (1, 1, 1);
    }
  }

  private void CheckingEquipment(Item equipItem)
  {
    for (int i = 0; i < equipment.Length; i++)
    {
      if (equipItem.item.itemType1 == equipment [i].name) 
      {
        equipment [i].GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + equipItem.item.name);
        equipment [i].GetChild (1).GetComponent<Text> ().text = equipItem.item.name;
        equipment [i].GetComponent<Button> ().onClick.AddListener (() => GoToEquipmentPage (equipItem));
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
