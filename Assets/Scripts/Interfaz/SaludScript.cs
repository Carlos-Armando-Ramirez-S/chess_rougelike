using UnityEngine;
using UnityEngine.UI;

public class SaludScript : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    public ColorPieza colorJugador;

    [Header("Interfaz de Usuario")]
    public Image BarraDeSalud;
    public Text TextoDeSalud;

    private int reyesMaximos = 0;
    private int reyesActuales = 0;

    /// <summary>
    /// El GameManager llama a este método para establecer el tope de vida.
    /// </summary>
    public void EstablecerReyesMaximos(int cantidad)
    {
        reyesMaximos = cantidad;
        Debug.Log("[" + colorJugador + "] SaludScript: Máximo de reyes establecido en " + cantidad);
    }

    /// <summary>
    /// El GameManager llama a este método cada vez que el número de reyes cambia.
    /// </summary>
    public void ActualizarVida(int cantidadReyesActuales)
    {
        reyesActuales = cantidadReyesActuales;
        Debug.Log("[" + colorJugador + "] SaludScript: Actualizando vida a " + reyesActuales + "/" + reyesMaximos);
        
        float porcentajeSalud = (reyesMaximos > 0) ? (float)reyesActuales / reyesMaximos : 0f;
        
        if (BarraDeSalud != null) {
            BarraDeSalud.fillAmount = porcentajeSalud;
        } else {
            Debug.LogError("[" + colorJugador + "] SaludScript: Error, el campo 'BarraDeSalud' no está asignado en el Inspector.");
        }

        if (TextoDeSalud != null) {
            TextoDeSalud.text = "Reyes: " + reyesActuales + " / " + reyesMaximos;
        } else {
            Debug.LogError("[" + colorJugador + "] SaludScript: Error, el campo 'TextoDeSalud' no está asignado en el Inspector.");
        }

        if (reyesActuales == 0 && reyesMaximos > 0)
        {
            Debug.Log("[" + colorJugador + "] SaludScript: ¡DERROTA DETECTADA! Avisando al GameManager.");
            if (GameManager.instance != null)
            {
                GameManager.instance.FinalizarPartida(colorJugador);
            }
        }
    }
}