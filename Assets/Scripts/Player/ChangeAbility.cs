using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChangeAbility : MonoBehaviour 
{
  int changingAbilityID;
  public int changingAbilityOrdering;
  public Transform changeAbleAbilitySlots;
  public Transform changingDetails;
  public Transform changeAbleDetails;
  
  public GameObject abilityInSlots;
  
  public void SetUpAbility(AbilityStatus abilityStatus)
  {
    HideAbility ();
    changingAbilityID = abilityStatus.ability.ID;
    changingDetails.gameObject.SetActive (true); 
    changingDetails.GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + abilityStatus.ability.ID);
    changingDetails.GetChild (1).GetChild (0).GetComponent<Text> ().text = abilityStatus.ability.abilityName.ToString ();
    changingDetails.GetChild (2).GetComponent<Text> ().text = abilityStatus.ability.describe.ToString ();
    changingDetails.GetChild (3).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.power.ToString ();
    changingDetails.GetChild (4).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.hitAmount.ToString ();
    changingDetails.GetChild (5).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.abilityEff.ToString ();
  }
  
  public void HideAbility()
  {
    changingDetails.gameObject.SetActive (false); 
    changeAbleDetails.gameObject.SetActive (false);
    changingAbilityID = -1;
  }
  
  public void GenerateAbility(AbilityStatus abilityStatus)
  {
    GenerateAbility (abilityStatus.ability.abilityType);
    SetUpAbility (abilityStatus);
  }
  
  public void GenerateAbility(int type)
  {
    foreach (Transform child in changeAbleAbilitySlots)
    {
      Destroy (child.gameObject);
    }
    Destroy(GameObject.Find("selectedArrow"));
    
    
    List<AbilityStatus> changeAbleAbility = TemporaryData.GetInstance ().selectedCharacter.learnedAbility.Where (x => x.ability.abilityType == type || x.ability.abilityType == -type).ToList ();
    
    for (int i = 0; i < changeAbleAbility.Count; i++)
    {
      if (CheckingEquipedAbility(changeAbleAbility [i].ability.ID,type)) 
      {
        GameObject abilityObj = Instantiate (abilityInSlots);
        abilityObj.transform.SetParent (changeAbleAbilitySlots);
        abilityObj.transform.localScale = Vector3.one;
        abilityObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Ability/" + changeAbleAbility [i].ability.ID);
        abilityObj.transform.GetChild (1).GetComponent<Text> ().text = changeAbleAbility [i].ability.abilityName;
        abilityObj.GetComponent<ChangingAbilityInformation> ().abilityStatus = changeAbleAbility [i];
      }
    }
    
    if (changeAbleAbility.Count > 5)
    {
      changeAbleAbilitySlots.GetComponent<RectTransform> ().sizeDelta = new Vector2 (changeAbleAbilitySlots.GetComponent<RectTransform> ().sizeDelta.x , 255f * (changeAbleAbility.Count));
      changeAbleAbilitySlots.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      changeAbleAbilitySlots.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }

    changeAbleAbilitySlots.GetComponent<RectTransform> ().offsetMax = new Vector2 (changeAbleAbilitySlots.GetComponent<RectTransform> ().offsetMax.x, 1f);
  }
  
  public bool CheckingEquipedAbility(int ID,int type)
  {
    List<AbilityStatus> equipedAbility = TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.abilityType == type || x.ability.abilityType == -type).ToList ();
    for (int i = 0; i < equipedAbility.Count; i++) 
    {
      if (equipedAbility[i].ability.ID == ID) 
      {
        return false;
      }
    }
    return true;
  }
  
  public void ShowingDetails(AbilityStatus abilityStatus)
  {
    changeAbleDetails.gameObject.SetActive (true);
    changeAbleDetails.GetChild (1).GetChild (0).GetComponent<Text> ().text = abilityStatus.ability.describe.ToString ();
    changeAbleDetails.GetChild (1).GetChild (1).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.power.ToString ();
    changeAbleDetails.GetChild (1).GetChild (2).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.hitAmount.ToString ();
    changeAbleDetails.GetChild (1).GetChild (3).GetChild(0).GetComponent<Text> ().text = abilityStatus.ability.abilityEff.ToString ();
  }
  
  public void EquipedAbility(AbilityStatus abilityStatus)
  {
    if (TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.ID == changingAbilityID).Count () > 0) 
      TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Remove (TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Where (x => x.ability.ID == changingAbilityID).First ());
    
    TemporaryData.GetInstance ().selectedCharacter.equipedAbility.Add (abilityStatus);
    changingAbilityID = abilityStatus.ability.ID;
    GenerateAbility (abilityStatus);
  }
}
