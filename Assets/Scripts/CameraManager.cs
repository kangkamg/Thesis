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

  private Vector3 _originPos;
  private float _orthographicSize;

  public bool isFocus;

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
      _orthographicSize = _target.x * 2.5f;
      this.GetComponent<Camera> ().orthographicSize = _orthographicSize;

      _target.y = transform.position.y;
      _target.z = transform.position.z;

      transform.position = _target;
      _originPos = transform.position;
    }

  }

  public void FocusCamera(Vector3 _selectedCharacter, Vector3 _target)
  {
    this.GetComponent<Camera> ().orthographicSize = Mathf.RoundToInt(Vector3.Distance (_selectedCharacter, _target));

    transform.position = (_selectedCharacter+_target)/2;
  }

  public void ResetCamera()
  {
    transform.position = _originPos;
    this.GetComponent<Camera> ().orthographicSize = _orthographicSize;
  }
}
