using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour 

{
  private static CameraManager instance;
  public static CameraManager GetInstance()
  {
    return instance;
  }
    
  Vector3 offset;
  private Vector3 velocity = Vector3.zero;

  Vector3 target = new Vector3(-9999,-9999,-9999);

  public float smoothing;

  private void Awake()
  {
    instance = GetComponent<CameraManager> ();
  }

  private void LateUpdate () 
  {
    if (target.x != -9999)
    {
      Vector3 targetCamPos = target;

      targetCamPos.y = transform.position.y;

      transform.position = Vector3.SmoothDamp (transform.position, targetCamPos, ref velocity, smoothing * Time.deltaTime);

      if (transform.position.x == targetCamPos.x && transform.position.z == targetCamPos.z)
      {
        target = new Vector3 (-9999, -9999, -9999);
      }
    }
	}
    
  public void MoveCameraToTarget(Vector3 _target,bool _start = false)
  {
    if (!_start) 
    {
      target = _target;
    }
    else 
    {
      this.GetComponent<Camera> ().orthographicSize = (_target.x * 4);

      _target.y = transform.position.y;

      transform.position = _target;
    }

  }
}
