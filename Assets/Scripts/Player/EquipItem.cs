using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipItem : MonoBehaviour 
{
  private void Start()
  {
    this.GetComponent<Button> ().onClick.AddListener (() => EquipThisItem());
  }

  private void EquipThisItem()
  {
    CharacterStatusSceneManager.GetInstance ().equipmentPage.GetComponent<ChangeEquipmentManager> ().TryingItem (this.GetComponent<ItemData> ());

    if (CharacterStatusSceneManager.GetInstance ().selectedItem != this.transform) 
    {
      if(CharacterStatusSceneManager.GetInstance ().selectingArrow != null) Destroy (CharacterStatusSceneManager.GetInstance ().selectingArrow.gameObject);

      CharacterStatusSceneManager.GetInstance ().selectedItem = this.transform;
      GameObject arrow = Instantiate (CharacterStatusSceneManager.GetInstance ().selectEquipmentArrow);
      CharacterStatusSceneManager.GetInstance ().selectingArrow = arrow.transform;
      arrow.transform.SetParent (this.transform.parent.parent.parent);
      arrow.transform.localPosition = new Vector2 (-196, this.transform.localPosition.y);
      arrow.name = "selectedArrow";
    } 
    else 
    {
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
