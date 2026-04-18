using UnityEngine;

public class GestorReyes : MonoBehaviour
{
    private int reyesBlancos = 0;
    private int reyesNegros = 0;

    public void RegistrarRey(AtributosPieza pieza)
    {
        if (pieza.tipo != TipoPieza.Rey)
            return;

        if (pieza.color == ColorPieza.BLANCO)
            reyesBlancos++;
        else
            reyesNegros++;
    }

    public void ReyCapturado(AtributosPieza rey)
    {
        // Al capturar al rey, finalizamos la partida.
        // El perdedor es el color del rey que acaba de ser capturado.
        GameManager.instance.FinalizarPartida(rey.color);
    }

    public void EliminarRey(AtributosPieza pieza)
    {
        if (pieza.tipo != TipoPieza.Rey)
            return;

        if (pieza.color == ColorPieza.BLANCO)
            reyesBlancos--;
        else
            reyesNegros--;
    }

    public bool ReyBlancoVivo()
    {
        return reyesBlancos > 0;
    }

    public bool ReyNegroVivo()
    {
        return reyesNegros > 0;
    }
}