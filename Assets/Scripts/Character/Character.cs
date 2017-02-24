using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class CharacterStatus
{
  public int ID;
  public string characterName;
  public int level,experience;
  public int maxHP = 25;
  public int attack;
  public int defense;
  public int resistance;
  public int movementPoint = 5;
  public float criRate = 0.10f;
  public float guardRate = 0.10f;
  public string job;
  public string type;
  public List<string> weaponProficiency = new List<string> ();
}

public class Character : MonoBehaviour
{
  public string characterName;

  public List<Vector3>positionQueue = new List<Vector3>();
 
  public float moveSpeed = 10;

  public Vector3 gridPosition = Vector3.zero;

  public int currentHP;
  public int ordering;

  public bool floating = false;
  public bool stunning = false;
  public bool poisoning = false;
  public bool blinding = false;

  public bool played = false;

  public bool isAI;

  public Character target;

  public CharacterStatus characterStatus = new CharacterStatus();
  public List<Ability> setupAbility = new List<Ability>();
  public List<Item> characterItem = new List<Item>();

  public Slider hpSlider;

  public int attackOverAll
  {
    get 
    { int ret = characterStatus.attack;
      foreach (Item i in characterItem) 
      {
        if (i.itemType == "Weapon")
        {
          ret += i.increaseAttack;
        }
      }
      return ret;
    }
    set{ }
  }

  public virtual void Awake()
  {
    characterStatus = GetDataFromSql.GetCharacter (characterName);

    characterItem.Add (GetDataFromSql.GetItemFromName ("WoodSword"));

    foreach (Item i in characterItem)
    {
      foreach (Ability a in i.itemAb)
      {
        setupAbility.Add (a);
      }
    }

    currentHP = characterStatus.maxHP;

    hpSlider.maxValue = characterStatus.maxHP;
    hpSlider.value = currentHP;
  }
   
  public virtual void Update()
  {
    if (currentHP <= 0)
    {
      Destroy (gameObject);
    }
  }

  public virtual void TurnUpdate()
  {
   
  }
}
