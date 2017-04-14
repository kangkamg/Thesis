using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Ability
{
  public int ID = 0;
  public string abilityName;
  public float power;
  public float powerGrowth;
  public int hitAmount;
  public int hitAmountGrowth;
  public int range;
  public int rangeGrowth;
  public bool usingAround = false;
  public int rangeType;
  public int abilityType;
  public int abilityEff;
  public int abilityElement;
  public int gaugeUse;
  public int coolDown;
  public string describe;
}

[System.Serializable]
public class AbilityStatus
{
  public Ability ability;
  public int level;
  public int exp;

  public float power
  {
    get 
    { 
      float ret = ability.power;
      if (level > 1)
      {
        ret += level * ability.powerGrowth;
      }
      return ret;
    }
    private set{ }
  }

  public int hitAmount
  {
    get 
    { 
      int ret = ability.hitAmount;
      if (level > 1)
      {
        ret += level * ability.hitAmountGrowth;
      }
      return ret;
    }
    private set{ }
  }

  public int range
  {
    get 
    { 
      int ret = ability.range;
      if (level > 1)
      {
        ret += level * ability.rangeGrowth;
      }
      return ret;
    }
    private set{ }
  }
}

public class GetUsedAbility
{
  private static Dictionary<string, int> usedAbility = new Dictionary<string, int>();
  
  public static int GetCoolDown(int characterOrdering, int abilityID)
  {
    int coolDown = -1;
    if (usedAbility.TryGetValue (characterOrdering.ToString () + "," + abilityID.ToString (), out coolDown))
      return coolDown;
    else
      return -99;
  }
  
  public static void ModifyAbility (int characterOrdering, int abilityID, int _coolDown)
  {
    int coolDown = GetCoolDown (characterOrdering, abilityID);
    coolDown += _coolDown;
    
    usedAbility [characterOrdering.ToString () + "," + abilityID.ToString ()] = coolDown;
  }
  
  public static void AddAbility(int characterOrdering, Ability ability)
  { 
    usedAbility [characterOrdering.ToString()+","+ability.ID.ToString()] = 0;
  }
  
  public static void RemoveAbility(int characterOrdering)
  {
      usedAbility.Remove (usedAbility.Where (x => int.Parse(x.Key.Split ("," [0]) [0]) == characterOrdering).First ().Key);
  }
}

