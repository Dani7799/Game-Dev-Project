using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int currentLevelIndex;

    internal bool pause;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1.0f;

        if (SceneManager.GetActiveScene().name == "Preloader")
        {
            PlayerPrefs.SetInt("loadedFromPreloader", 1);

            Debug.Log("YOS");
            SceneManager.LoadScene(PlayerPrefs.GetInt("totalLevelsUnlocked") + 1);
        }



        Debug.Log("total Unlocked: " + PlayerPrefs.GetInt("totalLevelsUnlocked"));
    }


    private void OnEnable()
    {
        PlayerCollision.boatReached += LevelEnd;
        UIManager.restart += ReloadLevel;
        UIManager.pause += PauseGame;
        UIManager.unpause += UnpauseGame;

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }


    void LevelEnd()
    {
        int nextLevel = currentLevelIndex + 1;
        nextLevel %= SceneManager.sceneCountInBuildSettings;
        if (nextLevel == 0)
        {
            nextLevel = 1;
        }

        AudioManager.instance.Play("BoatReached");
        AudioManager.instance.Play("Woo");

        if(PlayerPrefs.GetInt("totalLevelsUnlocked") < SceneManager.sceneCountInBuildSettings - 2)
        {
            if(SceneManager.GetActiveScene().buildIndex - 1 == PlayerPrefs.GetInt("totalLevelsUnlocked"))
            {
                PlayerPrefs.SetInt("totalLevelsUnlocked", PlayerPrefs.GetInt("totalLevelsUnlocked") + 1);
            }
        }

        StartCoroutine(LoadLevel(nextLevel));
    }

    void PauseGame()
    {
        pause = true;
        Time.timeScale = 0;
    }
    void UnpauseGame()
    {
        pause = false;
        Time.timeScale = 1;
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Load New Level: "+sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }



    private void OnDisable()
    {
        PlayerCollision.boatReached -= LevelEnd;
        UIManager.restart -= ReloadLevel;
        UIManager.pause -= PauseGame;
        UIManager.unpause -= UnpauseGame;
    }
}
