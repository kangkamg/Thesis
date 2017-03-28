using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UsingAbilityManager : MonoBehaviour 

{
  private int identity;
  private bool readyToUse = false;
  
  private void Awake()
  {
    identity = int.Parse(this.name.Split (" " [0]) [1]);
    
    if (identity == 0)
      this.GetComponent<Button>().onClick.AddListener(()=>SetSpecialAttack ());
    else
      this.GetComponent<Button>().onClick.AddListener(()=>SetUsingItems ());
  }
  
  public void SetSpecialAttack ()
  {
    GameObject highlighted = new GameObject();
    if (!readyToUse) 
    {
      readyToUse = true;
      GameManager.GetInstance ().usingAbility = GameManager.GetInstance ().selectedCharacter.characterStatus.specialAttack;
      highlighted = PrefabHolder.GetInstance ().AttackTile;
    }
    else
    {
      readyToUse = false;
      GameManager.GetInstance().usingAbility = GameManager.GetInstance().selectedCharacter.characterStatus.normalAttack;
      highlighted = PrefabHolder.GetInstance ().AttackTile;
    }

    GameManager.GetInstance().RemoveMapHighlight ();
    GameManager.GetInstance(). HighlightTileAt (GameManager.GetInstance().originGrid, PrefabHolder.GetInstance ().MovementTile, GameManager.GetInstance().selectedCharacter.characterStatus.movementPoint);
    GameManager.GetInstance().HighlightTileAt (GameManager.GetInstance().selectedCharacter.gridPosition, highlighted, GameManager.GetInstance().usingAbility.range, GameManager.GetInstance().usingAbility.ability.rangeType);
    GameManager.GetInstance().HighlightTargetInRange (GameManager.GetInstance().usingAbility);
  }
  
  public void SetUsingItems ()
  {
    GameObject highlighted = new GameObject();
    if (!readyToUse) 
    {
      readyToUse = true;
      ItemStatus usingItem = GameManager.GetInstance().selectedCharacter.characterStatus.equipItem.Where (x => x.item.itemType1 == "Items").First ().item;
      GameManager.GetInstance().usingAbility = new AbilityStatus ();
      GameManager.GetInstance().usingAbility.ability = GetDataFromSql.itemAbilityStatus (usingItem.ID);
      GameManager.GetInstance().usingAbility.ability.power = usingItem.increaseHP;
      GameManager.GetInstance().usingAbility.level = 1;
      GameManager.GetInstance().usingAbility.ability.hitAmount = 1;
      highlighted = PrefabHolder.GetInstance ().HealingTile;
    }
    else
    {
      readyToUse = false;
      GameManager.GetInstance().usingAbility = GameManager.GetInstance().selectedCharacter.characterStatus.normalAttack;
      highlighted = PrefabHolder.GetInstance ().AttackTile;
    }

    GameManager.GetInstance().RemoveMapHighlight ();
    GameManager.GetInstance(). HighlightTileAt (GameManager.GetInstance().originGrid, PrefabHolder.GetInstance ().MovementTile, GameManager.GetInstance().selectedCharacter.characterStatus.movementPoint);
    GameManager.GetInstance().HighlightTileAt (GameManager.GetInstance().selectedCharacter.gridPosition, highlighted, GameManager.GetInstance().usingAbility.range, GameManager.GetInstance().usingAbility.ability.rangeType);
    GameManager.GetInstance().HighlightTargetInRange (GameManager.GetInstance().usingAbility);
  }
}
