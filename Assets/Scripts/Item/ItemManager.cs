using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour 
{
  private static ItemManager instance;
  public static ItemManager GetInstance()
  {
    return instance;
  }

  public GameObject showing;

  private List<ItemStatus> canBuy = new List<ItemStatus>();
  public List<GameObject> allItem = new List<GameObject> ();

  public void Awake()
  {
    instance = this;
    canBuy = GetDataFromSql.GetItemFromMap (PlayerPrefs.GetInt (Const.TownSceneNo, 1).ToString ());
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
        allItem.Add (items);
      }
    }

    if (allItem.Count > 6)
    {
      showing.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 80 * (allItem.Count - 1));
      showing.transform.localPosition = showing.GetComponent<RectTransform> ().sizeDelta / -(allItem.Count - 2);
      showing.GetComponentInParent<ScrollRect> ().enabled = true;
    } 
    else
    {
      showing.GetComponentInParent<ScrollRect> ().enabled = false;
    }
    for (int i = 0; i < allItem.Count; i++)
    {
      allItem[i].transform.localPosition = new Vector3 (1, (showing.GetComponent<RectTransform>().sizeDelta.y/2) - (80*i), 0);
    }
  }
}
