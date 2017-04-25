using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : StateMachineBehaviour 
{

  private Character _target ;
  private int _amountOfResults = 0;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	//OnStateExit is called when a transition ends and the state machine finishes evaluating this state
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
  {
    amountOfHit (_target, _amountOfResults);
	}
  
  public void amountOfHit(Character target, int amountOfResults)
  {
    Animator targetAnim = target.transform.GetChild(0).GetComponent<Animator> ();
    targetAnim.Play ("Damaged");
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
  
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
  
}
