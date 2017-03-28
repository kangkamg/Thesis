using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCharacterInfo : MonoBehaviour
{
  Character characterStatus;
	
  public void UpdateInfo(Character target)
  {
    characterStatus = target;
    this.GetComponent<Text> ().text = characterStatus.currentHP.ToString();
  }

  public void Update()
  {
    UpdateInfoPos ();
  }

  public void UpdateInfoPos()
  {
      Vector2 screenPosition = Camera.main.WorldToScreenPoint (characterStatus.transform.position);

      this.GetComponent<Text> ().text = characterStatus.currentHP.ToString ();

      transform.position = new Vector2 (screenPosition.x - 9f, screenPosition.y - 15f);
  }
}
