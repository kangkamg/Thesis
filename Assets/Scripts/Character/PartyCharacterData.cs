using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyCharacterData : MonoBehaviour
{
  public CharacterData characterData = new CharacterData();

  private void Start()
  {
    characterData.canvasGroup = GetComponent<CanvasGroup> ();

    transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + characterData.status.basicStatus.characterName);
    transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Text> ().text = characterData.status.basicStatus.characterName.ToString();
    transform.GetChild (0).GetChild (2).GetChild (0).GetComponent<Text> ().text = characterData.status.characterLevel.ToString();
    transform.GetChild (0).GetChild (3).GetChild (0).GetComponent<Text> ().text = characterData.status.maxHp.ToString();
  }
    
}
