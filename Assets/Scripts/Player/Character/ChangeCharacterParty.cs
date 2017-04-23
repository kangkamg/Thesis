using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacterParty : MonoBehaviour
{
  public Text[] statusText;
  public Image characterImage;

  public void UpdateStatus()
  {
    characterImage.sprite = Resources.Load<Sprite> ("Image/Character/" + TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName);
    statusText [0].text = TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName.ToString();
    statusText[1].text = TemporaryData.GetInstance ().selectedCharacter.characterLevel.ToString();
    statusText[2].text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString();
  }
}
