using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
  public string name;
  public int price;
  public int increaseHP;
  public int increaseAttack;
  public int increaseDefense;
  public float increaseCriRate;
  public float increaseGuardRate;
  public int increaseMovementPoint;
  public List<Ability> itemAb = new List<Ability>();
  public bool IsRuneStone = false;
  public string itemType;
  public List<string> sellMap = new List<string>();
}
