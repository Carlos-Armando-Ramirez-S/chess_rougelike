using UnityEngine;
using System.Collections;

public class AnimacionPiezas : MonoBehaviour
{
    [SerializeField] private float duracionMovimiento = 0.35f;
    [SerializeField] private float alturaSalto = 0.25f;

    private bool moviendo = false;

    public bool EstaMoviendo => moviendo;

    // Método antiguo (por si lo usas en otro lado)
    public void MoverPieza(Transform pieza, Vector3 destino)
    {
        StartCoroutine(AnimarMovimiento(pieza, destino));
    }

    // NUEVO MÉTODO PARA COORDENADAS LOCALES
    public void MoverPiezaLocal(Transform pieza, Vector3 destinoLocal)
    {
        StartCoroutine(AnimarMovimientoLocal(pieza, destinoLocal));
    }

    // Corrutina original (Mundo)
    private IEnumerator AnimarMovimiento(Transform pieza, Vector3 destino)
    {
        moviendo = true;
        float tiempo = 0f;
        Vector3 posicionInicial = pieza.position;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionMovimiento;
            Vector3 posicion = Vector3.Lerp(posicionInicial, destino, t);
            float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
            pieza.position = posicion + new Vector3(0, salto, 0);
            yield return null;
        }

        pieza.position = destino;
        moviendo = false;
    }

    // NUEVA CORRUTINA (Local)
    private IEnumerator AnimarMovimientoLocal(Transform pieza, Vector3 destinoLocal)
    {
        moviendo = true;
        float tiempo = 0f;
        Vector3 posicionInicial = pieza.localPosition; // Usamos localPosition

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionMovimiento;

            // Interpolamos en espacio local
            Vector3 posicion = Vector3.Lerp(posicionInicial, destinoLocal, t);

            // El salto también se suma en local
            float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;

            pieza.localPosition = posicion + new Vector3(0, salto, 0);

            yield return null;
        }

        pieza.localPosition = destinoLocal;
        moviendo = false;
    }
}