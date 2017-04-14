using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class SystemManager 
{  
  public static IEnumerator LevelUpSystem(CharacterStatus characterStatus, int exp, Transform showingSlider)
  {
    int getExp = exp;
    showingSlider.GetComponent<Slider> ().maxValue = 100;
    showingSlider.GetComponent<Slider> ().value = (characterStatus.experience*100)/characterStatus.nextLevelExp;
    while (getExp > 0) 
    {
      if (characterStatus.experience < characterStatus.nextLevelExp)
      {
        characterStatus.experience++;
        getExp--;   
        showingSlider.GetComponent<Slider> ().value = Mathf.Lerp(showingSlider.GetComponent<Slider> ().value,(characterStatus.experience*100)/characterStatus.nextLevelExp,
          ((characterStatus.experience*100)/characterStatus.nextLevelExp - showingSlider.GetComponent<Slider> ().value) * 10 );
     
        yield return new WaitForEndOfFrame ();
        
        if (characterStatus.experience == characterStatus.nextLevelExp)
        {
          characterStatus.characterLevel += 1;
          characterStatus.experience = 0;
          showingSlider.GetComponent<Slider> ().value = 0;
        }
      }
    }
    
    while (getExp > 0) 
    {
      if (characterStatus.experience < characterStatus.nextLevelExp)
      {
        characterStatus.experience ++;
        getExp --;
        showingSlider.GetComponent<Slider> ().value++;

        if (characterStatus.experience == characterStatus.nextLevelExp)
        {
          characterStatus.characterLevel += 1;
          characterStatus.experience = 0;
          showingSlider.GetComponent<Slider> ().maxValue = characterStatus.nextLevelExp;
          showingSlider.GetComponent<Slider> ().value = 0;
        }
      }
    }
    
    AddingAbility (characterStatus);
    yield return 0;
  }
  
  public static void AddingAbility(CharacterStatus characterStatus)
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
          characterStatus.learnedAbility.Add (learning);
        }
      }
    }
  }
  
  public static void SaveGameData()
  {
    float playedHrs = Time.realtimeSinceStartup;
    
    TemporaryData.GetInstance ().playerData.playedHrs += playedHrs;
    SaveAndLoadPlayerData.SaveData (TemporaryData.GetInstance ().playerData,TemporaryData.GetInstance().playerData.id);
  }
}
