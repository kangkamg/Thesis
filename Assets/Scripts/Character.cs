using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
  public List<Vector3>positionQueue = new List<Vector3>();

  public int movementPoint = 5;

  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;
}
