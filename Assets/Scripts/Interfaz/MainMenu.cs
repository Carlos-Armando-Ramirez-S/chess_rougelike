using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
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
