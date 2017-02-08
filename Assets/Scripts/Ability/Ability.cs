using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
  public int ID = 001;
  public string abilityName;

  public float atkMultiply = 0f;
  public float healMultiply = 0f;
  public int range;
  public float defIncrease = 0f;
  public float atkIncrease = 0f;
  public float criRateIncrease = 0f;
  public float movementIncrease = 0f;
  public float dodgeIncrease = 0f;
  public float guardRateIncrease = 0f;

  public bool usingAround = false;
  public bool active = false;
  public string rangeType; 
}

