using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private string opponentName;

    [SerializeField]
    private Text opponentText;
    [SerializeField]
    private GameObject loadObject;
    public bool songFunction = false;

    [SerializeField]
    private Slider mySlider, serverSlider, opponentSlider;

    #region Singleton Architecture

    private static GameManager singleton;
    private SongManager manager;

    private void Start()
    {
        if(singleton == null)
        {
            singleton = this;
            EnableInitializeGame();
            GameSceneLoaded?.Invoke();
            return;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(singleton == this)
        {
            singleton = null;
        }
    }

    public static GameManager GetSingleton()
    {
        return singleton;
    }

    private static Action GameSceneLoaded;
    private AudioSource song;

    public static void SubscribeGameSceneLoad(Action act)
    {
        GameSceneLoaded += act;
    }

    public static void UnSubscribeGameSceneLoad(Action act)
    {
        GameSceneLoaded -= act;
    }

    #endregion

    #region Server Commands

    public void EnableInitializeGame()
    {
        Debug.Log("The game shall now initialize by the requests of the server :O");
        loadObject.SetActive(false);
        song = GetComponent<AudioSource>();
        song.Play();
    }

   

    public void InitializeGameValues(string opponentName)
    {
        //set opponent name and display it
        this.opponentName = opponentName;
        opponentText.text = "Playing Against: " + opponentName;


    }

    public void UpdateServerPos(float val)
    {
        serverSlider.value = val;
    }

    public void UpdateOpponentPos(float val)
    {
        opponentSlider.value = val;
    }

    public void UpdateLocalPlayerPos(float val)
    {
        mySlider.value = val;
    }


    #endregion

    public void OnMySliderChange()
    {
        RequestPlayerPosUpdate(mySlider.value);
    }

    public void RequestPlayerPosUpdate(float val)
    {
        Client.UpdatePlayerPos(val);
    }

}
