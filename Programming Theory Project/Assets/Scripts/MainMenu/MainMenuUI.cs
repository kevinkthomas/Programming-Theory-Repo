using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(1000)]
public class MainMenuUI : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find("Continue Button").SetActive(GameManager.Instance.HasSaveGame);
    }

    public void NewGame()
    {
        GameManager.Instance.ResetGame();
        PlayGame();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        GameManager.Instance.SaveGame();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
}
