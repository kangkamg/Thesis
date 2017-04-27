using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class PartySceneManager : MonoBehaviour
{
  public GameObject changingPanel;
  public GameObject partyPanel;
  public GameObject character;
  public GameObject selectedToChangeCharacter;
  public GameObject selectedToAdd;

  public List<CharacterStatus> party = new List<CharacterStatus> ();
  public List<CharacterStatus> otherCharacters = new List<CharacterStatus> ();
  public List<GameObject> slotsParty = new List<GameObject> ();
  public List<GameObject> slotsOtherCharacter = new List<GameObject> ();

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
    GenerateParty ();
  }

  public void GenerateParty()
  {
    foreach (GameObject a in slotsParty)
    {
      Destroy (a);
    }
    foreach (GameObject a in slotsOtherCharacter)
    {
      Destroy (a);
    }
    slotsParty.Clear ();
    slotsOtherCharacter.Clear ();
    party.Clear ();
    otherCharacters.Clear ();
    this.transform.GetChild (0).gameObject.SetActive (false);
    this.transform.GetChild (1).gameObject.SetActive (false);
    this.transform.GetChild (4).gameObject.SetActive (false);

    for (int i = 0; i < 4; i++) 
    {
      GameObject slotsPartyObj = Instantiate (Resources.Load<GameObject> ("SupMenu/PartySlots"));
      slotsParty.Add (slotsPartyObj);
      slotsPartyObj.transform.SetParent (partyPanel.transform);
      slotsPartyObj.transform.localScale = Vector3.one;
    }

    for (int i = 0; i < TemporaryData.GetInstance().playerData.characters.Count; i++)
    {
      if (TemporaryData.GetInstance ().playerData.characters [i].isInParty) 
      {
        party.Add (TemporaryData.GetInstance ().playerData.characters [i]);
      }
      else
      {
        GenerateOtherCharacter (TemporaryData.GetInstance ().playerData.characters [i]);
      }
    }

    party.Sort (delegate(CharacterStatus a, CharacterStatus b) 
      {
        return (a.partyOrdering.CompareTo(b.partyOrdering));
      });
    
    for (int i = 0; i < party.Count; i++)
    {
      GameObject partyObj = Instantiate (character);
      partyObj.transform.SetParent (slotsParty[i].transform);
      partyObj.transform.localScale = Vector3.one;
      partyObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + party [i].basicStatus.characterName);
      partyObj.GetComponent<AllCharacterData> ().characterStatus = party[i];
      partyObj.GetComponent<Button> ().onClick.AddListener (() => LookStatus (partyObj));
      partyObj.GetComponent<Button> ().onClick.AddListener (() => CompareStatus (partyObj.GetComponent<AllCharacterData> ().characterStatus,true));
    }
      
    partyPanel.transform.position = new Vector2 (partyPanel.transform.position.x, partyPanel.transform.position.y + 50);
    changingPanel.transform.parent.parent.position = new Vector2 (changingPanel.transform.parent.position.x, changingPanel.transform.parent.position.y + 50);

    if (slotsOtherCharacter.Count > 8)
    {
      changingPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (changingPanel.GetComponent<RectTransform> ().sizeDelta.x , 255f * (slotsOtherCharacter.Count));
      changingPanel.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
    } 
    else
    {
      changingPanel.GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
    }
    
    changingPanel.GetComponent<RectTransform> ().offsetMax = new Vector2 (changingPanel.GetComponent<RectTransform> ().offsetMax.x, 1f);
  }

  public void GenerateOtherCharacter(CharacterStatus status)
  {
    otherCharacters.Add (status);
    GameObject otherObj = Instantiate (character);
    otherObj.transform.SetParent (changingPanel.transform);
    otherObj.GetComponent<AllCharacterData> ().characterStatus = status;
    otherObj.transform.localScale = Vector3.one;
    otherObj.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + status.basicStatus.characterName);
    otherObj.GetComponent<Button> ().onClick.AddListener (() => LookStatus (otherObj));
    otherObj.GetComponent<Button> ().onClick.AddListener (() => CompareStatus (otherObj.GetComponent<AllCharacterData> ().characterStatus,false));
    slotsOtherCharacter.Add (otherObj);
  }

  public void LookStatus(GameObject status)
  {
    if (status.GetComponent<AllCharacterData>().characterStatus.isInParty) selectedToChangeCharacter = status;
    if (!status.GetComponent<AllCharacterData> ().characterStatus.isInParty) 
    {
      if (status != selectedToAdd)
      {
        selectedToAdd = status;
        if (CheckingPartySlots ()) 
        {
          this.transform.GetChild (4).gameObject.SetActive (true);
        }
      } 
      else
      {
        ChangingCharacter ();
      }
    }

    if (!this.transform.GetChild (1).gameObject.activeSelf && !this.transform.GetChild (0).gameObject.activeSelf ) 
    {
      partyPanel.transform.position = new Vector2 (partyPanel.transform.position.x, partyPanel.transform.position.y - 50);
      changingPanel.transform.parent.parent.position = new Vector2 (changingPanel.transform.parent.position.x, changingPanel.transform.parent.position.y - 50);
      this.transform.GetChild (1).gameObject.SetActive (true);
    }
    else if (!this.transform.GetChild (1).gameObject.activeSelf && this.transform.GetChild (0).gameObject.activeSelf ) 
    {
      this.transform.GetChild (1).gameObject.SetActive (true);
      this.transform.GetChild (0).gameObject.SetActive (false);
    }
    Transform selectedCharacterStatus = this.transform.GetChild (1).GetChild (0);

    selectedCharacterStatus.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("Image/Character/"  + status.GetComponent<AllCharacterData>().characterStatus.basicStatus.characterName);
    selectedCharacterStatus.GetChild (0).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.basicStatus.characterName.ToString ();
    selectedCharacterStatus.GetChild (1).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.characterLevel.ToString ();
    selectedCharacterStatus.GetChild (2).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.maxHp.ToString ();
    selectedCharacterStatus.GetChild (3).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.attack.ToString ();
    selectedCharacterStatus.GetChild (4).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.defense.ToString ();
    selectedCharacterStatus.GetChild (5).GetChild (0).GetComponent<Text> ().text = status.GetComponent<AllCharacterData>().characterStatus.criRate.ToString ();
  }
    
  public void CompareStatus(CharacterStatus status, bool isParty)
  {
    if (!isParty)
    {
      if (!this.transform.GetChild (0).gameObject.activeSelf && selectedToChangeCharacter != null)
      {
        this.transform.GetChild (0).gameObject.SetActive (true);
        this.transform.GetChild (1).gameObject.SetActive (false);
        this.transform.GetChild (4).gameObject.SetActive (true);
      }
      if (selectedToChangeCharacter != null) 
      {
        Transform firstCharacter = this.transform.GetChild (0).GetChild (1).GetChild (0);
        Transform secondCharacter = this.transform.GetChild (0).GetChild (2).GetChild (0);

        firstCharacter.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.basicStatus.characterName);
        firstCharacter.GetChild (0).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.basicStatus.characterName.ToString ();
        firstCharacter.GetChild (1).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.characterLevel.ToString ();
        firstCharacter.GetChild (2).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.maxHp.ToString ();
        firstCharacter.GetChild (3).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.attack.ToString ();
        firstCharacter.GetChild (4).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.defense.ToString ();
        firstCharacter.GetChild (5).GetChild (0).GetComponent<Text> ().text = selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.criRate.ToString ();

        secondCharacter.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + status.basicStatus.characterName);
        secondCharacter.GetChild (0).GetChild (0).GetComponent<Text> ().text = status.basicStatus.characterName.ToString ();
        secondCharacter.GetChild (1).GetChild (0).GetComponent<Text> ().text = status.characterLevel.ToString ();
        secondCharacter.GetChild (2).GetChild (0).GetComponent<Text> ().text = status.maxHp.ToString ();
        secondCharacter.GetChild (3).GetChild (0).GetComponent<Text> ().text = status.attack.ToString ();
        secondCharacter.GetChild (4).GetChild (0).GetComponent<Text> ().text = status.defense.ToString ();
        secondCharacter.GetChild (5).GetChild (0).GetComponent<Text> ().text = status.criRate.ToString ();
      }
    }
    else
    {
      if (!this.transform.GetChild (0).gameObject.activeSelf && selectedToAdd != null)
      {
        this.transform.GetChild (0).gameObject.SetActive (true);
        this.transform.GetChild (1).gameObject.SetActive (false);
        this.transform.GetChild (4).gameObject.SetActive (true);
      }
      if (selectedToAdd != null) 
      {
        Transform firstCharacter = this.transform.GetChild (0).GetChild (1).GetChild (0);
        Transform secondCharacter = this.transform.GetChild (0).GetChild (2).GetChild (0);

        secondCharacter.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/"  + selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.characterName);
        secondCharacter.GetChild (0).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.characterName.ToString ();
        secondCharacter.GetChild (1).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.characterLevel.ToString ();
        secondCharacter.GetChild (2).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.maxHp.ToString ();
        secondCharacter.GetChild (3).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.attack.ToString ();
        secondCharacter.GetChild (4).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.defense.ToString ();
        secondCharacter.GetChild (5).GetChild (0).GetComponent<Text> ().text = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.criRate.ToString ();

        firstCharacter.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Image/Character/" + status.basicStatus.characterName);
        firstCharacter.GetChild (0).GetChild (0).GetComponent<Text> ().text = status.basicStatus.characterName.ToString ();
        firstCharacter.GetChild (1).GetChild (0).GetComponent<Text> ().text = status.characterLevel.ToString ();
        firstCharacter.GetChild (2).GetChild (0).GetComponent<Text> ().text = status.maxHp.ToString ();
        firstCharacter.GetChild (3).GetChild (0).GetComponent<Text> ().text = status.attack.ToString ();
        firstCharacter.GetChild (4).GetChild (0).GetComponent<Text> ().text = status.defense.ToString ();
        firstCharacter.GetChild (5).GetChild (0).GetComponent<Text> ().text = status.criRate.ToString ();
      }
    }
  }

  public void ChangingCharacter()
  {
    if (selectedToAdd != null) 
    {
      if (CheckingPartySlots() && selectedToChangeCharacter == null)
      {
        selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.isInParty = true;
        selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.partyOrdering = party.Count;
        TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().isInParty = true;
        TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().partyOrdering = party.Count;
        selectedToAdd = null;
      } 
      else 
      {
        if (selectedToChangeCharacter != null) 
        {
          selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.isInParty = true;
          selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.partyOrdering = selectedToChangeCharacter.GetComponent<AllCharacterData>().characterStatus.partyOrdering;
          TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().isInParty = true;
          TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().partyOrdering = selectedToAdd.GetComponent<AllCharacterData> ().characterStatus.partyOrdering;
          TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().isInParty = false;
          TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == selectedToChangeCharacter.GetComponent<AllCharacterData> ().characterStatus.basicStatus.ID).First ().partyOrdering = -1;
          selectedToAdd = null;
          selectedToChangeCharacter = null;
        }
      }
      GenerateParty ();
    }
  }

  private bool CheckingPartySlots()
  {
    for (int i = 0; i < slotsParty.Count; i++)
    {
      if (slotsParty [i].transform.childCount <= 0) 
      {
        return true;
      }
    }
    return false;
  }
}
