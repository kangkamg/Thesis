using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelManager : MonoBehaviour 
{
  public SkinnedMeshRenderer[] materials;
  
  public void AttackTarget()
  {
    transform.GetComponentInParent<Character> ().AttackTarget ();
  }
}
