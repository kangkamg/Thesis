using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : StateMachineBehaviour
{
  private Character _target;
  private int _amountOfResults;
  
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
  {
    amountOfHit (_target, _amountOfResults);
    animator.SetInteger  ("animatorIndex", 0);
  }

  public void amountOfHit(Character target, int amountOfResults)
  {
    Animator targetAnim = target.transform.GetChild(0).GetComponent<Animator> ();
    targetAnim.SetInteger ("animatorIndex", 2);
    target.currentHP += amountOfResults;
    if(amountOfResults <= 0) GameManager.GetInstance().FloatingTextController (amountOfResults*-1, target.transform);
    else GameManager.GetInstance().FloatingTextController  (amountOfResults, target.transform);
    if (target.GetType () == typeof(AICharacter)) FinishingGaugeManager.GetInstance ().ChangeSliderValue (5);
    else FinishingGaugeManager.GetInstance ().ChangeSliderValue (2.5f);
  }

  public void AddingTarget(Character target, int amountOfResults)
  {
    _target = target;
    _amountOfResults = amountOfResults;
  }
}
