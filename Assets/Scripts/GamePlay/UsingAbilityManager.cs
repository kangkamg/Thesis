using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UsingAbilityManager : MonoBehaviour 

{
  public AbilityStatus data;
  
  public void SelectThisAbility()
  {
    GameManager.GetInstance ().SelectedAbility (data);
  }
}
