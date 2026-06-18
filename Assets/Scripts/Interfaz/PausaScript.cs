using UnityEngine;
using UnityEngine.SceneManagement;


public class PausaScript : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public void Pausa()
    {
        Time.timeScale= 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }
    public void Reanudar(){
        Time.timeScale= 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }
    public void Reiniciar(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Salir(){
        // Cambiamos la música inmediatamente al hacer clic
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMenuMusic();
        }
        // Luego cargamos la escena
        SceneManager.LoadScene("NivelesScene");
    }
}