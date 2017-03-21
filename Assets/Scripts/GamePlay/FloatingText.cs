using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour

{
  public Animator anim;
  public Text popUpText;

  private void Start()
  {
    AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo (0);
    Destroy (gameObject, clipInfo [0].clip.length);
  }

  public void SetText(int value)
  {
    popUpText.text = value.ToString ();
  }
}
