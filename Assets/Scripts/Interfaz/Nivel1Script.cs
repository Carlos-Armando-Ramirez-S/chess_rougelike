using UnityEngine;
using UnityEngine.SceneManagement;


public class Nivel1Script : MonoBehaviour
{
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}