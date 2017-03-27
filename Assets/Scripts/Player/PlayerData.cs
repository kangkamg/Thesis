using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
  public string name;
  public int rewardGold;
  public int rewardExp;
  public List<Item> rewardItem = new List<Item>();
  public string detail;
}

[System.Serializable]
public class PlayerData
{
  public string id;
  public string name;
  public List<Quest> acceptedQuest = new List<Quest>();
  public List<CharacterStatus> characters = new List<CharacterStatus>();
  public int gold;
  public List<Item> inventory = new List<Item>();
  public int chapter;
  public int storyID;
}

public class Result
{
  public List<ItemStatus> droppedItem = new List<ItemStatus>();
  public int givenExp = 0;
  public int givenGold = 0;
}

public class TemporaryData
{
  public Result result = new Result();

  public PlayerData playerData;

  public CharacterStatus selectedCharacter = new CharacterStatus();

  public int storyID = 0;

  public bool firstTimeOpenGame = false;

  private static TemporaryData instance;

  public static TemporaryData GetInstance()
  {
    if (instance == null) 
    {
      instance = new TemporaryData ();
    }
    return instance;
  }
}