using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ErrorScene : MonoBehaviour
{

    private static string errorText;

    [SerializeField]
    private Text errorUIText;

    void Start()
    {
        errorUIText.text = errorText;
    }

    public static void LoadError(string text)
    {
        errorText = text;
        Debug.LogError(text);
        SceneManager.LoadScene("ErrorScene");
    }

    public void SwitchSceneButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
