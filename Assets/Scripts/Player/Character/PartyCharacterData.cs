using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyCharacterData : MonoBehaviour
{
  public CharacterStatus characterStatus = new CharacterStatus();

  private void Start()
  {
    transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + characterStatus.basicStatus.characterName);
    transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = characterStatus.basicStatus.characterName.ToString();
    transform.GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = characterStatus.characterLevel.ToString();
    transform.GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = characterStatus.maxHp.ToString();
  }
    
}
