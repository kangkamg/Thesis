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
  float previousmultiply;
  
  private bool following = false;
  private bool isLookWholeMap = false;
  public bool isFocus = false;
  public bool isMoving = false;
  
  Vector2 originTouch;

  private void Awake()
  {
    instance = GetComponent<CameraManager> ();
  }

  private void LateUpdate () 
  {
    if (following)
    {
      Vector3 targetCamPos = follower.position ;

      if (follower.position.x == 0)
      {
        targetCamPos.x += 6;
        targetCamPos.y = 7;
        targetCamPos.z -= 6;
      }
      else if (follower.position.x > 3)
      {
        targetCamPos.x += Mathf.Abs (follower.position.x);
        targetCamPos.y = Mathf.Abs (follower.position.x);
        targetCamPos.z -= Mathf.Abs (follower.position.x);
      }
      else
      {
        targetCamPos.x += Mathf.Abs (follower.position.x*2);
        targetCamPos.y = Mathf.Abs (follower.position.x*2);
        targetCamPos.z -= Mathf.Abs (follower.position.x*2);
      }

      transform.position = Vector3.SmoothDamp (transform.position, targetCamPos, ref velocity, smoothing * Time.deltaTime);

      if (Vector3.Distance (targetCamPos, transform.position) < 0.1f)
      {
        transform.position = targetCamPos;
        following = false;
      }
    }
    
    if (!isFocus && !isLookWholeMap && !following && !GameManager.GetInstance().hitButton) 
    {      
      if (GameManager.GetInstance ().isTouch) 
      {
        if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began)
        {
          originTouch = Input.GetTouch (0).position;
        }
        
        if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved)
        {
          MoveCameraWithTouch ();
        }
        
        if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Ended) 
        {
          isMoving = false;
        }
      }
      
      if (Input.touchCount == 2)
      {
        PinchToZoom (Input.GetTouch (0), Input.GetTouch (1));
      }
    }
	}
    
  public void SetUpStartCamera(Vector3 _target)
  {
    following = false;
    isLookWholeMap = false;
    _orthographicSize = _target.x * 4f;
    this.GetComponent<Camera> ().orthographicSize = _orthographicSize;

    _target.y = transform.position.y;
    _target.z = transform.position.z;

    transform.position = _target;
    _originPos = transform.position;
  }
  
  public void MoveCameraToTarget(Transform followTarget, float mutiply = 7.5f)
  {
    follower = followTarget;
    previousmultiply = mutiply;
    if (!isLookWholeMap)
    {
      this.GetComponent<Camera> ().orthographicSize = follower.localScale.x * mutiply;
    
      following = true;
    }
  }

  public void FocusCamera(Vector3 _selectedCharacter, Vector3 _target)
  {
    isFocus = true;
    following = false;
    
    this.GetComponent<Camera> ().orthographicSize = Mathf.RoundToInt(Vector3.Distance (_selectedCharacter, _target));

    transform.position = (_selectedCharacter+_target)/2;
  }

  public void ResetCamera()
  {
    if (!isLookWholeMap) 
    {
      Vector3 targetCamPos = follower.position ;

      if (follower.position.x == 0)
      {
        targetCamPos.x += 6;
        targetCamPos.y = 7;
        targetCamPos.z -= 6;
      }
      else if (follower.position.x > 3)
      {
        targetCamPos.x += Mathf.Abs (follower.position.x);
        targetCamPos.y = Mathf.Abs (follower.position.x);
        targetCamPos.z -= Mathf.Abs (follower.position.x);
      }
      else
      {
        targetCamPos.x += Mathf.Abs (follower.position.x*2);
        targetCamPos.y = Mathf.Abs (follower.position.x*2);
        targetCamPos.z -= Mathf.Abs (follower.position.x*2);
      }
      transform.position = targetCamPos;
      
      if(follower.GetType() == typeof(Character))
        this.GetComponent<Camera> ().orthographicSize = follower.localScale.x * 7.5f;
      else
        this.GetComponent<Camera> ().orthographicSize = follower.localScale.x * 5f;
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
    if (!isLookWholeMap) 
    {
      this.GetComponent<Camera> ().orthographicSize = _orthographicSize;
      isLookWholeMap = true;
    }
    else 
    {
      isLookWholeMap = false;
      if (follower != null)
      {
        MoveCameraToTarget (follower, previousmultiply);
      }
    }
  }
  
  public void PinchToZoom(Touch touchZero, Touch touchOne)
  {
    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    
    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
    
    float dealtaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
    
    if (this.GetComponent<Camera> ().orthographicSize > 4 && this.GetComponent<Camera> ().orthographicSize < 20)
    {
      this.GetComponent<Camera> ().orthographicSize += dealtaMagnitudeDiff * orthoZoomSpeed;
    
      this.GetComponent<Camera> ().orthographicSize = Mathf.Max (this.GetComponent<Camera> ().orthographicSize, 0.1f);
    }
  }
  
  public void MoveCameraWithTouch()
  {
    isMoving = true;
    following = false;
    
    Vector2 newTouchposition = Input.GetTouch(0).position;
    
    transform.position += transform.TransformDirection ((Vector3)(originTouch - newTouchposition)*this.GetComponent<Camera> ().orthographicSize/this.GetComponent<Camera> ().pixelHeight*2f);
    
    originTouch = newTouchposition;
  }
}
