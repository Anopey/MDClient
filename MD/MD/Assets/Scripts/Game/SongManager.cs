using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{

    //TODO LATER ON MAKE THIS MORE ADVANCED FOR MULTIPLE SONG SELECTIONS!

    [SerializeField]
    private Song activeSong;
    public AudioSource song;

    

    #region Singleton Architecture

    private static SongManager singleton;

    private void Start()
    {
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            ProcessDefaultSong();
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

    public static SongManager GetSingleton()
    {
        return singleton;
    }

    #endregion
    
    private void ProcessDefaultSong()
    {
        DontDestroyOnLoad(activeSong);
    }

    public Song GetActiveSong()
    {

        return activeSong;
    }


}
