using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Cargamos el volumen guardado (un entero de 0 a 10)
        int volumenGuardado = PlayerPrefs.GetInt("volumenMusica", 1); // Valor por defecto 5

        // Llamamos a nuestro método SetVolume para aplicar el valor
        SetVolume(volumenGuardado);
    }

    // Ahora este método espera un entero y hace la conversión a float
    public void SetVolume(int volume)
    {
        // Convertimos el rango del slider (0-10) al rango del volumen (0.0 - 1.0)
        float volumenFloat = (float)volume / 10.0f;
        audioSource.volume = volumenFloat;

        // Guardamos el valor entero (el del slider) en PlayerPrefs
        PlayerPrefs.SetInt("volumenMusica", volume);
    }
}