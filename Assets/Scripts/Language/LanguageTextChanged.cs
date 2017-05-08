using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LanguageTextChanged : MonoBehaviour

{
  public string keyLanguage;
  
  private void OnEnable()
  {
    SetLanguage (keyLanguage);
  }
  
  private void Update()
  {
    SetLanguage (keyLanguage);
  }
  
  public void SetLanguage(string _keyLanguage)
  {
     this.transform.GetComponent<Text>().text =  LanguageDatabase.GetText (_keyLanguage);
    keyLanguage = _keyLanguage;
  }
}
