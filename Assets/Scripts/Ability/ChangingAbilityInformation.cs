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
      if(CharacterStatusSceneManager.GetInstance ().selectingArrow != null) Destroy (CharacterStatusSceneManager.GetInstance ().selectingArrow.gameObject);

      CharacterStatusSceneManager.GetInstance ().selectedItem = this.transform;
      GameObject arrow = Instantiate (CharacterStatusSceneManager.GetInstance ().selectEquipmentArrow);
      CharacterStatusSceneManager.GetInstance ().selectingArrow = arrow.transform;
      arrow.transform.SetParent (this.transform.parent.parent.parent);
      arrow.transform.localPosition = new Vector2 (-300, this.transform.localPosition.y+15);
      arrow.name = "selectedArrow";
      arrow.transform.localScale = Vector3.one;
    } 
    else 
    {
      if(CharacterStatusSceneManager.GetInstance ().selectingArrow != null) Destroy (CharacterStatusSceneManager.GetInstance ().selectingArrow.gameObject);
      
      CharacterStatusSceneManager.GetInstance ().abilityPage.GetComponent<ChangeAbility> ().EquipedAbility (abilityStatus);
    }
  }
}
