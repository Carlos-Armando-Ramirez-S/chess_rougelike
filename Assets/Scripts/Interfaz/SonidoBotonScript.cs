using UnityEngine;

public class SonidoBotonScript : MonoBehaviour
{
     public void PlayClick()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClick();
        }
    }
}
