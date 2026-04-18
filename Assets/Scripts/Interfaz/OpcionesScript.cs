using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class OpcionesScript : MonoBehaviour
{
    public Slider slider;
    // La variable sliderValue ya no es necesaria si usas el valor del slider directamente.
    public Toggle toggle;
    public Slider volumenSlider;

    public TMP_Dropdown resolucionesDropdown;
    private Resolution[] resoluciones;

    void Start()
    {
        // Cargamos el volumen usando la MISMA clave que MusicManager
        slider.value = PlayerPrefs.GetInt("volumenMusica", 1);

        // Llamamos al método ChangeSlider para que aplique el volumen inicial
        ChangeSlider();

        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        RevisarResolucion();
    }

    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropdown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }
        resolucionesDropdown.AddOptions(opciones);
        resolucionesDropdown.value = resolucionActual;
        resolucionesDropdown.RefreshShownValue();

        // Cargar la resolución guardada
        resolucionesDropdown.value = PlayerPrefs.GetInt("numeroResolucion", 0);
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        PlayerPrefs.SetInt("numeroResolucion", indiceResolucion);
        Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }

    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    public void ChangeSlider() 
    {

        // Obtenemos el valor directamente de nuestra referencia al slider
        int valor = (int)volumenSlider.value;

        PlayerPrefs.SetInt("volumenMusica", valor);

        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetVolume(valor);
        }
        else
        {
            Debug.LogWarning("MusicManager no encontrado.");
        }
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
}