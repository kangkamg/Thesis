using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStatus
{
  public int level,experience;
  public int maxHP = 25;
  public int currentHP;
  public int attack;
  public int defense;
  public int attackRange;
  public int movementPoint = 5;
  public float criRate = 0.10f;
  public float dogdeRate = 0.10f;
  public float guardRate = 0.10f;
}
public class Character : MonoBehaviour
{
  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;
 
  public bool floating = false;
  public bool stunning = false;
  public bool poisoning = false;
  public bool blinding = false;

  public bool moving = false;
  public bool attacking = false;

  public CharacterStatus characterStatus = new CharacterStatus();
  public List<Ability> setupAbility = new List<Ability> ();

  public Texture _MaxHp;
  public Texture _CurrentHP;

  public Slider hpSlider;

  public virtual void Awake()
  {
    characterStatus.currentHP = characterStatus.maxHP;

    hpSlider.maxValue = characterStatus.maxHP;
    hpSlider.value = characterStatus.currentHP;
  }

  public virtual void Update()
  {
    
  }

  public virtual void TurnUpdate()
  {
    
  }
}
