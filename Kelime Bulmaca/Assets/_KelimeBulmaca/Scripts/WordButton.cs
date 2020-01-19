using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordButton : MonoBehaviour
{
    [Header("Word")]
    public string buttonWord;
    public TMP_Text buttonWordText;

    [Header("Word Button Booleans")]
    public bool isWordButtonAvailable;

    [Header("Button Image")]
    public Image wordButtonImage;

    public Animator wordButtonAnimator;

    private void Start()
    {
        wordButtonAnimator = GetComponent<Animator>();
        isWordButtonAvailable = true;
    }

    public void WordButtonAction()
    {
        if (GameManager.Instance.gameStage == GameStageEnums.GameOngoingInputAvailable && isWordButtonAvailable)
        {
            MatchTrueAction();
        }
    }

    public void MatchTrueAction()
    {
        bool isMatchTrue = GameManager.Instance.ReturnWordMatch(buttonWord);

        if (isMatchTrue)
        {
            isWordButtonAvailable = false;

            wordButtonAnimator.Play("WordImageButtonCloseAnimation", -1, 0f);

            GameManager.Instance.CheckWordMatchAction(this);

            AudioManager.Instance.rightClickAudio.Play();
        }
        else
        {
            AudioManager.Instance.wrongClickAudio.Play();
        }
    }
}