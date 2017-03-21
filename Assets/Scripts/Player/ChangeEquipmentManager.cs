using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChangeEquipmentManager : MonoBehaviour
{
  public Image itemPic;
  public Text[] equipmentStatus;
  public Transform changeAbleItem;
  public Transform weaponDetail;

  public List<Item> items = new List<Item>();
  public List<GameObject> slots = new List<GameObject> ();

  public void TryingItem(ItemData equipedItem)
  {
    itemPic.sprite = Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name);
    equipmentStatus[0].text = equipedItem.items.item.name.ToString();
    equipmentStatus [1].text = (TemporaryData.GetInstance ().selectedCharacter.basicMaxHp + equipedItem.items.item.increaseHP).ToString();
    equipmentStatus[2].text = (TemporaryData.GetInstance ().selectedCharacter.basicAttack + equipedItem.items.item.increaseAttack).ToString();
    equipmentStatus[3].text = (TemporaryData.GetInstance ().selectedCharacter.basicDefense + equipedItem.items.item.increaseDefense).ToString();
    equipmentStatus[4].text = (TemporaryData.GetInstance ().selectedCharacter.basicCriRate + equipedItem.items.item.increaseCriRate).ToString();

    if (TemporaryData.GetInstance ().selectedCharacter.equipItem.Count != 0)
    {
      for (int i = 0; i < TemporaryData.GetInstance ().selectedCharacter.equipItem.Count; i++)
      {
        if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.itemType1 == equipedItem.items.item.itemType1) 
        {
          if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseHP > equipedItem.items.item.increaseHP) 
          {
            equipmentStatus [1].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
          } 
          else if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseHP < equipedItem.items.item.increaseHP)
          {
            equipmentStatus [1].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
          }

          if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseAttack > equipedItem.items.item.increaseAttack)
          {
            equipmentStatus [2].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
          } 
          else if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseAttack < equipedItem.items.item.increaseAttack)
          {
            equipmentStatus [2].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
          }

          if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseDefense > equipedItem.items.item.increaseDefense)
          {
            equipmentStatus [3].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
          }
          else if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseDefense < equipedItem.items.item.increaseDefense
          ) {
            equipmentStatus [3].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
          }

          if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseCriRate > equipedItem.items.item.increaseCriRate) 
          {
            equipmentStatus [4].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
          } 
          else if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.increaseCriRate < equipedItem.items.item.increaseCriRate) 
          {
            equipmentStatus [4].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
          }
        } 
      }
    }
    else
    {
      if (TemporaryData.GetInstance ().selectedCharacter.maxHp > TemporaryData.GetInstance ().selectedCharacter.maxHp + equipedItem.items.item.increaseHP) 
      {
        equipmentStatus [1].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
      } 
      else if (TemporaryData.GetInstance ().selectedCharacter.maxHp < TemporaryData.GetInstance ().selectedCharacter.maxHp + equipedItem.items.item.increaseHP)
      {
        equipmentStatus [1].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
      }

      if (TemporaryData.GetInstance ().selectedCharacter.attack > TemporaryData.GetInstance ().selectedCharacter.attack + equipedItem.items.item.increaseAttack)
      {
        equipmentStatus [2].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
      } 
      else if (TemporaryData.GetInstance ().selectedCharacter.attack < TemporaryData.GetInstance ().selectedCharacter.attack + equipedItem.items.item.increaseAttack)
      {
        equipmentStatus [2].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
      }

      if (TemporaryData.GetInstance ().selectedCharacter.defense > TemporaryData.GetInstance ().selectedCharacter.defense + equipedItem.items.item.increaseDefense)
      {
        equipmentStatus [3].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
      }
      else if (TemporaryData.GetInstance ().selectedCharacter.defense < TemporaryData.GetInstance ().selectedCharacter.defense + equipedItem.items.item.increaseDefense)
      {
        equipmentStatus [3].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
      }

      if (TemporaryData.GetInstance ().selectedCharacter.criRate > TemporaryData.GetInstance ().selectedCharacter.criRate + equipedItem.items.item.increaseCriRate)
      {
        equipmentStatus [4].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.red;
      } 
      else if (TemporaryData.GetInstance ().selectedCharacter.criRate < TemporaryData.GetInstance ().selectedCharacter.criRate + equipedItem.items.item.increaseCriRate)
      {
        equipmentStatus [4].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.green;
      }
    }
  }

  public void EquipedItem(ItemData equipedItem)
  {
    equipmentStatus [1].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.white;
    equipmentStatus [2].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.white;
    equipmentStatus [3].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.white;
    equipmentStatus [4].transform.parent.transform.GetChild (1).GetComponent<Image> ().color = Color.white;

    if (!CheckingIfEquipedThisItemType (equipedItem))
    {
      TemporaryData.GetInstance ().selectedCharacter.equipItem.Add (equipedItem.items);
    }

    itemPic.sprite = Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name);
    equipmentStatus[0].text = equipedItem.items.item.name.ToString();
    equipmentStatus [1].text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
    equipmentStatus[2].text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString();
    equipmentStatus[3].text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString();
    equipmentStatus[4].text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString();

    GenerateInventoryItem (equipedItem.items);
  }

  private bool CheckingIfEquipedThisItemType(ItemData equipedItem)
  {
    for (int i = 0; i < TemporaryData.GetInstance ().selectedCharacter.equipItem.Count; i++)
    {
      if (TemporaryData.GetInstance ().selectedCharacter.equipItem [i].item.itemType1 == equipedItem.items.item.itemType1) 
      {
        for (int j = 0; j < TemporaryData.GetInstance ().playerData.inventory.Count; j++) 
        {
          if (TemporaryData.GetInstance ().playerData.inventory [j].ordering == TemporaryData.GetInstance ().selectedCharacter.equipItem [i].ordering) 
          {
            TemporaryData.GetInstance ().playerData.inventory [j].equiped = false;
            break;
          }
        }
        TemporaryData.GetInstance ().selectedCharacter.equipItem [i] = equipedItem.items;
        return true;
      } 
    }
    return false;
  }

  public void ChangingItem(Item selectedItem)
  {
    GenerateInventoryItem (selectedItem);

    itemPic.sprite = Resources.Load<Sprite> ("Item/Texture/" + selectedItem.item.name);
    equipmentStatus[0].text = selectedItem.item.name.ToString();
    equipmentStatus [1].text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
    equipmentStatus[2].text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString();
    equipmentStatus[3].text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString();
    equipmentStatus[4].text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString();
  }

  public void ChangingItem(string itemtype1)
  {
    GenerateInventoryItem (itemtype1);

    equipmentStatus[0].text = itemtype1.ToString();
    equipmentStatus[1].text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
    equipmentStatus[2].text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString();
    equipmentStatus[3].text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString();
    equipmentStatus[4].text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString();
  }
    
  public void GenerateInventoryItem(Item selectedItem)
  {
    foreach (GameObject a in slots)
    {
      Destroy (a);
    }
    slots.Clear ();
    items.Clear ();

    for (int i = 0; i < TemporaryData.GetInstance().playerData.inventory.Count; i++)
    {
      if (TemporaryData.GetInstance().playerData.inventory [i].item.itemType1 == selectedItem.item.itemType1 && !TemporaryData.GetInstance().playerData.inventory[i].equiped)
      {
        if (TemporaryData.GetInstance().playerData.inventory[i].item.stackable && CheckIfItemIsExists(TemporaryData.GetInstance().playerData.inventory[i])) 
        {
          for (int j = 0; j < slots.Count; j++) 
          {
            if (slots [j].GetComponent<ItemData>().items.item.ID == TemporaryData.GetInstance().playerData.inventory[i].item.ID) 
            {
              items.Add (TemporaryData.GetInstance().playerData.inventory[i]);
              items[items.Count-1].ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
              ItemData data = slots [j].GetComponent<ItemData> ();
              data.amount++;
              data.transform.GetChild (2).GetComponent<Text> ().text = data.amount.ToString ();
              break;
            }
          }
        }
        else
        {
          items.Add (TemporaryData.GetInstance().playerData.inventory[i]);
          GameObject itemObj = Instantiate (CharacterStatusSceneManager.GetInstance().changingItemObj);
          itemObj.transform.SetParent (changeAbleItem.transform);
          slots.Add (itemObj);
          itemObj.GetComponent<ItemData> ().items = TemporaryData.GetInstance().playerData.inventory [i];

          Sprite sprite = new Sprite();
          if (Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name);
          else sprite = Resources.Load<Sprite>("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory[0].item.name);

          itemObj.GetComponent<ItemData> ().items.ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
          itemObj.GetComponent<ItemData> ().amount = 1;
          itemObj.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
          itemObj.transform.GetChild (1).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().items.item.name.ToString();
          itemObj.transform.GetChild (2).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().amount.ToString ();
        }
      }
    }

    if (slots.Count > 6)
    {
      changeAbleItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 80 * (slots.Count - 1));
      changeAbleItem.transform.localPosition = changeAbleItem.GetComponent<RectTransform> ().sizeDelta / -(slots.Count - 2);
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }

    items.Sort (delegate(Item a, Item b) 
      {
        return (a.ordering.CompareTo (b.ordering));
      });
  }

  public void GenerateInventoryItem(string itemType)
  {
    foreach (GameObject a in slots)
    {
      Destroy (a);
    }
    slots.Clear ();
    items.Clear ();

    for (int i = 0; i < TemporaryData.GetInstance().playerData.inventory.Count; i++)
    {
      if (TemporaryData.GetInstance().playerData.inventory [i].item.itemType1 == itemType && !TemporaryData.GetInstance().playerData.inventory[i].equiped)
      {
        if (TemporaryData.GetInstance().playerData.inventory[i].item.stackable && CheckIfItemIsExists(TemporaryData.GetInstance().playerData.inventory[i])) 
        {
          for (int j = 0; j < slots.Count; j++) 
          {
            if (slots [j].GetComponent<ItemData>().items.item.ID == TemporaryData.GetInstance().playerData.inventory[i].item.ID) 
            {
              items.Add (TemporaryData.GetInstance().playerData.inventory[i]);
              items[items.Count-1].ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
              ItemData data = slots [j].GetComponent<ItemData> ();
              data.amount++;
              data.transform.GetChild (2).GetComponent<Text> ().text = data.amount.ToString ();
              break;
            }
          }
        }
        else
        {
          items.Add (TemporaryData.GetInstance().playerData.inventory[i]);
          GameObject itemObj = Instantiate (CharacterStatusSceneManager.GetInstance().changingItemObj);
          itemObj.transform.SetParent (changeAbleItem.transform);
          slots.Add (itemObj);
          itemObj.GetComponent<ItemData> ().items = TemporaryData.GetInstance().playerData.inventory [i];
         
          Sprite sprite = new Sprite();
          if (Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name);
          else sprite = Resources.Load<Sprite>("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory[0].item.name);

          itemObj.GetComponent<ItemData> ().items.ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
          itemObj.GetComponent<ItemData> ().amount = 1;
          itemObj.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
          itemObj.transform.GetChild (1).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().items.item.name.ToString();
          itemObj.transform.GetChild (2).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().amount.ToString ();
        }
      }
    }

    if (slots.Count > 6)
    {
      changeAbleItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 80 * (slots.Count - 1));
      changeAbleItem.transform.localPosition = changeAbleItem.GetComponent<RectTransform> ().sizeDelta / -(slots.Count - 2);
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }

    items.Sort (delegate(Item a, Item b) 
      {
        return (a.ordering.CompareTo (b.ordering));
      });
  }

  private bool CheckIfItemIsExists(Item item)
  {
    for (int i = 0; i < items.Count; i++) 
    {
      if (items [i].item.ID == item.item.ID) 
      {
        return true;
      }
    }
    return false;
  }
}
