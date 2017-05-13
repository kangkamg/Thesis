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

  public void SetText(int value, bool Critical)
  {
    popUpText.text = value.ToString ();
    
    if (Critical) 
    {
      popUpText.fontSize = 150;
      popUpText.color = new Color (1, 0.5f, 0);
    }
    else
    {
      popUpText.fontSize = 120;
      popUpText.color = new Color (1, 0, 0);
    }
    
  }
}
