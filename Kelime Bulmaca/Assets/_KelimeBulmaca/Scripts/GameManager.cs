using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public static bool isGameOn;

    [Header("Game Stage")]
    public GameStageEnums gameStage;

    [Header("Words Lists")]
    public int sessionTotalWordCount;
    public List<string> allWords = new List<string>();
    public List<string> selectedWords = new List<string>();

    [Header("Word Buttons")]
    public List<WordButton> wordButtonScripts = new List<WordButton>();
    public List<WordButton> availableWordButtonScripts = new List<WordButton>();
    public List<WordButton> notAvailableWordButtonScripts = new List<WordButton>();
    public Color[] wordButtonImageColors;

    [Header("Current Word")]
    public string currentWord;

    private UiControlManager uiControlManagerScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        AwakeStuff();
    }

    private void Start()
    {
        uiControlManagerScript = FindObjectOfType<UiControlManager>();

        for (int i = 0; i < wordButtonScripts.Count; i++)
        {
            availableWordButtonScripts.Add(wordButtonScripts[i]);
            wordButtonScripts[i].buttonWord = selectedWords[i];
            wordButtonScripts[i].buttonWordText.text = selectedWords[i];
            wordButtonScripts[i].wordButtonImage.color = wordButtonImageColors[i];
        }
    }

    private void AwakeStuff()
    {
        gameStage = GameStageEnums.GameStart;
        isGameOn = false;
        ShuffleAllWordsList();
        SelectedWordsListAssign();
        ShuffleWordButtonImageColors();
    }

    private void ShuffleAllWordsList()
    {
        var allWordsList = allWords;
        for (int i = 0; i < allWordsList.Count; i++)
        {
            string newWord = allWordsList[i];
            int r = Random.Range(i, allWordsList.Count);
            allWordsList[i] = allWordsList[r];
            allWordsList[r] = newWord;
        }
    }

    private void SelectedWordsListAssign()
    {
        for (int i = 0; i < sessionTotalWordCount; i++)
        {
            selectedWords.Add(allWords[i]);
        }
    }

    private void ShuffleWordButtonImageColors()
    {
        var wordButtonImageColorsArray = wordButtonImageColors;
        for (int i = 0; i < wordButtonImageColorsArray.Length; i++)
        {
            Color newColor = wordButtonImageColorsArray[i];
            int r = Random.Range(i, wordButtonImageColorsArray.Length);
            wordButtonImageColorsArray[i] = wordButtonImageColorsArray[r];
            wordButtonImageColorsArray[r] = newColor;
        }
    }

    public void SelectCurrentWord()
    {
        int randomNoForCurrentWord = Random.Range(0, availableWordButtonScripts.Count);
        currentWord = availableWordButtonScripts[randomNoForCurrentWord].buttonWord;
    }

    public bool ReturnWordMatch(string pressedButtonWord)
    {
        if (currentWord == pressedButtonWord)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckWordMatchAction(WordButton wordButtonScript)
    {
        gameStage = GameStageEnums.GameOngoingInputNotAvailable;

        availableWordButtonScripts.Remove(wordButtonScript);
        notAvailableWordButtonScripts.Add(wordButtonScript);
        selectedWords.Remove(currentWord);

        uiControlManagerScript.StopTimerCountdownCoroutine();
        uiControlManagerScript.timerAnimator.SetFloat("TimeControlMultiplier", 0f);
        uiControlManagerScript.timerAnimator.Play("TimeShowScaleDownAnimation", -1, 0f);
        uiControlManagerScript.tickImageAnimator.Play("TickImageScaleUpAnimation", -1, 0f);
        uiControlManagerScript.currentWordTextAnimator.Play("CurrentWordTextScaleDownAnimation", -1, 0f);

        uiControlManagerScript.SetKaristirEsGecButtonsInteractibility(false);

        if (selectedWords.Count == 0)
        {
            GameEndedAction(true);
        }
        else
        {
            SelectCurrentWord();

            StartCoroutine("ActivateInputAfterTrueMatch");
        }
    }

    private IEnumerator ActivateInputAfterTrueMatch()
    {
        yield return new WaitForSeconds(1f);
        uiControlManagerScript.currentWordText.text = currentWord;
        yield return new WaitForSeconds(1f);
        gameStage = GameStageEnums.GameOngoingInputAvailable;
        uiControlManagerScript.tickImageAnimator.Play("TickImageScaleDownAnimation", -1, 0f);
        uiControlManagerScript.StartTimerCountdownCoroutine();
        uiControlManagerScript.SetKaristirEsGecButtonsInteractibility(true);
    }

    public void GameEndedAction(bool isGameEndedSuccessfully)
    {
        if (isGameEndedSuccessfully)
        {
            GameManager.Instance.gameStage = GameStageEnums.GameFinish;

            uiControlManagerScript.muazzamTextAnimator.Play("MuazzamTextScaleUpAnimation", -1, 0f);
            Invoke("OpenSuccessPanel", 3f);
        }
        else
        {
            GameManager.Instance.gameStage = GameStageEnums.GameFinish;
            GameManager.isGameOn = false;

            uiControlManagerScript.zamanBittiTextAnimator.Play("ZamanBittiTextScaleUpAnimation", -1, 0f);
            uiControlManagerScript.esGecButton.interactable = false;
            uiControlManagerScript.karistirButton.interactable = false;
            Invoke("OpenFailPanel", 3f);
        }
    }

    private void OpenSuccessPanel()
    {
        uiControlManagerScript.tickImageAnimator.Play("TickImageScaleDownAnimation", -1, 0f);
        uiControlManagerScript.successPanelAnimator.Play("SuccessPanelAnimation", -1, 0f);
        AudioManager.Instance.successAudio.Play();
    }

    private void OpenFailPanel()
    {
        uiControlManagerScript.failPanelAnimator.Play("FailPanelAnimation", -1, 0f);
        AudioManager.Instance.failAudio.Play();

        uiControlManagerScript.timerAnimator.Play("TimeShowScaleDownAnimation", -1, 0f);
        uiControlManagerScript.selectedWordsOverlayAnimator.Play("SelectedWordsOverlayScaleDownAnimation", -1, 0f);
    }

    public void GameReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
    }
}