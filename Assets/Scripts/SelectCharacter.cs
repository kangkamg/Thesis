using UnityEngine;
using System.Collections;

public class SelectCharacter : MonoBehaviour 
{
  public int movement;
  private bool walking;
  private bool selectSlot;
  private bool turning;

  private float Xori,Zori;
  private float x, z;

  private Animator anim;
  private Rigidbody playRid;

  private void Awake()
  {
    anim = GetComponent<Animator> ();
    playRid = GetComponent<Rigidbody> ();
  }
  private void Update()
  {
    if (Input.GetMouseButtonDown(0) && !walking) 
    {
      Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast (camRay, out hit, 100f)) 
      {
        
      if (!selectSlot) 
      {
        if (hit.transform.tag == "Player")
        {
            ShowMoveSlot ();
            Xori = transform.position.x;
            Zori = transform.position.z;
        }
      } 
      else 
      {
        if (hit.transform.tag == "MoveSlot")
        {
          x = hit.transform.position.x;
          z = hit.transform.position.z;
          
          walking = true;
          selectSlot = false;
        }
      }
      }
    }
  }

  private void FixedUpdate()
  {
    if (walking) 
    {
      anim.Play ("Walking@loop");
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
      anim.Play ("Standing@loop");
    }
  }

  private void ShowMoveSlot()
  {
    MoveSlot.GetInstance ().CreateMoveSlot (movement, transform.position.x,transform.position.z);
    selectSlot = true;
  }

  private void MoveStraight()
  {
    if (transform.position.x != x)
    {
      transform.position = Vector3.Lerp (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (x, transform.position.y, transform.position.z), 0.025f);
      if (!turning) 
      {
        if (x > Xori) Turning (transform.right);
        else Turning (transform.right*-1);
      }
    }

    if (x > Xori && transform.position.x > x - 0.1f)
    {
      transform.position = new Vector3 (x, transform.position.y, transform.position.z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
    } 
    else if (x < Xori && transform.position.x < x + 0.1f) 
    {
      transform.position = new Vector3 (x, transform.position.y, transform.position.z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
    }

    if (transform.position.z != z) 
    {
      transform.position = Vector3.Lerp (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (transform.position.x, transform.position.y, z), 0.025f);
      if (!turning) 
      {
        if (z > Zori) Turning (transform.forward);
        else Turning (transform.forward*-1);
      }
    }

    if (z > Zori && transform.position.z > z - 0.1f)
    {
      transform.position = new Vector3 (transform.position.x, transform.position.y, z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
    } 
    else if (z < Zori && transform.position.z < z + 0.1f) 
    {
      transform.position = new Vector3 (transform.position.x, transform.position.y, z);
      walking = false;
      Turning (Vector3.forward);
      turning = false;
      MoveSlot.GetInstance ().ClearMoveSlot ();
    }
  }

  private void MoveOblique()
  {
    if (transform.position.x != x)
    {
      transform.position = Vector3.Lerp (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (x, transform.position.y, transform.position.z), 0.025f);
      if (!turning) 
      {
        if (x > Xori) Turning (transform.right);
        else Turning (transform.right*-1);
      }
      if (x > Xori && transform.position.x > x - 0.1f)
      {
        transform.position = new Vector3 (x, transform.position.y, transform.position.z);
        turning = false;
      } 
      else if (x < Xori && transform.position.x < x + 0.1f) 
      {
        transform.position = new Vector3 (x, transform.position.y, transform.position.z);
        turning = false;
      }
    }
    else
    {
      if (transform.position.z !=z)
      {
        transform.position = Vector3.Lerp (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (transform.position.x, transform.position.y, z), 0.025f);
        if (!turning) 
        {
          if (z > Zori) Turning (Vector3.forward);
          else Turning (Vector3.forward*-1);
        }
      }

      if (z > Zori && transform.position.z > z - 0.1f)
      {
        transform.position = new Vector3 (transform.position.x, transform.position.y, z);
        walking = false;
        Turning (Vector3.forward);
        turning = false;
        MoveSlot.GetInstance ().ClearMoveSlot ();
      } 
      else if (z < Zori && transform.position.z < z + 0.1f) 
      {
        transform.position = new Vector3 (transform.position.x, transform.position.y, z);
        walking = false;
        Turning (Vector3.forward);
        turning = false;
        MoveSlot.GetInstance ().ClearMoveSlot ();
      }
    }  
  }

  private void Turning(Vector3 lookAt)
  {
    Quaternion newRot = Quaternion.LookRotation (lookAt);
    playRid.MoveRotation (newRot);
    turning = true;
  }
}
