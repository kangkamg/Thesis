using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemManager : MonoBehaviour 
{
  private static ItemManager instance;
  public static ItemManager GetInstance()
  {
    return instance;
  }

  public GameObject showing;

  public Text goldText;
  public bool isBuying;
  
  private List<ItemStatus> canBuy = new List<ItemStatus>();
  public List<GameObject> allItem = new List<GameObject> ();

  public void Awake()
  {
    instance = this;
    canBuy = GetDataFromSql.GetItemFromMap (PlayerPrefs.GetInt (Const.TownSceneNo, 1).ToString ());
    goldText.text = TemporaryData.GetInstance ().playerData.gold.ToString ();
  }

  public void GenerateItem(string itemsType)
  {
    if (isBuying)   
      GenerateBuyingItems (itemsType);
    else
      GenerateInventoryItems (itemsType);
  }
  
  public void GenerateBuyingItems(string itemsType)
  {
    foreach (GameObject a in allItem)
    {
      Destroy (a);
    }
    allItem.Clear ();

    for (int i = 0; i < canBuy.Count; i++)
    {
      if (canBuy [i].itemType1 == itemsType)
      {
        GameObject items = Instantiate (Resources.Load<GameObject> ("Item/Item"));
        items.transform.SetParent (showing.transform);
        items.GetComponent<ItemData> ().items.item = canBuy [i];
        items.transform.GetChild (1).GetComponent<Text> ().text = items.GetComponent<ItemData> ().items.item.name.ToString();
        items.transform.GetChild (2).GetComponent<Text> ().text = items.GetComponent<ItemData> ().items.item.price.ToString();
        Sprite sprite = new Sprite();
        if (Resources.Load<Sprite> ("Item/Texture/" + canBuy [i].name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + canBuy [i].name);
        else sprite = Resources.Load<Sprite>("Item/Texture/" + canBuy[0].name);
         
        items.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
        items.transform.localScale = Vector3.one;
        items.GetComponent<Button> ().onClick.AddListener (() => BuyingItem (items.GetComponent<ItemData>().items.item));
        allItem.Add (items);
      }
    }

    if (allItem.Count > 6)
    {
      showing.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 120 * (allItem.Count - 1));
      showing.transform.localPosition = showing.GetComponent<RectTransform> ().sizeDelta / -(allItem.Count - 2);
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }
    for (int i = 0; i < allItem.Count; i++)
    {
      allItem[i].transform.localPosition = new Vector3 (1, (showing.GetComponent<RectTransform>().sizeDelta.y/2) - (80*i), 0);
    }
  }
  
  public void GenerateInventoryItems(string itemsType)
  {
    foreach (GameObject a in allItem)
    {
      Destroy (a);
    }
    allItem.Clear ();

    List<Item> inventoryItem = TemporaryData.GetInstance ().playerData.inventory.Where (x => !x.equiped && x.item.itemType1 == itemsType).ToList ();
    
    for (int i = 0; i < inventoryItem.Count; i++)
    {
      GameObject items = Instantiate (Resources.Load<GameObject> ("Item/Item"));
      items.transform.SetParent (showing.transform);
      items.GetComponent<ItemData> ().items = inventoryItem [i];
      items.transform.GetChild (1).GetComponent<Text> ().text = items.GetComponent<ItemData> ().items.item.name.ToString();
      items.transform.GetChild (2).GetComponent<Text> ().text = items.GetComponent<ItemData> ().items.item.price.ToString();
      Sprite sprite = new Sprite();
      if (Resources.Load<Sprite> ("Item/Texture/" + inventoryItem [i].item.name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + inventoryItem [i].item.name);
      else sprite = Resources.Load<Sprite>("Item/Texture/" + inventoryItem[0].item.name);

      items.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
      items.transform.localScale = Vector3.one;
      items.GetComponent<Button> ().onClick.AddListener (() => SellingItem (items.GetComponent<ItemData>().items));
      allItem.Add (items);
    }

    if (allItem.Count > 6)
    {
      showing.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 120 * (allItem.Count - 1));
      showing.transform.localPosition = showing.GetComponent<RectTransform> ().sizeDelta / -(allItem.Count - 2);
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }
    for (int i = 0; i < allItem.Count; i++)
    {
      allItem[i].transform.localPosition = new Vector3 (1, (showing.GetComponent<RectTransform>().sizeDelta.y/2) - (80*i), 0);
    }
  }
  
  private void BuyingItem(ItemStatus data)
  {
    if (TemporaryData.GetInstance ().playerData.gold >= data.price) 
    {
      Item newItem = new Item ();
      newItem.item = data;
      newItem.equiped = false;
      newItem.ordering = TemporaryData.GetInstance ().playerData.inventory.Count;
      
      TemporaryData.GetInstance ().playerData.inventory.Add (newItem);
      TemporaryData.GetInstance ().playerData.gold -= newItem.item.price;
      
      goldText.text = TemporaryData.GetInstance ().playerData.gold.ToString ();
    } 
    else
    {
      Debug.Log ("NotEnoughGold");
    }
  }
  
  private void SellingItem(Item item)
  {
    TemporaryData.GetInstance ().playerData.inventory.RemoveAt (item.ordering);
    TemporaryData.GetInstance ().playerData.gold += item.item.price;
    GenerateInventoryItems (item.item.itemType1);
    goldText.text = TemporaryData.GetInstance ().playerData.gold.ToString ();
  }
}
