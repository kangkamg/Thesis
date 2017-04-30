using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
  
  public bool following = false;
  public bool isLookWholeMap = false;
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
      targetCamPos.y = 2.5f;
      
      int mutilplyTargetCam = 0;
      
      if (follower.GetComponent<Character>() != null) 
      {
        mutilplyTargetCam = GameManager.GetInstance ()._mapSize [0] - (int)follower.GetComponent<Character> ().gridPosition.x;
      } 
      else if (follower.GetComponent<Tile>() != null)
      {
        mutilplyTargetCam = GameManager.GetInstance ()._mapSize [0] - (int)follower.GetComponent<Tile> ().gridPosition.x;
      }
      targetCamPos.x += Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));
      targetCamPos.y += Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));
      targetCamPos.z -= Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));

      transform.position = Vector3.SmoothDamp (transform.position, targetCamPos, ref velocity, smoothing * Time.deltaTime);

      if (Vector3.Distance (targetCamPos, transform.position) < 0.1f)
      {
        transform.position = targetCamPos;
        following = false;
      }
    }
    
    if (!isFocus && !isLookWholeMap && !following && !GameManager.GetInstance().hitButton && !GameManager.GetInstance().isPause) 
    {      
      if (GameManager.GetInstance ().isTouch) 
      {
        if (Input.touchCount > 0)
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
        else if (Input.touchCount > 1)
        {
          if (Input.touchCount == 2) PinchToZoom (Input.GetTouch (0), Input.GetTouch (1));
        }
      } 
      else
      {
        if (Input.touchCount == 2) 
        {
          PinchToZoom (Input.GetTouch (0), Input.GetTouch (1));
        }
      }
    }
	}
    
  public void SetUpStartCamera(Vector3 _target)
  {
    following = false;
    isLookWholeMap = false;
    _orthographicSize = _target.x *3;
    this.GetComponent<Camera> ().orthographicSize = _orthographicSize;

    transform.position = _target;
    _originPos = transform.position;
    
    transform.eulerAngles = new Vector3 (90, 0, 0);
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

      int mutilplyTargetCam = 0;

      if (follower.GetComponent<Character>() != null) 
      {
        mutilplyTargetCam = GameManager.GetInstance ()._mapSize [0] - (int)follower.GetComponent<Character> ().gridPosition.x;
      } 
      else if (follower.GetComponent<Tile>() != null)
      {
        mutilplyTargetCam = GameManager.GetInstance ()._mapSize [0] - (int)follower.GetComponent<Tile> ().gridPosition.x;
      }
      targetCamPos.x += Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));
      targetCamPos.y += Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));
      targetCamPos.z -= Mathf.Abs (follower.position.x + (mutilplyTargetCam*2));
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
