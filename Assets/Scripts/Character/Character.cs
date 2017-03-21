using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Character : MonoBehaviour
{
  public string characterName;

  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;

  public int currentHP;
  public int ordering;

  public bool played = false;

  public bool isAI;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();

  public virtual void Awake()
  {
    characterStatus.basicStatus = GetDataFromSql.GetCharacter (characterName);
    currentHP = characterStatus.maxHp;
  }
   
  public virtual void Update()
  {
    if (currentHP <= 0)
    {
      Destroy (gameObject);
    }
  }

  public virtual void TurnUpdate()
  {
   
  }
}
