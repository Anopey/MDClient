using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle_Manager : MonoBehaviour
{
    #region Variables
    Song tolerance;
    float spawn;
    float multiplier;
    Song_Mapping indexButton;
    int button = 0;
    Vector2 sizeTemp;
    Vector2 initialSize;
    #endregion
    private void Start()
    {
        //spawn = indexButton.keyToHit[indexButton.indexArray].timeToHold; 
        spawn = 0.5f;
        //button = indexButton.keyToHit[indexButton.indexArray].index;
        button = 2;
        Debug.Log(button);
        #region Conditionals fow Spaw Size
        //0.9 0.7
        if ( button == 0)
        {
            initialSize.x = 0.9f + spawn;
            initialSize.y = 0.9f + spawn;
        }
        else if ( button == 1)
        {
            initialSize.x = 0.9f + spawn;
            initialSize.y = 0.9f + spawn;
        }
        else if (button == 2)
        {
            initialSize.x = 0.7f + spawn;
            initialSize.y = 0.7f + spawn;
        }
        else if (button == 3)
        {
            initialSize.x = 0.7f + spawn;
            initialSize.y = 0.7f + spawn;
        }
        #endregion
        gameObject.transform.localScale = initialSize;
        sizeTemp = transform.localScale;
    }
   private void Update()
    {
        #region Size Change
        sizeTemp = transform.localScale;
        sizeTemp.x -= Time.deltaTime;
        sizeTemp.y -= Time.deltaTime;
        transform.localScale = sizeTemp;
        #endregion

        #region Conditionals to Destroy the Object
        if ( transform.localScale.x <= 1.4f - spawn && (button == 0 || button == 1))
        {
            Destroy(gameObject);
        }
        else if( sizeTemp.y <= 1.2f - spawn && (button == 2|| button == 3))
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
