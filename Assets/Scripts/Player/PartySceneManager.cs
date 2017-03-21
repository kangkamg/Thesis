using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PartySceneManager : MonoBehaviour
{
  public GameObject changingPanel;
  public GameObject changeAbleCharacter;
  public GameObject partyPanel;
  public GameObject characterInParty;
  public GameObject changingCharacter;

  public List<CharacterStatus> party = new List<CharacterStatus> ();
  public List<CharacterStatus> characters = new List<CharacterStatus> ();
  public List<GameObject> slotsParty = new List<GameObject> ();
  public List<GameObject> slotsCharacter = new List<GameObject> ();
  public List<GameObject> charactersInSlots = new List<GameObject> ();

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
    foreach (GameObject a in slotsCharacter)
    {
      Destroy (a);
    }
    slotsParty.Clear ();
    slotsCharacter.Clear ();
    party.Clear ();
    characters.Clear ();

    for (int i = 0; i < TemporaryData.GetInstance().playerData.characters.Count; i++)
    {
      if (TemporaryData.GetInstance ().playerData.characters [i].isInParty) 
      {
        party.Add (TemporaryData.GetInstance ().playerData.characters [i]);
      }
      characters.Add (TemporaryData.GetInstance ().playerData.characters [i]);
      GameObject otherObj = Instantiate (changeAbleCharacter);
      otherObj.transform.SetParent (changingPanel.transform);
      otherObj.GetComponentInChildren<OtherCharacterData> ().characterData.status = TemporaryData.GetInstance ().playerData.characters [i];
      otherObj.GetComponentInChildren<Button>().onClick.AddListener(()=>SelectedChanging(otherObj.GetComponentInChildren<OtherCharacterData>().characterData.status));

      slotsCharacter.Add (otherObj);
    }

    party.Sort (delegate(CharacterStatus a, CharacterStatus b) 
      {
        return (a.partyOrdering.CompareTo(b.partyOrdering));
      });
    
    for (int i = 0; i < party.Count; i++)
    {
      GameObject partyObj = Instantiate (characterInParty);
      partyObj.transform.SetParent (partyPanel.transform);
      partyObj.GetComponent<PartyCharacterData> ().characterData.status = party [i];
      partyObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=>GoToChangingParty(partyObj.GetComponent<PartyCharacterData>().characterData.status));

      slotsParty.Add (partyObj);
    }
  }

  public void GoToChangingParty(CharacterStatus selectedCharacter)
  {
    TemporaryData.GetInstance ().selectedCharacter = selectedCharacter;

    partyPanel.SetActive (false);
    changingPanel.transform.parent.gameObject.SetActive (true);
    changingCharacter.GetComponent<ChangeCharacterParty> ().UpdateStatus ();
  }

  public void SelectedChanging(CharacterStatus selectedCharacter)
  {
    CharacterStatus changedCharacter = TemporaryData.GetInstance ().selectedCharacter;

    TemporaryData.GetInstance ().selectedCharacter = selectedCharacter;

    changingCharacter.GetComponent<ChangeCharacterParty> ().UpdateStatus ();

    TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == changedCharacter.basicStatus.ID).First ().isInParty = false;
    TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == TemporaryData.GetInstance ().selectedCharacter.basicStatus.ID).First ().isInParty = true;
    TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == TemporaryData.GetInstance ().selectedCharacter.basicStatus.ID).First ().partyOrdering = changedCharacter.partyOrdering;
    TemporaryData.GetInstance ().playerData.characters.Where (x => x.basicStatus.ID == changedCharacter.basicStatus.ID).First ().partyOrdering = -1;

    GenerateParty ();
  }
}
