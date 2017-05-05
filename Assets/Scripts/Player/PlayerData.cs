﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
  public int id;
  public string name;
  public List<Quest> acceptedQuest = new List<Quest>();
  public List<CharacterStatus> characters = new List<CharacterStatus>();
  public List<int> passedMap = new List<int> ();
  public int gold;
  public List<Item> inventory = new List<Item>();
  public List<string> readedStories = new List<string> ();
  public int chapter;
  public int storyID;
  public float playedHrs;
}

public class DroppedItem
{
  public ItemStatus itemStatus = new ItemStatus ();
  public int amount = 0;
}

public class Result
{
  public List<DroppedItem> droppedItem = new List<DroppedItem>();
  public int givenExp = 0;
  public int givenGold = 0;
}

public class TemporaryData
{
  public Result result = new Result();

  public PlayerData playerData;

  public CharacterStatus selectedCharacter = new CharacterStatus();

  public bool firstTimeOpenGame = false;
  public bool isTutorialDone = false;
  public string choosenLanguage = null;
  
  private static TemporaryData instance;
  
  public string storyPlayingName;

  public static TemporaryData GetInstance()
  {
    if (instance == null) 
    {
      instance = new TemporaryData ();
    }
    return instance;
  }
}