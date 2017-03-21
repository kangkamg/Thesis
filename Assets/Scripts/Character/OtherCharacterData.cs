using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherCharacterData : MonoBehaviour
{
  public CharacterData characterData = new CharacterData();

  private void Start()
  {
    characterData.canvasGroup = GetComponent<CanvasGroup> ();

    transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("PlayerPrefab/" + characterData.status.basicStatus.characterName);
    transform.GetChild (1).gameObject.SetActive (characterData.status.isInParty);
    transform.GetComponent<Button> ().enabled = !characterData.status.isInParty;
  }
}
