using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FinishingGaugeManager : MonoBehaviour

{
  private static FinishingGaugeManager instance;
  
  public static FinishingGaugeManager GetInstance()
  {
    if (instance != null) return instance;
    return null;
  }
  
  private Slider slider;
  
  private void Awake()
  {
    instance = this;
    slider = this.transform.GetChild (1).GetComponent<Slider> ();
    slider.minValue = 0;
    slider.maxValue = 100;
  }
  
  public void ChangeSliderValue(float increase)
  {
    slider.value += increase;
  }
  
  public float GetSliderValue()
  {
    return slider.value;
  }
}
