using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // AÑADE ESTO: Referencia al GameController (o lo buscamos al inicio)
    private void Start()
    {
        // Nos aseguramos de que exista el GameController en la escena
        if (GameController.Instance == null)
        {
            GameObject go = new GameObject("GameController");
            go.AddComponent<GameController>();
        }
    }

    public void PlayGame()
    {
        // AÑADE ESTO: Activamos el modo CPU
        GameController.Instance.SetModoCPU(true);

        SceneManager.LoadScene("NivelesScene");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OpcionesScene");
    }

    public void OpenHowPlay()
    {
        SceneManager.LoadScene("ComoJugarScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}