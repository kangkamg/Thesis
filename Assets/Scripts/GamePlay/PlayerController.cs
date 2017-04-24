using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  Vector3 selectedPosition;
  public GameObject selectedSlots;
  GameObject selectedTile;
  
  public void SetUpSelectedPosition()
  {
    RemoveSelected ();
    selectedPosition = GameManager.GetInstance ().selectedCharacter.gridPosition;
    selectedTile = Instantiate (selectedSlots, Vector3.zero, Quaternion.Euler(new Vector3(90,0,0)))as GameObject;
    selectedTile.name = "selectedTileController";
    selectedTile.transform.position = GameManager.GetInstance ().map [(int)selectedPosition.x] [(int)selectedPosition.z].transform.position + (0.53f * Vector3.up);
    CameraManager.GetInstance ().MoveCameraToTarget (selectedTile.transform,5);
  }
  
  public void RemoveSelected()
  {
    Destroy (selectedTile);
  }
  
  public void SelectedTile()
  {
    if(selectedPosition != GameManager.GetInstance ().selectedCharacter.gridPosition)
      GameManager.GetInstance ().CheckingSelectedTile (selectedPosition);
  }
  
  public void MovePosition(int Direction)
  {
    switch (Direction)
    {
      case 0:
      if(selectedPosition.x-1>=0)
        selectedPosition.x -= 1;
      break;
      case 1:
      if(selectedPosition.z+1<GameManager.GetInstance ().map [(int)selectedPosition.x].Count)
        selectedPosition.z += 1;
      break;
      case 2:
      if(selectedPosition.z-1>=0)
        selectedPosition.z -= 1;
      break;
      case 3:
      if(selectedPosition.x+1<GameManager.GetInstance ().map.Count)
        selectedPosition.x += 1;
      break;
    }
    if (selectedPosition.z >= 0 && selectedPosition.x >= 0 && selectedPosition.z < GameManager.GetInstance ().map [(int)selectedPosition.x].Count && selectedPosition.x < GameManager.GetInstance ().map.Count)
      selectedTile.transform.position = GameManager.GetInstance ().map [(int)selectedPosition.x] [(int)selectedPosition.z].transform.position + (0.53f * Vector3.up);
    CameraManager.GetInstance ().MoveCameraToTarget (selectedTile.transform,5);
  }
}
