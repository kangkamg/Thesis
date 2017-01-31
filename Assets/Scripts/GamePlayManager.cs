using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GamePlayManager : MonoBehaviour
{
  private static GamePlayManager instance = null;

  private List<GameObject> player = new List<GameObject>();
  private List<GameObject> enemy = new List<GameObject>();

  public bool isPlayerTurn = true;

  public static GamePlayManager GetInstance()
  {
    return instance;
  }

  private void Awake()
  {
    instance = GetComponent<GamePlayManager> ();
    GameObject[] cha = GameObject.FindGameObjectsWithTag ("Player");
    GameObject[] ene = GameObject.FindGameObjectsWithTag ("Enemy");

    foreach (GameObject a in cha) 
    {
      player.Add (a);
    }
    foreach (GameObject a in ene) 
    {
      enemy.Add (a);
    }
  }

  private void Update()
  {
    ChangeTurn ();
  }

  private void ChangeTurn()
  {
    if (isPlayerTurn)
    {
      for (int i = 0; i < player.Count; i++)
      {
        if (player.All (x => player [i].GetComponent<CharacterManager> ().played))
        {
          isPlayerTurn = false;
          enemy [i].GetComponent<CharacterManager> ().played = false;
        }
      }
    } 
    else 
    {
      for (int i = 0; i < enemy.Count; i++)
      {
        if (enemy.All (x => enemy [i].GetComponent<CharacterManager> ().played))
        {
          isPlayerTurn = true;
          player [i].GetComponent<CharacterManager> ().played = false;
        }
      }
    }
  }
}
