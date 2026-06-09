using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [Header("Fuentes de Audio")]
    // Usaremos dos AudioSources separados para poder bajar la música y mantener los efectos sonando
    public AudioSource musicSource; 
    public AudioSource sfxSource;   
    public AudioClip buttonClick;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Cargar volúmenes guardados (de 0 a 10)
        int volumenMusicaGuardado = PlayerPrefs.GetInt("volumenMusica", 5);
        int volumenSfxGuardado = PlayerPrefs.GetInt("volumenSFX", 5);

        SetMusicVolume(volumenMusicaGuardado);
        SetSFXVolume(volumenSfxGuardado);
    }

    // --- MÉTODOS DE MÚSICA ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(int volume)
    {
        float volumenFloat = (float)volume / 10.0f;
        musicSource.volume = volumenFloat;
        PlayerPrefs.SetInt("volumenMusica", volume);
    }

    // --- MÉTODOS DE EFECTOS DE SONIDO (SFX) ---
    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot permite reproducir un sonido sin interrumpir el que ya está sonando
        sfxSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(int volume)
    {
        float volumenFloat = (float)volume / 10.0f;
        sfxSource.volume = volumenFloat;
        PlayerPrefs.SetInt("volumenSFX", volume);
    }

    public void PlayButtonClick()
    {
        Debug.Log("Botón presionado");

        if (buttonClick == null)
        {
            Debug.LogError("buttonClick es NULL");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogError("sfxSource es NULL");
            return;
        }

        sfxSource.PlayOneShot(buttonClick);
    }
    
}