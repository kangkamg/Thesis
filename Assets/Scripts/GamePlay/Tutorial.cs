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
    StartCoroutine (StartTutorial ());
  }
  
  public void NextTutorial()
  {
    if (pages < 4) 
    {
      pages += 1;
      StartCoroutine(GenerateNextTutorial ());
    }
    else
    {
      Destroy (tutorial);
      Time.timeScale = 1f;
    }
  }
  
  public IEnumerator StartTutorial()
  {
    yield return StartCoroutine (Tutorial_ ());
  }
  
  public IEnumerator Tutorial_()
  {
    tutorial = Instantiate (Resources.Load<GameObject> ("GamePlay/Tutorial/1"));
    tutorial.name = "Tutorial";
    tutorial.transform.SetParent (GameObject.Find ("Canvas").transform);
    tutorial.transform.localScale = Vector3.one;
    tutorial.transform.localPosition = Vector2.zero;
    tutorial.GetComponent<Button> ().onClick.AddListener (() => NextTutorial ());
    pages = 1;
    yield return 0;
  }
  
  public IEnumerator GenerateNextTutorial()
  {
    Destroy (tutorial);
    tutorial = Instantiate (Resources.Load<GameObject> ("GamePlay/Tutorial/" + pages));
    tutorial.transform.SetParent (GameObject.Find ("Canvas").transform);
    tutorial.transform.localScale = Vector3.one;
    tutorial.transform.localPosition = Vector2.zero;
    tutorial.GetComponent<Button> ().onClick.AddListener (() => NextTutorial ());
    yield return 0;
  }
}
