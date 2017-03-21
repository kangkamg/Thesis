using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusSceneManager : MonoBehaviour
{
  public GameObject otherCharacterPanel;
  public GameObject partyPanel;
  public GameObject partyCharacter;
  public GameObject otherCharacter;
  public GameObject changingItemObj;
  public GameObject skillObj;
  public GameObject selectEquipmentArrow;

  public Transform selectedItem;
  public Transform selectingArrow;

  public GameObject mainPage;
  public GameObject statusPage;
  public GameObject equipmentPage;

  public List<CharacterStatus> party = new List<CharacterStatus> ();
  public List<CharacterStatus> otherCharacters = new List<CharacterStatus> ();
  public List<GameObject> slotsParty = new List<GameObject> ();
  public List<GameObject> slotsOtherCharacter = new List<GameObject> ();

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
  }

  public void GenerateCharacter()
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

    for (int i = 0; i < TemporaryData.GetInstance().playerData.characters.Count; i++)
    {
      if (TemporaryData.GetInstance ().playerData.characters [i].isInParty) 
      {
        party.Add (TemporaryData.GetInstance ().playerData.characters [i]);
      }
      else
      {
        otherCharacters.Add (TemporaryData.GetInstance ().playerData.characters [i]);
        GameObject otherObj = Instantiate (otherCharacter);
        otherObj.transform.SetParent (otherCharacterPanel.transform);
        otherObj.GetComponentInChildren<OtherCharacterData> ().characterData.status = TemporaryData.GetInstance ().playerData.characters [i];
        otherObj.GetComponentInChildren<Button>().onClick.AddListener(()=>LookStatus(otherObj.GetComponentInChildren<OtherCharacterData>().characterData.status));

        slotsOtherCharacter.Add (otherObj);
      }
    }
    party.Sort (delegate(CharacterStatus a, CharacterStatus b) 
      {
        return (a.partyOrdering.CompareTo(b.partyOrdering));
      });
    
    for (int i = 0; i < party.Count; i++) 
    {
      GameObject partyObj = Instantiate (partyCharacter);
      partyObj.transform.SetParent (partyPanel.transform);
      partyObj.GetComponent<PartyCharacterData> ().characterData.status = party [i];
      partyObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=>LookStatus(partyObj.GetComponent<PartyCharacterData>().characterData.status));

      slotsParty.Add (partyObj);
    }
  }

  public void LookStatus(CharacterStatus selectedStatus)
  {
    TemporaryData.GetInstance ().selectedCharacter = selectedStatus;

    mainPage.SetActive (false);
    equipmentPage.SetActive (false);
    statusPage.SetActive (true);

    statusPage.GetComponent<ShowingCharacterStatusManager> ().UpdateStatus ();
  }
}
