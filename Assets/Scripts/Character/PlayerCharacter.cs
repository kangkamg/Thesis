﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character 
{
  public override void Awake()
  {
    characterStatus.movementPoint = 5;

    base.Awake ();
  }
	
  private void Update()
  {
    if (positionQueue.Count > 0)
    {
      if (Vector3.Distance (positionQueue [0], transform.position) > 0.1f)
      {
        transform.position = Vector3.MoveTowards (transform.position, positionQueue [0], moveSpeed*Time.deltaTime);
        if (Vector3.Distance (positionQueue [0], transform.position) < 0.1f) 
        {
          transform.position = positionQueue [0];
          positionQueue.RemoveAt (0);
          if (positionQueue.Count == 0)
          {
            
          }
        }
      }
    }
  }

  public void UpdateUI()
  {
    
  }
}
