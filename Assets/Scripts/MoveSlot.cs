using UnityEngine;
using System.Collections;

public class MoveSlot : MonoBehaviour
{
  private static MoveSlot moveSlot;

  private GameObject slot;

  private void Awake()
  {
    moveSlot = GetComponent<MoveSlot> ();
    slot = (GameObject)Resources.Load ("ETC_/MoveSlot");
  }
  public static MoveSlot GetInstance()
  {
    return MoveSlot.moveSlot;
  }

  public void CreateMoveSlot(int movement, float x, float z)
  {
    ClearMoveSlot ();
      
    GameObject masterSlot = new GameObject ("Empty");

    for (int i = -movement; i <= movement; i++) 
    {
      for (int j = -movement; j <= movement; j++) 
      {
          if (i + j <= movement && j * 3 + z >= 0) 
          {
            GameObject MovementSlot = Instantiate (slot, new Vector3 (((float)i * 3) + x, 1.01f, ((float)j * 3) + z), Quaternion.identity)as GameObject;
            MovementSlot.transform.SetParent (masterSlot.transform);
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
