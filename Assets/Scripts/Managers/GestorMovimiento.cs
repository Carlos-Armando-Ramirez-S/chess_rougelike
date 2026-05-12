using UnityEngine;
using System.Collections; // <--- NECESARIO para IEnumerator

public class GestorMovimiento : MonoBehaviour
{
    [SerializeField] private GestorTablero gestorTablero;
    [SerializeField] private AnimacionPiezas animacionPiezas;
    [SerializeField] private GestorCombate gestorCombate;

    public IEnumerator MoverPieza(AtributosPieza pieza, Vector2Int destino)
    {
        Vector2Int origen = pieza.posicionEnTablero;

        // --- LÓGICA DE CAPTURA CON DADO ---
        AtributosPieza piezaEnDestino = gestorTablero.GetPiezaEn(destino);

        if (piezaEnDestino != null && piezaEnDestino.color != pieza.color)
        {
            if (gestorCombate != null)
            {
                bool capturaExitosa = true;

                yield return StartCoroutine(gestorCombate.ProcesarCaptura(pieza, piezaEnDestino, (res) =>
                {
                    capturaExitosa = res;
                }));

                if (!capturaExitosa)
                {
                    Debug.Log("El ataque falló (Dado 1). La pieza no se mueve.");
                    yield break;
                }
            }
        }
        // -------------------------------

        // --- MOVER PIEZA ---
        gestorTablero.MoverRegistro(origen, destino);
        pieza.posicionEnTablero = destino;

        if (!pieza.yaSeMovioEnPartida)
        {
            pieza.yaSeMovioEnPartida = true;
        }

        GameObject casillaDestino = gestorTablero.GetCasilla(destino);
        if (casillaDestino == null) yield break;

        pieza.transform.SetParent(casillaDestino.transform);

        Renderer rendererCasilla = casillaDestino.GetComponentInChildren<Renderer>();
        float alturaSuperficieMundo = casillaDestino.transform.position.y;
        if (rendererCasilla != null) alturaSuperficieMundo = rendererCasilla.bounds.max.y;

        Renderer rendererPieza = pieza.GetComponentInChildren<Renderer>();
        float correccionPivot = 0f;
        if (rendererPieza != null)
        {
            // CORRECCIÓN: Aquí decía 'piece', ahora dice 'pieza'
            correccionPivot = pieza.transform.position.y - rendererPieza.bounds.min.y;
        }

        float alturaLocalObjetivo = (alturaSuperficieMundo + correccionPivot) - casillaDestino.transform.position.y;
        Vector3 destinoLocal = new Vector3(0, alturaLocalObjetivo, 0);

        if (animacionPiezas != null)
        {
            animacionPiezas.MoverPiezaLocal(pieza.transform, destinoLocal);
        }
        else
        {
            pieza.transform.localPosition = destinoLocal;
        }
    }
}