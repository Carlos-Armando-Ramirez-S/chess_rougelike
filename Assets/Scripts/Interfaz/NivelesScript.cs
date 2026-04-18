using UnityEngine;
using UnityEngine.SceneManagement;


public class NivelesScript : MonoBehaviour
{
    public void OpenLevelOne()
    {
        SceneManager.LoadScene("TableroScene");
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
