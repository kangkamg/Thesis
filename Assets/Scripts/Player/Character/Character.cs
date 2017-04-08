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

  public GameObject info;
  
  public int currentHP;
  public int ordering;

  public bool played = false;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();

  public virtual void Update()
  {
    info.transform.GetChild (1).GetComponent<TextMesh> ().text = currentHP.ToString();
  }

  public virtual void TurnUpdate()
  {
   
  }
  
  public virtual void SetStatus(int ID)
  {
    characterStatus.basicStatus = GetDataFromSql.GetCharacter (ID);

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
