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
    canBuy = GetDataFromSql.GetShopItem (TemporaryData.GetInstance().playerData.passedMap);
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
        else sprite = Resources.Load<Sprite>("Item/Texture/BookOf" + canBuy [i].itemType1);
         
        items.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
        items.transform.localScale = Vector3.one;
        items.GetComponent<Button> ().onClick.AddListener (() => BuyingItem (items.GetComponent<ItemData>().items.item, items));
        if (TemporaryData.GetInstance ().playerData.gold < canBuy [i].price) items.GetComponent<Button> ().interactable = false;
        allItem.Add (items);
      }
    }

    if (allItem.Count > 7) 
    {
      showing.GetComponent<RectTransform> ().sizeDelta = new Vector2 (showing.GetComponent<RectTransform> ().sizeDelta.x, 245f * (allItem.Count));
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      if (allItem.Count < 1)
      {
        showing.transform.parent.parent.GetChild (1).GetComponent<Text> ().text = "Empty Inventory";
        showing.transform.parent.parent.GetChild (1).gameObject.SetActive (true);
      }
      else showing.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
      showing.GetComponentInParent<ScrollRect> ().vertical = false;
    }
    showing.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -showing.GetComponent<RectTransform> ().rect.height/ 2);
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
      items.transform.GetChild (2).GetComponent<Text> ().text = (items.GetComponent<ItemData> ().items.item.price/2).ToString();
      Sprite sprite = new Sprite();
      if (Resources.Load<Sprite> ("Item/Texture/" + inventoryItem [i].item.name) != null) sprite = Resources.Load<Sprite> ("Item/Texture/" + inventoryItem [i].item.name);
      else sprite = Resources.Load<Sprite>("Item/Texture/BookOf" + inventoryItem [i].item.itemType1);

      items.transform.GetChild (0).GetComponent<Image>().sprite = sprite;
      items.transform.localScale = Vector3.one;
      items.GetComponent<Button> ().onClick.AddListener (() => SellingItem (items.GetComponent<ItemData>().items, items));
      allItem.Add (items);
    }

    if (allItem.Count > 7)
    {
      showing.GetComponent<RectTransform> ().sizeDelta = new Vector2 (showing.GetComponent<RectTransform> ().sizeDelta.x , 245f * (allItem.Count));
      showing.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      if (allItem.Count < 1)
      {
        showing.transform.parent.parent.GetChild (1).GetComponent<Text> ().text = "Empty Inventory";
        showing.transform.parent.parent.GetChild (1).gameObject.SetActive (true);
      }
      else showing.transform.parent.parent.GetChild (1).gameObject.SetActive (false);
      showing.GetComponentInParent<ScrollRect> ().vertical = false;
    }
    showing.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -showing.GetComponent<RectTransform> ().rect.height/ 2);
  }
  
  private void BuyingItem(ItemStatus data, GameObject items)
  {
    foreach (GameObject a in allItem)
    {
      if (a != items)
      {
        Destroy (items);
      } 
      else 
      {
        a.transform.SetAsFirstSibling ();
      }
    }
    
    
    GameObject dialogBox = GameObject.Instantiate(DialogBoxManager.GetInstance ().GenerateDialogBox ("Are you sure to buy this item ?", true));
    
    dialogBox.transform.SetParent (showing.transform.parent.parent);
    dialogBox.transform.localScale = Vector3.one;
    dialogBox.transform.localPosition = Vector2.zero;
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => DialogBoxManager.GetInstance().AddChangeScene ("MainMenuScene"));
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => Buying(data));
    dialogBox.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => NotBuying (dialogBox,data));
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = false;
    }
    items.GetComponent<Button>().interactable = false;
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (false);
  }
  
  private void Buying(ItemStatus data)
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
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = true;
    }
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (true);
  }
  
  private void NotBuying(GameObject dialogBox, ItemStatus data)
  {
    Destroy (dialogBox);
    GenerateBuyingItems (data.itemType1);
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = true;
    }
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (true);
  }
  
  private void SellingItem(Item item, GameObject items)
  {
    foreach (GameObject a in allItem)
    {
      if (a != items)
      {
        Destroy (items);
      } 
      else 
      {
        a.transform.SetAsFirstSibling ();
      }
    }


    GameObject dialogBox = GameObject.Instantiate(DialogBoxManager.GetInstance ().GenerateDialogBox ("Are you sure to sell this item ?", true));

    dialogBox.transform.SetParent (showing.transform.parent.parent);
    dialogBox.transform.localScale = Vector3.one;
    dialogBox.transform.localPosition = Vector2.zero;
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => DialogBoxManager.GetInstance().AddChangeScene ("MainMenuScene"));
    dialogBox.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => Selling(item));
    dialogBox.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => NotSelling (dialogBox,item));
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = false;
    }
    items.GetComponent<Button>().interactable = false;
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (false);
    
  }
  
  private void Selling(Item data)
  {
    TemporaryData.GetInstance ().playerData.inventory.RemoveAt (data.ordering);
    TemporaryData.GetInstance ().playerData.gold += data.item.price/2;
    GenerateInventoryItems (data.item.itemType1);
    goldText.text = TemporaryData.GetInstance ().playerData.gold.ToString ();
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = true;
    }
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (true);
  }

  private void NotSelling(GameObject dialogBox, Item data)
  {
    Destroy (dialogBox);
    GenerateInventoryItems (data.item.itemType1);
    
    foreach (Transform child in GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(2))
    {
      child.GetComponent<Toggle> ().interactable = true;
    }
    GameObject.Find("ShopCanvas").transform.GetChild(1).GetChild(3).gameObject.SetActive (true);
  }
}
