using UnityEngine;
using UnityEngine.SceneManagement;


public class ComoJugarScript : MonoBehaviour
{
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
