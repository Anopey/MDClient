using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{



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

}
