using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UiControlManager : MonoBehaviour
{
    // // // // // // MAIN MENU PANEL // // // // //

    [Header("Background Image")]
    public Image backgroundImage;

    [Header("Oyna Button Animator")]
    public Animator oynaButtonAnimator;

    [Header("Kelime Bulmaca Text Animator")]
    public Animator kelimeBulmacaTextAnimator;

    // // // // // // IN MENU PANEL // // // // //

    [Header("Layout Overlay Rect Transform")]
    public RectTransform layoutOverlayRectTransform;

    [Header("Selected Words Overlay Animator")]
    public Animator selectedWordsOverlayAnimator;

    [Header("Current Word Text")]
    public TMP_Text currentWordText;
    public GameObject currentWordRadialShineImageObject;
    public Animator currentWordTextAnimator;

    [Header("Expression Text Animator")]
    public Animator expressionTextAnimator;

    [Header("Countdown Animator")]
    public Animator countdownTextAnimator;
    private TMP_Text countdownText;

    [Header("Timer Animator")]
    public Animator timerAnimator;
    public TMP_Text timerCurrentTimeText;

    [Header("Zaman Bitti Text Animator")]
    public Animator zamanBittiTextAnimator;

    [Header("Tick Image Animator")]
    public Animator tickImageAnimator;

    [Header("Muazzam Text Animator")]
    public Animator muazzamTextAnimator;

    // // // // // // FAIL PANEL // // // // //

    [Header("Fail Panel Animator")]
    public Animator failPanelAnimator;

    // // // // // // SUCCESS PANEL // // // // //

    [Header("Success Panel Animator")]
    public Animator successPanelAnimator;

    // // // // // // ES GEC - KARISTIR // // // // //

    [Header("Es Gec")]
    public GameObject esGecButtonObject;
    public Button esGecButton;
    private Animator esGecButtonAnimator;
    private bool isEsGecButtonUsedOnce;

    [Header("Karistir")]
    public GameObject karistirButtonObject;
    public Button karistirButton;
    private Animator karistirButtonAnimator;

    private void Awake()
    {
        AwakeStuff();
    }

    private void AwakeStuff()
    {
        isEsGecButtonUsedOnce = false;

        countdownText = countdownTextAnimator.gameObject.GetComponent<TMP_Text>();
        currentWordTextAnimator = currentWordText.gameObject.GetComponent<Animator>();

        esGecButton = esGecButtonObject.GetComponent<Button>();
        esGecButtonAnimator = esGecButtonObject.GetComponent<Animator>();

        karistirButton = karistirButtonObject.GetComponent<Button>();
        karistirButtonAnimator = karistirButtonObject.GetComponent<Animator>();
    }


    public void OynaButtonAction()
    {
        currentWordText.text = "? ? ? ? ?";
        oynaButtonAnimator.Play("OynaButtonScaleDownAnimation", -1, 0f);
        kelimeBulmacaTextAnimator.Play("KelimeBulmacaTextScaleDownAnimation", -1, 0f);
        selectedWordsOverlayAnimator.Play("SelectedWordsOverlayScaleUpAnimation", -1, 0f);
        StartCoroutine("ActivateGameAction");
    }

    private IEnumerator ActivateGameAction()
    {
        GameManager.Instance.SelectCurrentWord();
        GameManager.Instance.gameStage = GameStageEnums.GameOngoingInputNotAvailable;
        yield return new WaitForSeconds(3.5f);
        currentWordText.text = "";
        expressionTextAnimator.Play("ExpressionTextScaleDownAnimation", -1, 0f);
        yield return new WaitForSeconds(0.6f);
        countdownText.text = "3";
        countdownTextAnimator.Play("CountdownTextScaleDownAnimation", -1, 0f);
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "2";
        countdownTextAnimator.Play("CountdownTextScaleDownAnimation", -1, 0f);
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "1";
        countdownTextAnimator.Play("CountdownTextScaleDownAnimation", -1, 0f);
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "BAŞLA!";
        countdownTextAnimator.Play("CountdownTextScaleDownAnimation", -1, 0f);

        currentWordText.text = GameManager.Instance.currentWord;
        currentWordRadialShineImageObject.SetActive(true);
        currentWordTextAnimator.Play("CurrentWordTextScaleUpAnimation", -1, 0f);

        GameManager.Instance.gameStage = GameStageEnums.GameOngoingInputAvailable;
        GameManager.isGameOn = true;

        esGecButtonAnimator.Play("EsGecButtonScaleUpAnimation", -1, 0f);
        karistirButtonAnimator.Play("KaristirButtonScaleUpAnimation", -1, 0f);

        StartTimerCountdownCoroutine();
    }

    public void StartTimerCountdownCoroutine()
    {
        StartCoroutine("TimerCountdown");
    }

    public void StopTimerCountdownCoroutine()
    {
        StopCoroutine("TimerCountdown");
    }

    private IEnumerator TimerCountdown()
    {
        timerAnimator.Play("TimeShowScaleUpAnimation", -1, 0f);
        timerAnimator.Play("TimeStrokeAnimation", -1, 0f);
        timerAnimator.SetFloat("TimeControlMultiplier", 1f);

        currentWordTextAnimator.Play("CurrentWordTextScaleUpAnimation", -1, 0f);

        timerCurrentTimeText.text = "5";
        yield return new WaitForSeconds(1f);
        timerCurrentTimeText.text = "4";
        yield return new WaitForSeconds(1f);
        timerCurrentTimeText.text = "3";
        yield return new WaitForSeconds(1f);
        timerCurrentTimeText.text = "2";
        yield return new WaitForSeconds(1f);
        timerCurrentTimeText.text = "1";
        yield return new WaitForSeconds(1f);
        timerCurrentTimeText.text = "0";
    }

    public void EsGecButtonPressed()
    {
        if (!isEsGecButtonUsedOnce)
        {
            AudioManager.Instance.buttonsAudio.Play();
            isEsGecButtonUsedOnce = true;
            esGecButton.interactable = false;
            for (int i = 0; i < GameManager.Instance.availableWordButtonScripts.Count; i++)
            {
                if (GameManager.Instance.currentWord == GameManager.Instance.availableWordButtonScripts[i].buttonWord)
                {
                    GameManager.Instance.availableWordButtonScripts[i].MatchTrueAction();
                    break;
                }
            }
        }
    }

    public void KaristirButton()
    {
        AudioManager.Instance.buttonsAudio.Play();

        var buttonScripts = GameManager.Instance.wordButtonScripts;
        for (int i = 0; i < buttonScripts.Count; i++)
        {
            WordButton newScript = buttonScripts[i];
            int r = Random.Range(i, buttonScripts.Count);
            buttonScripts[i] = buttonScripts[r];
            buttonScripts[r] = newScript;
        }

        for (int i = 0; i < GameManager.Instance.wordButtonScripts.Count; i++)
        {
            GameManager.Instance.wordButtonScripts[i].transform.SetSiblingIndex(i);
            GameManager.Instance.wordButtonScripts[i].wordButtonAnimator.Play("WordImageButtonScaleUpAnimation", -1, 0f);
        }
    }

    public void SetKaristirEsGecButtonsInteractibility(bool isInteractable)
    {
        karistirButton.interactable = isInteractable;
        if (isEsGecButtonUsedOnce)
        {
            esGecButton.interactable = false;
        }
        else
        {
            esGecButton.interactable = isInteractable;
        }
    }
}