using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MultipleLanguage
{
  public string key;
  public string th;
  public string en;
}

public class LanguageDatabase
{
  private static Dictionary<string, MultipleLanguage> languageDB = new Dictionary<string, MultipleLanguage>();
  
  public static Dictionary<string, MultipleLanguage> _LanguageDB()
  {
    languageDB = new Dictionary<string, MultipleLanguage> ();
    
    MultipleLanguage newLang = new MultipleLanguage () 
    {
      key = "Tutorial1.1",
      th = "แตะที่ตัวละคร เพื่อเลือกตัวนั้น",
      en = "Touch the Character to select"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial1.2",
      th = "แตะที่ช่องระยะเดิน(ช่องสีฟ้า) เพื่อให้ตัวละครที่เลือก  เดินไปช่องนั้น",
      en = "Touch movement zone (bluetile) to move character to that selected tile"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial1.3",
      th = "ถ้าตัวละครที่เลือกไม่สามารถโจมตีได้ หรือไม่ต้องการใช้ตัวละครนั้นโจมตี สามารถกดปุ่มนี้ เพื่อสิ้นตาของตัวละคร",
      en = "If selected character can't attack or don't want to attack touch this button to finished turn of character"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial2.1",
      th = "หากมีช่องสีเหลือง ปรากฎขึ้นที่ศัตรู สามารถแตะที่ศัตรู เพื่อใช้ตัวละครตัวนั้น โจมตีได้ ",
      en = "If enemy has yellow tile below mean they can attacked, touch to attack them"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial2.2",
      th = "สามารถเลือกใช้ท่าโจมตีได้ จากการแตะที่ท่าโจมตี บริเวณมุมล่างขวา ",
      en = "Touch the ability in panel at the bottom right corner to change the attack ability"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial2.3",
      th = "ท่าโจมตีบางท่า ต้องการเกจไม้ตาย ซึ่งสะสมจาก การโจมตีหรือถูกโจมตี",
      en = "Some of abilities need to use finished strike guage that get from attacking or attacked"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.1",
      th = "ค่าสถานะ ของตัวละครที่เลือก",
      en = "Status of selected character"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.2",
      th = "หากไม่ถนัดการแตะด้วยนิ้ว สามารถกดปุ่มนี้ เพื่อเปิดคอนโทรลเลอร์ขึ้นมา",
      en = "If touch the screen is to hard touch this button to open controller"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.3",
      th = "เลื่อนไปทางบนซ้าย",
      en = "Move northwest direction"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.4",
      th = "เลื่อนไปทางล่างซ้าย",
      en = "Move southwest direction"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.5",
      th = "เลื่อนไปทางบนขวา",
      en = "Move northeast direction"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.6",
      th = "เลื่อนไปทางล่างขวา",
      en = "Move southeast direction"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial3.7",
      th = "เลือกช่องที่ถูกไฮท์ไลท์",
      en = "Select the hightlighted tile"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial4.1",
      th = "แตะและเลื่อนเพื่อขยับกล่องตาม",
      en = "Touch and swipe to move camera"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tutorial4.2",
      th = "ใช้สองนิ้วเพื่อซูมเข้าและซูมออก",
      en = "Pinch to zoom in and out"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Save",
      th = "เซฟ",
      en = "Save"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "MainMenu",
      th = "กลับสู่หน้าเริ่มเกม",
      en = "Main Menu"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "ExitGame",
      th = "ออกจากเกม",
      en = "Exit Game"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Options",
      th = "ตั้งค่า",
      en = "Options"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Shop",
      th = "ร้านค้า",
      en = "Shop"
    };
    languageDB.Add (newLang.key, newLang);
     
    newLang = new MultipleLanguage () 
    {
      key = "Infomations",
      th = "ข้อมูลตัวละคร",
      en = "Infomations"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Story",
      th = "เนื้อเรื่อง",
      en = "Stories"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Tale",
      th = "นิทานและวรรณกรรม",
      en = "TALE AND LITERATURE"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Mythology",
      th = "เทพปกรณรรม",
      en = "MYTHOLOGY"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Historical",
      th = "ประวัติศาสตร์",
      en = "HISTORICAL"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Party",
      th = "ปาร์ตี้",
      en = "PARTY"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Status",
      th = "สถานะตัวละคร",
      en = "CHARACTERS STATUS"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Inventory",
      th = "ไอเทมทั้งหมด",
      en = "INVENTORY"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "StoryBook",
      th = "หนังสือทั้งหมด",
      en = "STORYBOOK"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Buy",
      th = "ซื้อ",
      en = "BUY"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Sell",
      th = "ขาย",
      en = "SELL"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "Kraitong",
      th = "ไกรทอง",
      en = "Kraitong"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "1",
      th = "๑",
      en = "1"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "2",
      th = "๒",
      en = "2"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "3",
      th = "๓",
      en = "3"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "4",
      th = "๔",
      en = "4"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "TouchToStart",
      th = "แตะเพื่อเริ่ม",
      en = "Touch Any Where To Start"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "NewGame",
      th = "เริ่มเกมใหม่",
      en = "New Game"
    };
    languageDB.Add (newLang.key, newLang);
    
    newLang = new MultipleLanguage () 
    {
      key = "LoadGame",
      th = "โหลดเกม",
      en = "Load Game"
    };
    languageDB.Add (newLang.key, newLang);
    
    return languageDB;
  }
        
  public static string GetText(string key)
  {
    MultipleLanguage temp = null;
    string text = null;
    
    if (_LanguageDB().TryGetValue(key, out temp))
    {
      if (TemporaryData.GetInstance ().choosenLanguage == "Thai") 
      {
        text = temp.th;
        return text;
      }
      else  if (TemporaryData.GetInstance ().choosenLanguage == "English") 
      {
        text = temp.en;
        return text;
      }
    }
     return null;
  }
}
