using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour 
{
  public void AttackTarget()
  {
    transform.GetComponentInParent<Character> ().AttackTarget ();
  }
}
