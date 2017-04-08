using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilityInformation : MonoBehaviour

{
  public AbilityStatus abilityStatus = new AbilityStatus();
  public int ordering;
  
  public void SetUpAbilityStatus(AbilityStatus setup)
  {
    abilityStatus = setup;
    this.GetComponent<Button> ().onClick.AddListener (() => GoToAbilityPage (setup));
  }
  
  public void SetUpAbilityStatus(int type)
  {
    this.GetComponent<Button> ().onClick.AddListener (() => GoToAbilityPage (type));
  }
  
  public void GoToAbilityPage(AbilityStatus selectedAbility)
  {
    CharacterStatusSceneManager.GetInstance ().abilityPage.SetActive (true);
    CharacterStatusSceneManager.GetInstance ().equipmentPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().statusPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().mainPage.SetActive (false);
    
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().GenerateAbility (selectedAbility);
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().changingAbilityOrdering = ordering;
  }
  
  public void GoToAbilityPage(int type)
  {
    CharacterStatusSceneManager.GetInstance ().abilityPage.SetActive (true);
    CharacterStatusSceneManager.GetInstance ().equipmentPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().statusPage.SetActive (false);
    CharacterStatusSceneManager.GetInstance ().mainPage.SetActive (false);
    
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().HideAbility ();
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().GenerateAbility (type);
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().changingAbilityOrdering = ordering;
  }
}
