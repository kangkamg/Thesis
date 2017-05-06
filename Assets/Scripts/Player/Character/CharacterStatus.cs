﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectiveAttack
{
  Slash = 1,
  Pierce,
  Strike,
  Magic
}

public enum Element
{
  Earth = 1,
  Water,
  Fire,
  Wind,
  Lightning
}


[System.Serializable]
public class CharacterBasicStatus
{
  public int ID;
  public string characterName;
  public int maxHP;
  public int maxHpGrowth;
  public int attack;
  public int attackGrowth;
  public int defense;
  public int defenseGrowth;
  public float criRate;
  public float criRateGrowth;
  public int movementPoint;
  public List<string> learnAbleAbility = new List<string>();
}

[System.Serializable]
public class CharacterStatus
{
  public CharacterBasicStatus basicStatus;
  public int characterLevel = 1;
  public int experience = 0;
  public int nextLevelExp 
  {
    get 
    { 
      return (Mathf.RoundToInt (Mathf.Pow (characterLevel+1, 4) - Mathf.Pow(characterLevel,4)));
    }
    private set
    { 
      
    }
  }
  public List<AbilityStatus> learnedAbility = new List<AbilityStatus> ();
  public List<AbilityStatus> equipedAbility = new List<AbilityStatus> ();
  public List<Item> equipItem = new List<Item>();
  public bool isInParty;
  public int partyOrdering;


  public int basicMaxHp
  {
    get 
    { 
      int ret = basicStatus.maxHP;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.maxHpGrowth;
      }
      return ret;
    }
    private set{ }
  }
  public int basicAttack
  {
    get 
    { 
      int ret = basicStatus.attack;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.attackGrowth;
      }
      return ret;
    }
    private set{ }
  }
  public int basicDefense
  {
    get 
    { 
      int ret = basicStatus.defense;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.defenseGrowth;
      }
      return ret;
    }
    private set{ }
  }
  public float basicCriRate
  {
    get 
    { 
      float ret = basicStatus.criRate;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.criRateGrowth;
      }
      return ret;
    }
    private set{ }
  }
  public int basicMovementPoint
  {
    get 
    { 
      int ret = basicStatus.movementPoint;
      return ret;
    }
    private set{ }
  }

  public int maxHp
  {
    get 
    { 
      int ret = basicStatus.maxHP;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.maxHpGrowth;
      }
      for (int i = 0; i < equipItem.Count; i++)
      {
        ret += equipItem[i].item.increaseHP;
      }

      return ret;
    }
    private set{ }
  }

  public int attack
  {
    get 
    { 
      int ret = basicStatus.attack;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.attackGrowth;
      }
      for (int i = 0; i < equipItem.Count; i++)
      {
        ret += equipItem[i].item.increaseAttack;
      }

      return ret;
    }
    private set{ }
  }

  public int defense
  {
    get 
    { 
      int ret = basicStatus.defense;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.defenseGrowth;
      }
      for (int i = 0; i < equipItem.Count; i++)
      {
        ret += equipItem[i].item.increaseDefense;
      }

      return ret;
    }
    private set{ }
  }

  public float criRate
  {
    get 
    { 
      float ret = basicStatus.criRate;
      if (characterLevel > 1)
      {
        ret += characterLevel * basicStatus.criRateGrowth;
      }
      for (int i = 0; i < equipItem.Count; i++)
      {
        ret += equipItem[i].item.increaseCriRate;
      }

      if (ret > 80) ret = 80;
      return ret;
    }
    private set{ }
  }

  public int movementPoint
  {
    get 
    { 
      int ret = basicStatus.movementPoint;

      for (int i = 0; i < equipItem.Count; i++)
      {
        ret += equipItem[i].item.increaseMovementPoint;
      }

      return ret;
    }
    private set{ }
  }
}
