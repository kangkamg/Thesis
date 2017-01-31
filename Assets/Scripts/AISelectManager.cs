using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AISelectManager : MonoBehaviour 
{
  private int movement;
  private bool walking;
  private bool selectSlot;
  private bool turning;
  private float speed = 0.05f;

  private float Xori,Zori;
  private float x, z;

  private List<GameObject> allEnemy = new List<GameObject>();

  private GameObject selectedCharacter;

  private void Awake()
  {
    GameObject[] enemy = GameObject.FindGameObjectsWithTag ("Enemy");

    foreach (GameObject a in enemy) 
    {
      allEnemy.Add (a);
    }
  }
  private void Update()
  {
    if (!GamePlayManager.GetInstance().isPlayerTurn) 
    {
      if (!selectSlot)
      {
        selectedCharacter = allEnemy [Random.Range (0, allEnemy.Count)].transform.gameObject;
        movement = selectedCharacter.GetComponent<CharacterManager> ().movementslot;
        ShowMoveSlot ();
        Xori = selectedCharacter.transform.position.x;
        Zori = selectedCharacter.transform.position.z;
      } 
      else
      {
        if (!walking) 
        {
          GameObject[] moveslot = GameObject.FindGameObjectsWithTag ("AIMoveSlot");

          int n = Random.Range (0, moveslot.Length);

          x = moveslot [n].transform.position.x;
          z = moveslot [n].transform.position.z;

          walking = true;
          selectSlot = false;
        }
      }
    }
  }

  private void FixedUpdate()
  {
    if (walking) 
    {
      selectedCharacter.GetComponent<CharacterManager>().anim.Play ("Walking@loop");
      if (z == Zori || x == Xori) 
      {
        MoveStraight ();
      } 
      else 
      {
        MoveOblique ();
      }
    }
    else 
    {
      if (selectedCharacter != null) 
      {
        selectedCharacter.GetComponent<CharacterManager> ().anim.Play ("Standing@loop");
      }
    }
  }

  private void ShowMoveSlot()
  {
    MoveSlot.GetInstance ().CreateMoveSlot (movement, selectedCharacter.transform.position.x, selectedCharacter.transform.position.z,true);
    selectSlot = true;
  }

  private void MoveStraight()
  {
    if (selectedCharacter.transform.position.x != x)
    {
      selectedCharacter.transform.position = Vector3.MoveTowards (new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), 
        new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), speed);
      if (!turning) 
      {
        if (x > Xori) Turning (selectedCharacter.transform.right);
        else Turning (selectedCharacter.transform.right*-1);
      }
    }

    if (x > Xori && selectedCharacter.transform.position.x > x - 0.1f)
    {
      selectedCharacter.transform.position = new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
      selectedCharacter.GetComponent<CharacterManager> ().played = true;
    } 
    else if (x < Xori && selectedCharacter.transform.position.x < x + 0.1f) 
    {
      selectedCharacter.transform.position = new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
      selectedCharacter.GetComponent<CharacterManager> ().played = true;
    }

    if (selectedCharacter.transform.position.z != z) 
    {
      selectedCharacter.transform.position = Vector3.MoveTowards (new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), 
        new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z), speed);
      if (!turning) 
      {
        if (z > Zori) Turning (selectedCharacter.transform.forward);
        else Turning (selectedCharacter.transform.forward*-1);
      }
    }

    if (z > Zori && selectedCharacter.transform.position.z > z - 0.1f)
    {
      selectedCharacter.transform.position = new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
      selectedCharacter.GetComponent<CharacterManager> ().played = true;
    } 
    else if (z < Zori && selectedCharacter.transform.position.z < z + 0.1f) 
    {
      selectedCharacter.transform.position = new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
      selectedCharacter.GetComponent<CharacterManager> ().played = true;
    }
  }

  private void MoveOblique()
  {
    if (selectedCharacter.transform.position.x != x)
    {
      selectedCharacter.transform.position = Vector3.MoveTowards (new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), 
        new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), speed);
      if (!turning) 
      {
        if (x > Xori) Turning (transform.right);
        else Turning (transform.right*-1);
      }
      if (x > Xori && selectedCharacter.transform.position.x > x - 0.1f)
      {
        selectedCharacter.transform.position = new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
        turning = false;
      } 
      else if (x < Xori && selectedCharacter.transform.position.x < x + 0.1f) 
      {
        selectedCharacter.transform.position = new Vector3 (x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
        turning = false;
      }
    }
    else
    {
      if (selectedCharacter.transform.position.z !=z)
      {
        selectedCharacter.transform.position = Vector3.MoveTowards (new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, selectedCharacter.transform.position.z), 
          new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z), speed);
        if (!turning) 
        {
          if (z > Zori) Turning (Vector3.forward);
          else Turning (Vector3.forward*-1);
        }
      }

      if (z > Zori && selectedCharacter.transform.position.z > z - 0.1f)
      {
        selectedCharacter.transform.position = new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z);
        walking = false;
        Turning (Vector3.forward);
        turning = false;
        MoveSlot.GetInstance ().ClearMoveSlot ();
        selectedCharacter.GetComponent<CharacterManager> ().played = true;
      } 
      else if (z < Zori && selectedCharacter.transform.position.z < z + 0.1f) 
      {
        selectedCharacter.transform.position = new Vector3 (selectedCharacter.transform.position.x, selectedCharacter.transform.position.y, z);
        walking = false;
        Turning (Vector3.forward);
        turning = false;
        MoveSlot.GetInstance ().ClearMoveSlot ();
        selectedCharacter.GetComponent<CharacterManager> ().played = true;
      }
    }  
  }

  private void Turning(Vector3 lookAt)
  {
    Quaternion newRot = Quaternion.LookRotation (lookAt);
    selectedCharacter.GetComponent<CharacterManager>().playRid.MoveRotation (newRot);
    turning = true;
  }
}
