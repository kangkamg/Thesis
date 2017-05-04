using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChangeEquipmentManager : MonoBehaviour
{ 
  public Transform changeAbleItem;
  public Transform weaponDetail;

  public List<Item> items = new List<Item>();
  public List<GameObject> slots = new List<GameObject> ();

  public void TryingItem(ItemData equipedItem)
  {
    this.transform.GetChild (0).gameObject.SetActive (true);
    this.transform.GetChild (1).gameObject.SetActive (false);
    Transform equipedWeaponStatus = this.transform.GetChild (1);
    Transform firstWeaponStatus = this.transform.GetChild (0).GetChild(1);
    Transform secondWeaponStatus = this.transform.GetChild (0).GetChild(2);
    Transform isChanging = this.transform.GetChild (0).GetChild (3);
    
    if (equipedWeaponStatus.GetChild (1).GetComponent<Image> ().sprite != null) 
    {
      firstWeaponStatus.GetChild (0).gameObject.SetActive (true);
      firstWeaponStatus.GetChild (0).GetComponent<Image> ().sprite = equipedWeaponStatus.GetChild (1).GetComponent<Image> ().sprite;
    }
    else
      firstWeaponStatus.GetChild (0).gameObject.SetActive (false);
    firstWeaponStatus.GetChild(0).GetChild(0).GetComponent<Text>().text = equipedWeaponStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text;
    firstWeaponStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text = equipedWeaponStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text;
    firstWeaponStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text = equipedWeaponStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text;
    firstWeaponStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text = equipedWeaponStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text;
    firstWeaponStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text = equipedWeaponStatus.GetChild (5).GetChild (0).GetComponent<Text> ().text;
    
    if (Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name) != null)
      secondWeaponStatus.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name);
    else
      secondWeaponStatus.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/BookOf" + equipedItem.items.item.itemType1);
    secondWeaponStatus.GetChild (0).GetChild (0).GetComponent<Text> ().text = equipedItem.items.item.name;
    secondWeaponStatus.GetChild(1).GetChild(0).GetComponent<Text>().text  = equipedItem.items.item.increaseHP.ToString ();
    secondWeaponStatus.GetChild(2).GetChild(0).GetComponent<Text>().text = equipedItem.items.item.increaseAttack.ToString ();
    secondWeaponStatus.GetChild(3).GetChild(0).GetComponent<Text>().text  = equipedItem.items.item.increaseDefense.ToString ();
    secondWeaponStatus.GetChild(4).GetChild(0).GetComponent<Text>().text = equipedItem.items.item.increaseCriRate.ToString ();
    
    if (int.Parse (firstWeaponStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text.ToString ()) < equipedItem.items.item.increaseHP)
    {
      isChanging.GetChild (0).gameObject.SetActive (true);
      isChanging.GetChild (0).GetComponent<Image> ().color = Color.green;
      isChanging.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
    }
    else if (int.Parse (firstWeaponStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text.ToString ()) > equipedItem.items.item.increaseHP)
    {
      isChanging.GetChild (0).gameObject.SetActive (true);
      isChanging.GetChild (0).GetComponent<Image> ().color = Color.red;
      isChanging.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
    }
    else
    {
      isChanging.GetChild (0).gameObject.SetActive (false);
    }
      
    if (int.Parse (firstWeaponStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text.ToString ()) < equipedItem.items.item.increaseAttack)
    {
      isChanging.GetChild (1).gameObject.SetActive (true);
      isChanging.GetChild (1).GetComponent<Image> ().color = Color.green;
      isChanging.GetChild (1).rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
    }
    else if (int.Parse (firstWeaponStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text.ToString ()) > equipedItem.items.item.increaseAttack)
    {
      isChanging.GetChild (1).gameObject.SetActive (true);
      isChanging.GetChild (1).GetComponent<Image> ().color = Color.red;
      isChanging.GetChild (1).rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
    }
    else
    {
      isChanging.GetChild (1).gameObject.SetActive (false);
    }
      
    if (int.Parse (firstWeaponStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text.ToString ()) < equipedItem.items.item.increaseDefense)
    {
      isChanging.GetChild (2).gameObject.SetActive (true);
      isChanging.GetChild (2).GetComponent<Image> ().color = Color.green;
      isChanging.GetChild (2).rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
    }
    else if (int.Parse (firstWeaponStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text.ToString ()) > equipedItem.items.item.increaseDefense)
    {
      isChanging.GetChild (2).gameObject.SetActive (true);
      isChanging.GetChild (2).GetComponent<Image> ().color = Color.red;
      isChanging.GetChild (2).rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
    }
    else
    {
      isChanging.GetChild (2).gameObject.SetActive (false);
    }
      
    if (int.Parse (firstWeaponStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text.ToString ()) < equipedItem.items.item.increaseCriRate)
    {
      isChanging.GetChild (3).gameObject.SetActive (true);
      isChanging.GetChild (3).GetComponent<Image> ().color = Color.green;
      isChanging.GetChild (3).rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
    }
    else if (int.Parse (firstWeaponStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text.ToString ()) > equipedItem.items.item.increaseCriRate)
    {
      isChanging.GetChild (3).gameObject.SetActive (true);
      isChanging.GetChild (3).GetComponent<Image> ().color = Color.red;
      isChanging.GetChild (3).rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
    }
    else
    {
      isChanging.GetChild (3).gameObject.SetActive (false);
    }
  }

  public void EquipedItem(ItemData equipedItem)
  {
    if (!CheckingIfEquipedThisItemType (equipedItem))
    {
      TemporaryData.GetInstance ().selectedCharacter.equipItem.Add (equipedItem.items);
    }
    
    this.transform.GetChild (0).gameObject.SetActive (false);
    this.transform.GetChild (1).gameObject.SetActive (true);
    Transform equipedWeaponStatus = this.transform.GetChild (1);

    if(Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name) != null)
      equipedWeaponStatus.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite> ("Item/Texture/" + equipedItem.items.item.name);
    else
      equipedWeaponStatus.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite> ("Item/Texture/BookOf" + equipedItem.items.item.itemType1);
    
    equipedWeaponStatus.GetChild(1).GetChild(0).GetComponent<Text>().text  = equipedItem.items.item.name.ToString();
    equipedWeaponStatus.GetChild(2).GetChild(0).GetComponent<Text>().text  = equipedItem.items.item.increaseHP.ToString ();
    equipedWeaponStatus.GetChild(3).GetChild(0).GetComponent<Text>().text = equipedItem.items.item.increaseAttack.ToString ();
    equipedWeaponStatus.GetChild(4).GetChild(0).GetComponent<Text>().text  = equipedItem.items.item.increaseDefense.ToString ();
    equipedWeaponStatus.GetChild(5).GetChild(0).GetComponent<Text>().text = equipedItem.items.item.increaseCriRate.ToString ();
    
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
    
    this.transform.GetChild (0).gameObject.SetActive (false);
    this.transform.GetChild (1).gameObject.SetActive (true);
    Transform equipedWeaponStatus = this.transform.GetChild (1);
    
    equipedWeaponStatus.GetChild (1).gameObject.SetActive (true);
    equipedWeaponStatus.GetChild (2).gameObject.SetActive (true);
    equipedWeaponStatus.GetChild(3).gameObject.SetActive (true);
    equipedWeaponStatus.GetChild(4).gameObject.SetActive (true);
    equipedWeaponStatus.GetChild(5).gameObject.SetActive (true);
    
    if (Resources.Load<Sprite> ("Item/Texture/" + selectedItem.item.name) != null) 
    {
      equipedWeaponStatus.GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/" + selectedItem.item.name);
    }
    else
    {
      equipedWeaponStatus.GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Item/Texture/BookOf" + selectedItem.item.itemType1);
    }
    equipedWeaponStatus.GetChild(1).GetChild(0).GetComponent<Text>().text  = selectedItem.item.name.ToString();
    equipedWeaponStatus.GetChild(2).GetChild(0).GetComponent<Text>().text  = selectedItem.item.increaseHP.ToString ();
    equipedWeaponStatus.GetChild(3).GetChild(0).GetComponent<Text>().text = selectedItem.item.increaseAttack.ToString ();
    equipedWeaponStatus.GetChild(4).GetChild(0).GetComponent<Text>().text  = selectedItem.item.increaseDefense.ToString ();
    equipedWeaponStatus.GetChild(5).GetChild(0).GetComponent<Text>().text = selectedItem.item.increaseCriRate.ToString ();
  }

  public void ChangingItem(string itemtype1)
  {
    GenerateInventoryItem (itemtype1);

    this.transform.GetChild (0).gameObject.SetActive (false);
    this.transform.GetChild (1).gameObject.SetActive (true);
    Transform equipedWeaponStatus = this.transform.GetChild (1);
    
    equipedWeaponStatus.GetChild (1).gameObject.SetActive (false);
    equipedWeaponStatus.GetChild (2).gameObject.SetActive (false);
    equipedWeaponStatus.GetChild(3).gameObject.SetActive (false);
    equipedWeaponStatus.GetChild(4).gameObject.SetActive (false);
    equipedWeaponStatus.GetChild(5).gameObject.SetActive (false);
  }
    
  public void GenerateInventoryItem(Item selectedItem)
  {
    foreach (GameObject a in slots)
    {
      Destroy (a);
    }
    slots.Clear ();
    items.Clear ();
    Destroy(GameObject.Find("selectedArrow"));
    this.transform.GetChild (2).gameObject.SetActive (true);

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
          else sprite = Resources.Load<Sprite>("Item/Texture/BookOf" + TemporaryData.GetInstance().playerData.inventory[i].item.itemType1);

          itemObj.GetComponent<ItemData> ().items.ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
          itemObj.GetComponent<ItemData> ().amount = 1;
          itemObj.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
          itemObj.transform.GetChild (1).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().items.item.name.ToString();
          itemObj.transform.GetChild (2).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().amount.ToString ();

          itemObj.transform.localScale = Vector3.one;
        }
      }
    }
   
    if (slots.Count > 5)
    {
      changeAbleItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (changeAbleItem.GetComponent<RectTransform> ().sizeDelta.x , 255f * (slots.Count));
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
      changeAbleItem.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
    } 
    else
    {
      if(slots.Count < 1) changeAbleItem.transform.parent.parent.GetChild(1).GetComponent<Text>().text = "None Equipable Item";
      else changeAbleItem.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
      changeAbleItem.GetComponentInParent<ScrollRect> ().vertical = false;
    }

    changeAbleItem.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -changeAbleItem.GetComponent<RectTransform> ().rect.height / 2);

    items.Sort (delegate(Item a, Item b) 
      {
        return (a.item.ID.CompareTo (b.item.ID));
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
    this.transform.GetChild (2).gameObject.SetActive (true);

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
          else sprite = Resources.Load<Sprite>("Item/Texture/BookOf" + TemporaryData.GetInstance().playerData.inventory[i].item.itemType1);

          itemObj.GetComponent<ItemData> ().items.ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
          itemObj.GetComponent<ItemData> ().amount = 1;
          itemObj.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
          itemObj.transform.GetChild (1).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().items.item.name.ToString();
          itemObj.transform.GetChild (2).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().amount.ToString ();
          itemObj.transform.localScale = Vector3.one;
        }
      }
    }

    if (slots.Count > 5)
    {
      changeAbleItem.GetComponent<RectTransform> ().sizeDelta = new Vector2 (changeAbleItem.GetComponent<RectTransform> ().sizeDelta.x , 255f * (slots.Count));
      changeAbleItem.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
      changeAbleItem.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
    } 
    else
    {
      if(slots.Count < 1) changeAbleItem.transform.parent.parent.GetChild(1).GetComponent<Text>().text = "None Equipable Item";
      else changeAbleItem.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
      changeAbleItem.GetComponentInParent<ScrollRect> ().vertical = false;
    }
    changeAbleItem.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -changeAbleItem.GetComponent<RectTransform> ().rect.height / 2);

    items.Sort (delegate(Item a, Item b) 
      {
        return (a.item.ID.CompareTo (b.item.ID));
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
