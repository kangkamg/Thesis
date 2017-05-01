using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusSceneManager : MonoBehaviour
{
  public GameObject characterPanel;
  public GameObject characterInTeams;
  public GameObject changingItemObj;
  public GameObject selectEquipmentArrow;

  public Transform selectedItem;
  public Transform selectingArrow;

  public GameObject mainPage;
  public GameObject statusPage;
  public GameObject equipmentPage;
  public GameObject abilityPage;

  public List<CharacterStatus> allCharacters = new List<CharacterStatus> ();
  public List<GameObject> allCharacterSlots = new List<GameObject> ();

  public int selectedCharacter;

  private static CharacterStatusSceneManager instance;

  public static CharacterStatusSceneManager GetInstance()
  {
    return instance;
  }

  private void Awake()
  {
    instance = GetComponent<CharacterStatusSceneManager>();
  }

  private void Start()
  {
    GenerateCharacter ();
    this.transform.GetChild (0).gameObject.SetActive (false);
  }
  
  public void GenerateCharacter()
  {
    foreach (GameObject a in allCharacterSlots)
    {
      Destroy (a);
    }
    allCharacters.Clear ();
    allCharacterSlots.Clear ();
    TemporaryData.GetInstance ().selectedCharacter = null;

    for (int i = 0; i < TemporaryData.GetInstance().playerData.characters.Count; i++)
    {
      allCharacters.Add (TemporaryData.GetInstance ().playerData.characters [i]);
      GameObject characterObj = Instantiate (characterInTeams);
      characterObj.transform.SetParent (characterPanel.transform);
      characterObj.GetComponentInChildren<AllCharacterData> ().characterStatus = TemporaryData.GetInstance ().playerData.characters [i];
      characterObj.GetComponentInChildren<Button>().onClick.AddListener(()=>LookStatus(characterObj.GetComponentInChildren<AllCharacterData>().characterStatus));
      characterObj.transform.localScale = Vector3.one;
      characterObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + characterObj.GetComponentInChildren<AllCharacterData> ().characterStatus.basicStatus.characterName);

      allCharacterSlots.Add (characterObj);
    }
    
    if (allCharacterSlots.Count > 6) 
    {
      characterPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (characterPanel.GetComponent<RectTransform> ().sizeDelta.x, 340f * (allCharacterSlots.Count));
      characterPanel.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      characterPanel.GetComponentInParent<ScrollRect> ().vertical = false;
    }

    characterPanel.GetComponent<RectTransform> ().anchoredPosition  = new Vector2 (0, -characterPanel.GetComponent<RectTransform> ().rect.height / 2);
    
    characterPanel.transform.parent.parent.GetComponent<RectTransform>().position = new Vector2 (characterPanel.transform.parent.parent.position.x, characterPanel.transform.parent.parent.position.y + 50);
    
  }

  public void LookStatus(CharacterStatus selectedStatus)
  {
    if (selectedStatus != TemporaryData.GetInstance ().selectedCharacter)
    {
      if(!this.transform.GetChild(0).gameObject.activeSelf)
      {
        characterPanel.transform.parent.parent.position = new Vector2 (characterPanel.transform.parent.parent.position.x, characterPanel.transform.parent.parent.position.y - 50);
      }
      TemporaryData.GetInstance ().selectedCharacter = selectedStatus;
      this.transform.GetChild (0).gameObject.SetActive (true);
      Transform selectedCharacterStatus = this.transform.GetChild (0).GetChild (0);

      selectedCharacterStatus.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName);
      selectedCharacterStatus.GetChild (0).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.basicStatus.characterName.ToString ();
      selectedCharacterStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.characterLevel.ToString ();
      selectedCharacterStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.maxHp.ToString ();
      selectedCharacterStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.attack.ToString ();
      selectedCharacterStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.defense.ToString ();
      selectedCharacterStatus.GetChild (5).GetChild (0).GetComponent<Text> ().text = TemporaryData.GetInstance ().selectedCharacter.criRate.ToString ();
    } 
    else 
    {
      this.transform.GetChild (0).gameObject.SetActive (false);
      
      mainPage.SetActive (false);
      equipmentPage.SetActive (false);
      statusPage.SetActive (true);

      statusPage.GetComponent<ShowingCharacterStatusManager> ().UpdateStatus ();
    }
  }
}
