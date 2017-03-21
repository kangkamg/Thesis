using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillStatusManager : MonoBehaviour 
{
  public AbilityStatus ability;

  public GameObject skillExp;

  private void Start()
  {
    transform.GetChild (0).GetComponent<Text> ().text = ability.ability.abilityName.ToString();
    transform.GetChild (1).GetComponent<Text> ().text = ability.level.ToString();

    for(int i = 0;i<ability.level*2;i++)
    {
      GameObject exp = Instantiate (skillExp);
      exp.transform.SetParent (transform.GetChild (2));
    }
    transform.GetChild (3).GetComponent<Text> ().text = "Power:\t" + ability.power.ToString () +"\nHitAmount:\t" + ability.hitAmount.ToString () +"\nRange:\t" + ability.range.ToString ();
  }
}
