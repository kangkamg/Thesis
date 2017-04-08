using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipItem : MonoBehaviour 
{
  public void EquipThisItem()
  {
    CharacterStatusSceneManager.GetInstance ().equipmentPage.GetComponent<ChangeEquipmentManager> ().TryingItem (this.GetComponent<ItemData> ());

    if (CharacterStatusSceneManager.GetInstance ().selectedItem != this.transform) 
    {
      if(CharacterStatusSceneManager.GetInstance ().selectingArrow != null) Destroy (CharacterStatusSceneManager.GetInstance ().selectingArrow.gameObject);

      CharacterStatusSceneManager.GetInstance ().selectedItem = this.transform;
      GameObject arrow = Instantiate (CharacterStatusSceneManager.GetInstance ().selectEquipmentArrow);
      CharacterStatusSceneManager.GetInstance ().selectingArrow = arrow.transform;
      arrow.transform.SetParent (this.transform.parent.parent.parent);
      arrow.transform.localPosition = new Vector2 (-300, this.transform.localPosition.y-90);
      arrow.name = "selectedArrow";
      arrow.transform.localScale = Vector3.one;
    } 
    else 
    {
      if(CharacterStatusSceneManager.GetInstance ().selectingArrow != null) Destroy (CharacterStatusSceneManager.GetInstance ().selectingArrow.gameObject);

      for (int i = 0; i < TemporaryData.GetInstance ().playerData.inventory.Count; i++) 
      {
        if (this.GetComponent<ItemData> ().items.ordering == TemporaryData.GetInstance ().playerData.inventory [i].ordering && !TemporaryData.GetInstance ().playerData.inventory [i].equiped)
        {
          TemporaryData.GetInstance ().playerData.inventory [i].equiped = true;
          CharacterStatusSceneManager.GetInstance ().equipmentPage.GetComponent<ChangeEquipmentManager> ().EquipedItem (this.GetComponent<ItemData> ());
          break;
        }
      }
    }
  }
}
