using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SystemManager 
{
  public static void CheckingLevelUp(CharacterStatus characterStatus)
  {
    for (int i = 0; i < characterStatus.basicStatus.learnAbleAbility.Count; i++) 
    {
      AbilityStatus learning = new AbilityStatus ();
      string[] learnAbleAb = characterStatus.basicStatus.learnAbleAbility [i].Split (" " [0]);
      for (int j = 0; j < learnAbleAb.Length; j = j + 2)
      {
        if (int.Parse (learnAbleAb [j + 1]) == characterStatus.characterLevel && characterStatus.learnedAbility.Where(x=>x.ability.ID == int.Parse(learnAbleAb[j])).Count() <= 0) 
        {
          learning.ability = GetDataFromSql.GetAbility (int.Parse (learnAbleAb [j]));
          learning.level = 1;
          learning.exp = 0;
          learning.ordering = -1;
          characterStatus.learnedAbility.Add (learning);
        }
      }
    }
  }
}
