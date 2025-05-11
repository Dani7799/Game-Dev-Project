using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static event Action restart, pause, unpause;

    [SerializeField]
    private GameObject replayButton;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private GameObject levelEndCheck;

    [SerializeField]
    private float animMoveOffset;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private GameObject levelSelectContent;
    [SerializeField]
    private GameObject levelButton;

    [SerializeField]
    private GameObject EntryScreen;
    [SerializeField]
    private GameObject HUD;

    [SerializeField]
    Color currentLevelColor = Color.yellow;
    [SerializeField]
    Color completedLevelColor = Color.green;

    bool firstInput;

    private void OnEnable()
    {
        PlayerInput.swiped += EnableReplayButton;
        PlayerInput.swiped += DisableEntryScreen;
        PlayerCollision.boatReached += DisableReplayButton;

    }
    private void OnDisable()
    {
        PlayerInput.swiped -= EnableReplayButton;
        PlayerInput.swiped -= DisableEntryScreen;
        PlayerCollision.boatReached -= DisableReplayButton;

    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("loadedFromPreloader") == 0)
        {
            DisableEntryScreen();
        }
        else
        {
            PlayerPrefs.SetInt("loadedFromPreloader", 0);
        }

        firstInput = false;
        levelEndCheck.SetActive(false);
        replayButton.SetActive(false);
        pauseButton.SetActive(true);
        pauseButton.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0), 0.25f, 0, 1);
        levelText.text = "LEVEL " + (GameManager.currentLevelIndex);

        InstantiateLevelSelectButtons();
    }

    void DisableEntryScreen()
    {
        disableObject(EntryScreen);
        enableObject(HUD);
    }

    private void EnableReplayButton()
    {
        if(!firstInput)
        {
            firstInput = true;
            replayButton.SetActive(true);
            replayButton.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0), 0.25f, 0, 1);

            pauseButton.transform.DOLocalMoveX(pauseButton.transform.localPosition.x - animMoveOffset, 0.2f);
            replayButton.transform.DOLocalMoveX(replayButton.transform.localPosition.x + animMoveOffset, 0.2f);
        }
    }

    private void DisableReplayButton()
    {
        pauseButton.transform.DOLocalMoveX(pauseButton.transform.localPosition.x + animMoveOffset, 0.2f);
        replayButton.transform.DOLocalMoveX(replayButton.transform.localPosition.x - animMoveOffset, 0.2f);

        Invoke("EnableCheckMark", 0.25f);
    }

    private void EnableCheckMark()
    {
        replayButton.SetActive(false);
        pauseButton.SetActive(false);

        levelEndCheck.SetActive(true);
        levelEndCheck.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0), 0.25f, 0, 1);
    }

    public void PauseButtonClicked()
    {
        pause?.Invoke();
    }
    public void UnpauseButtonClicked()
    {
        unpause?.Invoke();
    }
    public void RestartButtonClicked()
    {
        restart?.Invoke();
    }

    public void enableObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void disableObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    private void InstantiateLevelSelectButtons()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 2; i++)
        {
            Debug.Log("button #" + i);
            Instantiate(levelButton, levelSelectContent.transform);
        }

        int it = 0;
        int levelsUnlocked = PlayerPrefs.GetInt("totalLevelsUnlocked");

        foreach (Transform t in levelSelectContent.transform)
        {
            int currentIndex = it;
            Button b = t.GetComponent<Button>();
            TextMeshProUGUI levelText = b.GetComponentInChildren<TextMeshProUGUI>();
            levelText.text = (currentIndex + 1).ToString();


            Image colorImg = t.GetComponent<Image>();
            //GameObject lockImg = t.GetChild(2).gameObject;

            b.onClick.AddListener(() => OnLevelSelectClicked(currentIndex + 1));


            if (it <= levelsUnlocked)
            {
                //lockImg.SetActive(false);
                if (it == SceneManager.GetActiveScene().buildIndex - 1)
                    colorImg.color = currentLevelColor;
                else
                    colorImg.color = completedLevelColor;
            }
            else
            {
                //levelText.gameObject.SetActive(false);
                b.interactable = false;
                colorImg.color = new Color(135f/255f, 135f/255f, 135f/255f, 1);
            }

            it++;

        }
    }

    public void OnLevelSelectClicked(int level)
    {
        Debug.Log(level);
        SceneManager.LoadScene(level);
    }
}
