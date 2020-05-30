using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void Press(int whichPad)
    {
        Debug.Log("Pressed pad:" + whichPad);
    }
    public static int atWhichOne;
    public Slider[] slides;

    public float GetSliderPos(int whichSlider)
    {
        Debug.Log("Slider " + whichSlider + " is " + slides[whichSlider].value);
        return slides[whichSlider].value;
    }
    public void SetSliderPos(int whichSlider, float value)
    {
        Debug.Log("Slider " + whichSlider + " set to " + value);
        slides[whichSlider].value = value;
    }

    public void MyOwnSliderChanged()
    {
        Debug.Log("My slider moved");
    }

    public EffectsPress[] buttons;
    public void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            Press(0);
            buttons[0].PressButton();
        }
        if (Input.GetKeyDown("w"))
        {
            Press(1);
            buttons[1].PressButton();
        }
        if (Input.GetKeyDown("e"))
        {
            Press(2);
            buttons[2].PressButton();
        }
        if (Input.GetKeyDown("r"))
        {
            Press(3);
            buttons[3].PressButton();
        }
    }
}
