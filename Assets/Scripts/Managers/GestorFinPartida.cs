using UnityEngine;

public class GestorFinPartida : MonoBehaviour
{
    private bool juegoTerminado = false;

    public void VerificarFin(int reyesBlancos, int reyesNegros)
    {
        if (juegoTerminado)
            return;

        if (reyesBlancos <= 0)
        {
            TerminarJuego("Negras");
        }

        if (reyesNegros <= 0)
        {
            TerminarJuego("Blancas");
        }
    }

    void TerminarJuego(string ganador)
    {
        juegoTerminado = true;

        Debug.Log("Ganador: " + ganador);
    }
}