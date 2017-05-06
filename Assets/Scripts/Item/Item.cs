using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStatus
{
  public int ID;
  public string name = "";
  public int price;
  public int increaseHP;
  public int increaseAttack;
  public int increaseDefense;
  public float increaseCriRate;
  public int increaseMovementPoint;
  public string itemType1;
  public string itemType2;
  public List<string> sellMap = new List<string>();
  public bool stackable;
}

[System.Serializable]
public class Item
{
  public ItemStatus item;
  public bool equiped;
  public int ordering;
}