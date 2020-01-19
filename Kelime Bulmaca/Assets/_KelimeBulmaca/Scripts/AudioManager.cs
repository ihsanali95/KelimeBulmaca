using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [Header("Audios")]
    public AudioSource successAudio;
    public AudioSource failAudio;
    public AudioSource rightClickAudio;
    public AudioSource wrongClickAudio;
    public AudioSource buttonsAudio;

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
    }
}