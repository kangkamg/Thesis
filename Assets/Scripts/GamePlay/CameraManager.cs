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
  
  Transform follower;

  public float smoothing;
  public float orthoZoomSpeed = 0.01f;

  private Vector3 _originPos;
  private float _orthographicSize;
  
  private bool following = false;
  private bool isLookWholeMap = false;
  public bool isFocus = false;
  
  Vector2 originTouch;

  private void Awake()
  {
    instance = GetComponent<CameraManager> ();
  }

  private void LateUpdate () 
  {
    if (following && !isFocus && !isLookWholeMap)
    {
      Vector3 targetCamPos = follower.position;

      targetCamPos.y = 0;

      transform.position = Vector3.SmoothDamp (transform.position, targetCamPos, ref velocity, smoothing * Time.deltaTime);
    }
    
    if (!isFocus && !isLookWholeMap) 
    {
      if (Input.touchCount == 2)
      {
        PinchToZoom (Input.GetTouch (0), Input.GetTouch (1));
      }
      else if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) 
      {
        originTouch = Input.mousePosition;
      }
      
      if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) 
      {
        MoveCameraWithTouch ();
      }
    }
	}
    
  public void SetUpStartCamera(Vector3 _target)
  {
    _orthographicSize = _target.x * 2.5f;
    this.GetComponent<Camera> ().orthographicSize = _orthographicSize;

    _target.y = transform.position.y;
    _target.z = transform.position.z;

    transform.position = _target;
    _originPos = transform.position;
    following = false;
  }
  
  public void MoveCameraToTarget(Transform followTarget)
  {
    follower = followTarget;
    if (!isLookWholeMap)
    {
      this.GetComponent<Camera> ().orthographicSize = followTarget.localScale.x * 10f;
    
      following = true;
    }
  }

  public void FocusCamera(Vector3 _selectedCharacter, Vector3 _target)
  {
    isFocus = true;
    
    this.GetComponent<Camera> ().orthographicSize = Mathf.RoundToInt(Vector3.Distance (_selectedCharacter, _target));

    transform.position = (_selectedCharacter+_target)/2;
  }

  public void ResetCamera()
  {
    if (!isLookWholeMap) 
    {
      transform.position = follower.position;
      this.GetComponent<Camera> ().orthographicSize = follower.localScale.x * 10f;
    }
    else 
    {
      transform.position = _originPos;
      this.GetComponent<Camera> ().orthographicSize = _orthographicSize;
    }
    isFocus = false;
  }
  
  public void LookWholeMap()
  {
    if (following) 
    {
      this.GetComponent<Camera> ().orthographicSize = _orthographicSize;
      transform.position = _originPos;
      following = false;
      isLookWholeMap = true;
    }
    else 
    {
      isLookWholeMap = false;
      MoveCameraToTarget (follower);
    }
  }
  
  public void PinchToZoom(Touch touchZero, Touch touchOne)
  {
    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    
    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
    
    float dealtaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
    
    if (this.GetComponent<Camera> ().orthographicSize < 10 || this.GetComponent<Camera> ().orthographicSize > 1)
    {
      this.GetComponent<Camera> ().orthographicSize += dealtaMagnitudeDiff * orthoZoomSpeed;
    
      this.GetComponent<Camera> ().orthographicSize = Mathf.Max (this.GetComponent<Camera> ().orthographicSize, 0.1f);
    }
  }
  
  public void MoveCameraWithTouch()
  {
    Vector2 newTouchposition = Input.GetTouch(0).position;
    
    transform.position = Vector3.SmoothDamp (transform.position,-1*(Vector3)(originTouch - newTouchposition)*this.GetComponent<Camera> ().orthographicSize/this.GetComponent<Camera> ().pixelHeight*2f, ref velocity, smoothing * Time.deltaTime);
    
    originTouch = newTouchposition;
  }
}
