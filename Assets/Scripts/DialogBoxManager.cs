using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogBoxManager 

{
  private static DialogBoxManager _instance;
  public static DialogBoxManager GetInstance()
  {
    _instance = new DialogBoxManager ();
    
    return _instance;
  }
  
  public GameObject GenerateDialogBox(string dialog, bool isYesNo)
  {
    GameObject dialogBox = Resources.Load<GameObject> ("DialogBox/DialogBox");
    dialogBox.transform.GetChild (0).GetComponent<Text> ().text = dialog;
    if (!isYesNo)
    {
      dialogBox.transform.GetChild (2).gameObject.SetActive (false);
      dialogBox.transform.GetChild (1).localPosition = new Vector2 (0, -69.5f);
    }
    return dialogBox;
  }
  
  public void AddChangeScene(string sceneName)
  {
    SceneManager.LoadScene (sceneName);
  }
}
