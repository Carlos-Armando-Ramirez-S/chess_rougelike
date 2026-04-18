using UnityEngine;

public class GestorMovimiento : MonoBehaviour
{
    [SerializeField] private GestorTablero gestorTablero;
    [SerializeField] private AnimacionPiezas animacionPiezas;
    [SerializeField] private GestorCombate gestorCombate;

    public void MoverPieza(AtributosPieza pieza, Vector2Int destino)
    {
        Vector2Int origen = pieza.posicionEnTablero;

        // --- NUEVA LÓGICA DE CAPTURA ---
        // 1. Intentar Captura
        AtributosPieza piezaEnDestino = gestorTablero.GetPiezaEn(destino);

        if (piezaEnDestino != null && piezaEnDestino.color != pieza.color)
        {
            if (gestorCombate != null)
            {
                // Recibimos la respuesta: ¿Pudimos comerla o tenía escudo?
                bool capturaExitosa = gestorCombate.ProcesarCaptura(pieza, piezaEnDestino);

                // SI FALLÓ (por escudo), cancelamos el movimiento y salimos.
                if (!capturaExitosa)
                {
                    Debug.Log("El ataque fue bloqueado, la pieza no se mueve.");
                    return;
                }
            }
        }
        // -------------------------------

        // 3. Actualizar registros lógicos
        gestorTablero.MoverRegistro(origen, destino);
        pieza.posicionEnTablero = destino;

        // --- NUEVO: MARCAR QUE LA PIEZA YA SE HA MOVIDO EN LA PARTIDA ---
        // Esto es vital para que el peón sepa que ya no puede mover 2 casillas en el futuro
        if (!pieza.yaSeMovioEnPartida)
        {
            pieza.yaSeMovioEnPartida = true;
        }
        // -------------------------------

        GameObject casillaDestino = gestorTablero.GetCasilla(destino);

        if (casillaDestino == null) return;

        // 4. VOLVEMOS LA PIEZA HIJA INMEDIATAMENTE
        pieza.transform.SetParent(casillaDestino.transform);

        // 5. CALCULAMOS EL DESTINO EN COORDENADAS LOCALES
        Renderer rendererCasilla = casillaDestino.GetComponentInChildren<Renderer>();

        float alturaSuperficieMundo = casillaDestino.transform.position.y;
        if (rendererCasilla != null) alturaSuperficieMundo = rendererCasilla.bounds.max.y;

        Renderer rendererPieza = pieza.GetComponentInChildren<Renderer>();
        float correccionPivot = 0f;
        if (rendererPieza != null)
        {
            correccionPivot = pieza.transform.position.y - rendererPieza.bounds.min.y;
        }

        // 6. CONVERTIMOS LA ALTURA MUNDIAL A ALTURA LOCAL
        float alturaLocalObjetivo = (alturaSuperficieMundo + correccionPivot) - casillaDestino.transform.position.y;
        Vector3 destinoLocal = new Vector3(0, alturaLocalObjetivo, 0);

        // 7. Animar usando coordenadas locales
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