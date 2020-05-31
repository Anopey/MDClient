using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{

    private float currentSecond = 0;


    #region Drum Integration

    public float GetHoldTolerance()
    {
        return 0.5f;
    }

    public float GetStartHoldTolerance()
    {
        return GetHoldTolerance() * 2;
    }

    public float GetPerfectTolerance()
    {
        return 0.2f;
    }

    public float GetDrumPoint()
    {
        return 500f;
    }
    #endregion

    #region Tempo System for Online Play

    public float GetCurrentTempo()
    {
        return 1200;
    }

    #endregion

}
