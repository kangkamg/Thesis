using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour 
{
  GameObject tutorial;
  int pages = 1;
	private void Awake ()
  {
    Time.timeScale = 0f;
	}
  
  private void Start()
  {
    tutorial = Instantiate (Resources.Load<GameObject> ("GamePlay/Tutorial/1"));
    tutorial.name = "Tutorial";
    tutorial.transform.SetParent (GameObject.Find ("Canvas").transform);
    tutorial.transform.localScale = Vector3.one;
    tutorial.transform.localPosition = Vector2.zero;
    tutorial.GetComponent<Button> ().onClick.AddListener (() => NextTutorial ());
    int pages = 1;
  }
  
  public void NextTutorial()
  {
    if (pages < 4) 
    {
      pages += 1;
      GenerateNextTutorial ();
    }
    else
    {
      Destroy (tutorial);
      Time.timeScale = 1f;
    }
  }
  
  public void GenerateNextTutorial()
  {
    Destroy (tutorial);
    tutorial = Instantiate (Resources.Load<GameObject> ("GamePlay/Tutorial/" + pages));
    tutorial.transform.SetParent (GameObject.Find ("Canvas").transform);
    tutorial.transform.localScale = Vector3.one;
    tutorial.transform.localPosition = Vector2.zero;
    tutorial.GetComponent<Button> ().onClick.AddListener (() => NextTutorial ());
  }
}
