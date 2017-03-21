using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  public string weaponEff;
  public string armorEff;
  public string type;
  public Ability normalAttack;
  public Ability specialAttack;
}

[System.Serializable]
public class CharacterStatus
{
  public CharacterBasicStatus basicStatus;
  public int characterLevel = 1;
  public int experience;
  public AbilityStatus normalAttack = new AbilityStatus ();
  public AbilityStatus specialAttack = new AbilityStatus ();
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
        if (equipItem [i].item.itemType1 == "Items") continue;
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
        if (equipItem [i].item.itemType1 == "Items") continue;
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
        if (equipItem [i].item.itemType1 == "Items") continue;
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
        if (equipItem [i].item.itemType1 == "Items") continue;
        ret += equipItem[i].item.increaseCriRate;
      }

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
        if (equipItem [i].item.itemType1 == "Items") continue;
        ret += equipItem[i].item.increaseMovementPoint;
      }

      return ret;
    }
    private set{ }
  }
}

public class CharacterData
{
  public int id;

  public CharacterStatus status;

  private Vector2 originPos;
  private Vector2 offset;
  private Transform originParent;

  public CanvasGroup canvasGroup;
}

