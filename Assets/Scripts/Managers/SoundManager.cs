using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [Header("Fuentes de Audio")]
    public AudioSource musicSource; 
    public AudioSource sfxSource;   
    
    [Header("Música del Menú")]
    public AudioClip musicaMenu; // Opcional, por si la necesitas

    [Header("Música del Nivel 1 (Ambientación)")]
    public AudioClip musicaTranquila;
    public AudioClip musicaFuerte;
    public AudioClip musicaCaos;

    [Header("SFX")]
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
        int volumenMusicaGuardado = PlayerPrefs.GetInt("volumenMusica", 5);
        int volumenSfxGuardado = PlayerPrefs.GetInt("volumenSFX", 5);

        SetMusicVolume(volumenMusicaGuardado);
        SetSFXVolume(volumenSfxGuardado);
    }

    // --- MÉTODOS DE MÚSICA ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // --- NUEVO: MÉTODO ESPECÍFICO PARA LAS FASES DEL NIVEL ---
    public void PlayLevelMusic(int fase)
    {
        AudioClip clipAReproducir = null;

        switch (fase)
        {
            case 0: clipAReproducir = musicaTranquila; break;
            case 1: clipAReproducir = musicaFuerte; break;
            case 2: clipAReproducir = musicaCaos; break;
        }

        // Si el clip es válido y no es el que ya está sonando, lo cambiamos
        if (clipAReproducir != null && musicSource.clip != clipAReproducir)
        {
            musicSource.clip = clipAReproducir;
            musicSource.Play();
        }
    }
    // ---METODO PARA QUE AL SALIR DE UN NIVEL DEL JUEGO SE CAMBIE A LA MUSICA DEL MENU
    public void PlayMenuMusic()
    {
        // Si la música del menú no está asignada, no hace nada
        if (musicaMenu == null) return;
        
        // Si ya está sonando la música del menú, no hace nada (evita que se reinicie)
        if (musicSource.clip == musicaMenu) return;

        musicSource.clip = musicaMenu;
        musicSource.Play();
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
        if (buttonClick == null || sfxSource == null) return;
        sfxSource.PlayOneShot(buttonClick);
    }
}