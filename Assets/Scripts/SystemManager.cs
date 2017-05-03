using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class SystemManager 
{  
  public static bool isFinishLevelUp = false;
  
  public static IEnumerator LevelUpSystem(CharacterStatus characterStatus, int exp, Transform showingSlider)
  {
    int getExp = exp;
    showingSlider.GetComponent<Slider> ().maxValue = 100;
    showingSlider.GetComponent<Slider> ().value = (characterStatus.experience*100)/characterStatus.nextLevelExp;
    
    while (getExp > 0) 
    {
      if (/*Input.GetMouseButton (0)*/Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) 
      {
        if (characterStatus.experience < characterStatus.nextLevelExp) 
        {
          showingSlider.parent.GetChild (1).gameObject.SetActive (false);
          characterStatus.experience++;
          getExp--;   
          showingSlider.GetComponent<Slider> ().value = Mathf.Lerp (showingSlider.GetComponent<Slider> ().value, (characterStatus.experience * 100) / characterStatus.nextLevelExp,
            ((characterStatus.experience * 100) / characterStatus.nextLevelExp - showingSlider.GetComponent<Slider> ().value) * 10);
        
          if (characterStatus.experience == characterStatus.nextLevelExp)
          {
            characterStatus.characterLevel += 1;
            characterStatus.experience = 0;
            showingSlider.GetComponent<Slider> ().value = 0;
            AddingAbility (characterStatus);
            showingSlider.parent.GetChild (1).gameObject.SetActive (true);
            showingSlider.parent.GetChild (1).GetComponent<Animator> ().Play ("LevelUpFloating");
          }
          isFinishLevelUp = true;
        }
      }
      else
      {
        if (characterStatus.experience < characterStatus.nextLevelExp) 
        {
          characterStatus.experience++;
          getExp--;   
          showingSlider.GetComponent<Slider> ().value = Mathf.Lerp (showingSlider.GetComponent<Slider> ().value, (characterStatus.experience * 100) / characterStatus.nextLevelExp,
            ((characterStatus.experience * 100) / characterStatus.nextLevelExp - showingSlider.GetComponent<Slider> ().value) * 10);
          
          yield return new WaitForEndOfFrame ();

          if (characterStatus.experience == characterStatus.nextLevelExp)
          {
            characterStatus.characterLevel += 1;
            characterStatus.experience = 0;
            showingSlider.GetComponent<Slider> ().value = 0;
            AddingAbility (characterStatus);
            showingSlider.parent.GetChild (1).gameObject.SetActive (true);
            showingSlider.parent.GetChild (1).GetComponent<Animator> ().Play ("LevelUpFloating");
          }
          
          if(getExp <= 0) isFinishLevelUp = true;
          else isFinishLevelUp = false;
        }
      }
    }
    
    showingSlider.parent.GetChild (1).gameObject.SetActive (false);
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
  
  public static void AddCharacterToParty(int characterID, List<int>equipItemID)
  {
    AddCharacterToParty (characterID, equipItemID,1, false);
  }
  
  public static void AddCharacterToParty(int characterID, List<int>equipItemID, int characterLevel = 1, bool addToParty = false)
  {
    CharacterStatus adding = new CharacterStatus ();
    AbilityStatus equiped = new AbilityStatus ();
    AbilityStatus learning = new AbilityStatus ();
    adding.basicStatus = GetDataFromSql.GetCharacter (characterID);
    adding.characterLevel = characterLevel;
    adding.isInParty = addToParty;
    
    for(int i= 0; i < adding.basicStatus.learnAbleAbility.Count;i++)
    {
      string[] learnAbleAb = adding.basicStatus.learnAbleAbility [i].Split (" " [0]);
      for(int j = 0; j < learnAbleAb.Length; j=j+2)
      {
        if(int.Parse(learnAbleAb[j+1]) <= adding.characterLevel)
        {
          learning = new AbilityStatus ();
          learning.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          learning.level = 1;
          learning.exp = 0;
          adding.learnedAbility.Add (learning);

          equiped = new AbilityStatus ();
          equiped.ability = GetDataFromSql.GetAbility(int.Parse(learnAbleAb [j]));
          equiped.level = 1;
          equiped.exp = 0;
          adding.equipedAbility.Add (equiped);
        }
      }
    }
    adding.partyOrdering = 0;
    adding.experience = 0;
    
    if (equipItemID.Count > 0)
    {
      foreach (int ID in equipItemID)
      {
        Item equipedItem = new Item ();
        equipedItem.item = GetDataFromSql.GetItemFromID (ID);
        SetUpEquipment (equipedItem, adding, TemporaryData.GetInstance ().playerData);
      }
    }
    
    TemporaryData.GetInstance ().playerData.characters.Add (adding);
  }
  
  private static void SetUpEquipment(Item checking, CharacterStatus adding, PlayerData data, bool equiped = true)
  {
    if (checking.item != null)
    {
      checking.equiped = equiped;
      if(equiped) adding.equipItem.Add (checking);
      data.inventory.Add (checking);
      checking.ordering = data.inventory.Count - 1;
    }
  }
}
