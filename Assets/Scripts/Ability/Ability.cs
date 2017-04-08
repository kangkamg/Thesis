using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  public int ordering = 0;

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

