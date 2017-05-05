using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowingResultOfAttack : MonoBehaviour 
{
  public Transform selectedData;
  public Transform targetData;
  public Image arrow;

  public void UpdateStatus(Character selectedCharacter,Character targetCharacter, int amountOfResult)
  {
    this.gameObject.SetActive (true);
    GameManager.GetInstance ().playerUI.transform.GetChild (0).gameObject.SetActive (false);

    if (Resources.Load<Sprite> ("Image/Character/" + selectedCharacter.name) != null)
    {
      selectedData.GetChild (0).GetComponent<Image> ().enabled = true;
      selectedData.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + selectedCharacter.name); 
    }
    else 
    {
      selectedData.GetChild (0).GetComponent<Image> ().enabled = false;
    }
    selectedData.GetChild (0).GetChild(0).GetComponent<Text> ().text = selectedCharacter.name;
    selectedData.GetChild (1).GetChild(0).GetComponent<Text> ().text = selectedCharacter.currentHP.ToString();
    selectedData.GetChild (2).GetChild(0).GetComponent<Text> ().text = selectedCharacter.characterStatus.attack.ToString();

    if (Resources.Load<Sprite> ("Image/Character/" + targetCharacter.name) != null)
    {
      targetData.GetChild (0).gameObject.SetActive (true);
      targetData.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + targetCharacter.name);
    }
    else 
    {
      targetData.GetChild (0).gameObject.SetActive (false);
    }
    targetData.GetChild (0).GetChild(0).GetComponent<Text> ().text = targetCharacter.name;
    targetData.GetChild (1).GetChild(0).GetComponent<Text> ().text = targetCharacter.currentHP.ToString();
    targetData.GetChild (2).GetChild(0).GetComponent<Text> ().text = targetCharacter.characterStatus.attack.ToString();

    arrow.transform.GetChild (0).GetComponent<Text> ().text = amountOfResult.ToString();
  }
}
