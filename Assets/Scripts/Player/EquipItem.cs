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
      CharacterStatusSceneManager.GetInstance ().selectedItem = this.transform;
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
