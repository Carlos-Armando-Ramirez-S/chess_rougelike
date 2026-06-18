using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class OpcionesScript : MonoBehaviour
{
    public Toggle toggle;
    public Slider volumenSlider;
    public Slider sfxSlider;

    public TMP_Dropdown resolucionesDropdown;
    private Resolution[] resoluciones;

    void Start()
    {
        volumenSlider.value = PlayerPrefs.GetInt("volumenMusica", 1);
        sfxSlider.value = PlayerPrefs.GetInt("volumenSFX", 5);
        ChangeMusicSlider();
        ChangeSFXSlider();

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
public void ChangeMusicSlider()
{
    int valor = (int)volumenSlider.value;

    Debug.Log("Guardando volumen música: " + valor);

    PlayerPrefs.SetInt("volumenMusica", valor);

    if (SoundManager.instance != null)
    {
        SoundManager.instance.SetMusicVolume(valor);
    }
}
    public void ChangeSFXSlider()
{
    int valor = (int)sfxSlider.value;

    Debug.Log("Guardando volumen SFX: " + valor);

    if (SoundManager.instance != null)
    {
        SoundManager.instance.SetSFXVolume(valor);
    }
}
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
}