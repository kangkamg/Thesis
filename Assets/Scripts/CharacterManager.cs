using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour 
{
  public string _name;
  private int ID;
  public int level;
  public int speed;
  public int movementslot;
  public int exp;

  public bool played = false;

  public Animator anim;
  public Rigidbody playRid;

  private void Awake()
  {
    anim = GetComponent<Animator> ();
    playRid = GetComponent<Rigidbody> ();

    Character data = CharacterData.GetName (_name);

    ID = data.ID;
    level = data.level;
    speed = data.speed;
    movementslot = data.movementslot;
    exp = data.exp;
  }
}
