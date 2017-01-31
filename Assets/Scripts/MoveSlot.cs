using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveSlot : MonoBehaviour
{
  private static MoveSlot moveSlot;

  private GameObject playerSlot, enemySlot;

  private void Awake()
  {
    moveSlot = GetComponent<MoveSlot> ();
    playerSlot = (GameObject)Resources.Load ("ETC_/MoveSlot");
    enemySlot = (GameObject)Resources.Load ("ETC_/AIMoveSlot");
  }
  public static MoveSlot GetInstance()
  {
    return MoveSlot.moveSlot;
  }

  public void CreateMoveSlot(int movement, float x, float z, bool enemy = false)
  {
    ClearMoveSlot ();

    GameObject slot;
    if (enemy) slot = enemySlot;
    else slot = playerSlot;
    
    GameObject masterSlot = new GameObject ("Empty");

    for (int i = -movement; i <= movement; i++) 
    {
      for (int j = -movement; j <= movement; j++) 
      {
        if (i < 0)
        {
          if (j > 0)
          {
            if ((-1 * i) + j <= movement && j * 3 + z >= 0) 
            {
              GameObject MovementSlot = Instantiate (slot, new Vector3 (((float)i * 3) + x, 1.01f, ((float)j * 3) + z), Quaternion.identity)as GameObject;
              MovementSlot.transform.SetParent (masterSlot.transform);
            }
          }
          else
          {
            if ((-1*i) + (-1*j) <= movement && j * 3 + z >= 0) 
            {
              GameObject MovementSlot = Instantiate (slot, new Vector3 (((float)i * 3) + x, 1.01f, ((float)j * 3) + z), Quaternion.identity)as GameObject;
              MovementSlot.transform.SetParent (masterSlot.transform);
            }
          }
        }
        else
        {
          if (j > 0)
          {
            if (i + j <= movement && j * 3 + z >= 0) 
            {
              GameObject MovementSlot = Instantiate (slot, new Vector3 (((float)i * 3) + x, 1.01f, ((float)j * 3) + z), Quaternion.identity)as GameObject;
              MovementSlot.transform.SetParent (masterSlot.transform);
            }
          }
          else
          {
            if (i + (-1*j) <= movement && j * 3 + z >= 0) 
            {
              GameObject MovementSlot = Instantiate (slot, new Vector3 (((float)i * 3) + x, 1.01f, ((float)j * 3) + z), Quaternion.identity)as GameObject;
              MovementSlot.transform.SetParent (masterSlot.transform);
            }
          }
        }
      }
    }
  }

  public void ClearMoveSlot()
  {
    if (GameObject.Find ("Empty") != null)
    {
      Destroy(GameObject.Find("Empty"));
    }
  }
}
