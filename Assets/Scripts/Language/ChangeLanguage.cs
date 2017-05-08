using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour 

{
  private void Start()
  {
    if (TemporaryData.GetInstance ().choosenLanguage == "English") 
      this.transform.GetChild(0).GetComponent<Text> ().text = "EN";
    else if (TemporaryData.GetInstance ().choosenLanguage == "Thai") 
      this.transform.GetChild(0).GetComponent<Text> ().text = "TH";
  }
  public void _ChangeLanguage()
  {
    if (TemporaryData.GetInstance ().choosenLanguage == "English") 
    {
      TemporaryData.GetInstance ().choosenLanguage = "Thai";
      PlayerPrefs.SetString (Const.Language, "Thai");
      this.transform.GetChild(0).GetComponent<Text> ().text = "TH";
    }
    else if (TemporaryData.GetInstance ().choosenLanguage == "Thai") 
    {
      TemporaryData.GetInstance ().choosenLanguage = "English";
      PlayerPrefs.SetString (Const.Language, "English");
      this.transform.GetChild(0).GetComponent<Text> ().text = "EN";
    }
  }
}
