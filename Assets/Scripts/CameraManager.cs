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

  public float smoothing;

  void Awake()
  {
    instance = GetComponent<CameraManager> ();
  }
    
	void LateUpdate () 
  {
    if (GameManager.GetInstance ().selectedCharacter != null)
    {
      Vector3 targetCamPos = GameManager.GetInstance ().selectedCharacter.transform.position - new Vector3(7,0,7);

      targetCamPos.y = transform.position.y;

      transform.position = Vector3.SmoothDamp (transform.position, targetCamPos, ref velocity, smoothing * Time.deltaTime);
    }
	}
}
