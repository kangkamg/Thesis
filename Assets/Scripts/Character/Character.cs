using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStatus
{
  public int ID;
  public string characterName;
  public int level,experience;
  public int maxHP = 25;
  public int attack;
  public int defense;
  public int movementPoint = 5;
  public float criRate = 0.10f;
  public float dogdeRate = 0.10f;
  public float guardRate = 0.10f;
  public string job;
  public List<Ability> characterAbility = new List<Ability> ();
}

public class Character : MonoBehaviour
{
  public string characterName;

  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;

  public int currentHP;

  public bool floating = false;
  public bool stunning = false;
  public bool poisoning = false;
  public bool blinding = false;

  public bool moving = false;
  public bool attacking = false;

  public CharacterStatus characterStatus = new CharacterStatus();
  public List<Ability> setupAbility = new List<Ability>();

  public Slider hpSlider;

  public virtual void Awake()
  {
    characterStatus = GetDataFromSql.GetCharacter (characterName);
    setupAbility = characterStatus.characterAbility;

    currentHP = characterStatus.maxHP;

    hpSlider.maxValue = characterStatus.maxHP;
    hpSlider.value = currentHP;
  }
    
  public virtual void TurnUpdate()
  {
    
  }
}
