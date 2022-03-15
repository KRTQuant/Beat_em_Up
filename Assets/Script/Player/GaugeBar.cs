using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxGauge(float gauge)
    {
        slider.maxValue = gauge;
        slider.value = gauge;
    }

    public void SetGauge(float gauge)
    {
        slider.value = gauge;
    }
}
