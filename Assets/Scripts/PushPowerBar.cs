using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushPowerBar : MonoBehaviour
{

    public Slider _slider;
    public Image _fill;
    public Gradient _gradient;
    

    public void SetValue(float value)
    {
        _slider.value = value;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);

      

    }
    public void SetMaxValue(float maxValue)
    {
        _slider.maxValue = maxValue;
      
        _fill.color = _gradient.Evaluate(1f);

    }
}