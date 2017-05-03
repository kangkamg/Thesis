using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangingAbilityInformation : MonoBehaviour 
{
  public AbilityStatus abilityStatus;
  
  public void EquipThisAbility()
  {
    CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().ShowingDetails (abilityStatus);

    if (CharacterStatusSceneManager.GetInstance ().selectedItem != this.transform) 
    {
      CharacterStatusSceneManager.GetInstance ().selectedItem = this.transform;
    } 
    else 
    {      
      CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().EquipedAbility (abilityStatus);
    }
  }
}
