using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class InventoryManager : MonoBehaviour
{
  public GameObject inventoryPanel;
  public GameObject slotPanel;
  public GameObject inventoryItem;

  public List<Item> items = new List<Item>();
  public List<GameObject> slots = new List<GameObject> ();

  private void Start()
  {
    transform.GetChild (0).FindChild ("Heart").GetComponent<Toggle> ().isOn = true;
    GenerateInventoryItem ("Heart");
  }
    
  public void GenerateInventoryItem(string itemType)
  {
    foreach (GameObject a in slots)
    {
      Destroy (a);
    }
    slots.Clear ();
    items.Clear ();
    this.transform.GetChild (2).gameObject.SetActive (false);

    for (int i = 0; i < TemporaryData.GetInstance().playerData.inventory.Count; i++)
    {
      if (TemporaryData.GetInstance().playerData.inventory [i].item.itemType1 == itemType)
      {
        if (TemporaryData.GetInstance().playerData.inventory[i].item.stackable && CheckIfItemIsInInventory(TemporaryData.GetInstance().playerData.inventory[i])) 
        {
          for (int j = 0; j < slots.Count; j++) 
          {
            if (slots [j].GetComponent<ItemData>().items.item.ID == TemporaryData.GetInstance().playerData.inventory[i].item.ID) 
            {
              items.Add (TemporaryData.GetInstance().playerData.inventory[i]);
              items [items.Count - 1].ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
              ItemData data = slots [j].GetComponent<ItemData> ();
              data.amount++;
              data.transform.GetChild (2).GetComponent<Text> ().text = data.amount.ToString ();
              break;
            }
          }
        }
        else
        {
          items.Add (TemporaryData.GetInstance().playerData.inventory [i]);
          GameObject itemObj = Instantiate (inventoryItem);
          itemObj.transform.SetParent (slotPanel.transform);
          itemObj.GetComponent<ItemData> ().items = TemporaryData.GetInstance().playerData.inventory [i];
          itemObj.transform.GetChild (1).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().items.item.name.ToString();

          Sprite sprite = new Sprite();
          if (Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + TemporaryData.GetInstance().playerData.inventory [i].item.name);
          else sprite = Resources.Load<Sprite>("Item/Texture/BookOf" + TemporaryData.GetInstance().playerData.inventory[i].item.itemType1);

          itemObj.GetComponent<ItemData> ().items.ordering = TemporaryData.GetInstance().playerData.inventory[i].ordering;
          itemObj.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
          slots.Add (itemObj);
          itemObj.GetComponent<ItemData> ().amount = 1;
          itemObj.transform.GetChild (2).GetComponent<Text> ().text = itemObj.GetComponent<ItemData> ().amount.ToString ();
          itemObj.transform.localScale = Vector3.one;
          itemObj.GetComponent<Button> ().onClick.AddListener (() => ShowingItemInformation (itemObj.GetComponent<ItemData> ().items));
        }
      }
    }

    if (slots.Count > 8)
    {
      slotPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (slotPanel.GetComponent<RectTransform> ().sizeDelta.x, 255f * (slots.Count));
      slotPanel.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
      slotPanel.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
    } 
    else 
    {
      if(slots.Count < 1) slotPanel.transform.parent.parent.GetChild(1).GetComponent<Text>().text = "Empty Inventory";
      else slotPanel.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
      
      slotPanel.GetComponentInParent<ScrollRect> ().vertical = false;
    }
    
    slotPanel.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -slotPanel.GetComponent<RectTransform> ().rect.height / 2);
    
    items.Sort (delegate(Item a, Item b) 
      {
        return (a.item.ID.CompareTo (b.item.ID));
      });
  }

  public void AddItem(int ID)
  {
    Item itemToAdd = new Item();

    itemToAdd.item = GetDataFromSql.GetItemFromID (ID);

    items.Add(itemToAdd);
  }

  private bool CheckIfItemIsInInventory(Item item)
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
  
  private void ShowingItemInformation(Item itemStatus)
  {
    this.transform.GetChild (2).gameObject.SetActive (true);
    this.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text> ().text = "HP : " + itemStatus.item.increaseHP.ToString () +
      "\t\tATK : " + itemStatus.item.increaseAttack.ToString () + "\nDEF : " + itemStatus.item.increaseDefense.ToString ()
      + "\t\tCRI.R : " + itemStatus.item.increaseCriRate.ToString ();
  }
}
