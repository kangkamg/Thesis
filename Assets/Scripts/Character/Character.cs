using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Character : MonoBehaviour
{
  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;

  public int currentHP;
  public int ordering;

  public bool played = false;

  public bool isAI;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();

  public virtual void Update()
  {

  }

  public virtual void TurnUpdate()
  {
   
  }

  public virtual void SetStatus(string name)
  {
    characterStatus.basicStatus = GetDataFromSql.GetCharacter (name);

    currentHP = characterStatus.maxHp;
    this.name = characterStatus.basicStatus.characterName;
  }

  public virtual void SetStatus(CharacterStatus status)
  {
    characterStatus = status;

    currentHP = characterStatus.maxHp;
    this.name = characterStatus.basicStatus.characterName;
  }
}
