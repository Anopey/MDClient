using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song_Mapping : MonoBehaviour
{
    #region variables
    //Struct to define an array that holds our key variables
    [Serializable]
    public struct expectedKey {
        public float timeToHit;
        public int index;
        public float timeToHold;
    };

    public expectedKey[] keyToHit;
    float timeCounter = 0;
    expectedKey dummy;
    float holdTolerancePoint;
    Song callMethod;
    int indexArray = 0;
    #endregion
    Dictionary<float, List<expectedKey>> secondToKeys = new Dictionary<float, List<expectedKey>>();
    private void Start()
    {
        holdTolerancePoint = callMethod.GetHoldTolerance();
        List<expectedKey> valueList = new List<expectedKey>();
        #region Bubble Sort Of keyToHit Array
        for (int i = 0; i < keyToHit.Length; i++)
        {
            for( int j = 0; j < keyToHit.Length; j++)
            {
                if( keyToHit[i].timeToHit < keyToHit[j].timeToHit)
                {
                    dummy = keyToHit[i];
                    keyToHit[i] = keyToHit[j];
                    keyToHit[j] = dummy;
                }
            }
        }
        #endregion
        #region Dictionary
        for ( int i = 0; i < keyToHit.Length; i++ )
        {
            
            if( !secondToKeys.ContainsKey(keyToHit[i].timeToHit))
            {
                valueList = new List<expectedKey>();
                valueList.Add(keyToHit[i]);
                secondToKeys.Add(keyToHit[i].timeToHit, valueList);
            }
            else
            {
                valueList.Add(keyToHit[i]);
                secondToKeys[keyToHit[i].timeToHit][i] = valueList[i];
            }
            
        }
        #endregion


    }
    private void Update()
    {
        timeCounter += Time.deltaTime;
        
        if( timeCounter >= keyToHit[indexArray].timeToHit )
        {

        }
    }


}
