using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPress : MonoBehaviour
{
    public GameObject effect;
    public void PressButton()
    {
        Instantiate(effect, transform);
    }
    
}
