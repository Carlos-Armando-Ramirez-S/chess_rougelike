using UnityEngine;
using TMPro;
using System.Collections; // IMPORTANTE: Necesario para usar IEnumerator (corrutinas)

public class TurnAnnouncer : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelFondo; // Arrastra aquí tu Panel (fondo negro)
    [SerializeField] private TMP_Text textoTurno;   // Arrastra aquí tu Texto

    [Header("Configuración de Tiempo")]
    [Tooltip("Cuántos segundos estará visible el aviso")]
    [SerializeField] private float tiempoVisible = 2.5f; // Puedes cambiar esto en el Inspector

    private Coroutine corrutinaActual;

    public void MostrarAviso(ColorPieza turno)
    {
        // 1. Definir el texto
        string nombreTurno = turno == ColorPieza.BLANCO ? "BLANCO" : "NEGRO";
        textoTurno.text = "Turno: " + nombreTurno;

        // 2. Activar el panel visualmente
        panelFondo.SetActive(true);

        // 3. Iniciar el temporizador para apagarlo
        if (corrutinaActual != null)
        {
            StopCoroutine(corrutinaActual); // Detenemos uno anterior si existe
        }

        corrutinaActual = StartCoroutine(OcultarDespuesDeTiempo());
    }

    private IEnumerator OcultarDespuesDeTiempo()
    {
        // Esperamos el tiempo definido en la variable
        yield return new WaitForSeconds(tiempoVisible);

        // Apagamos el panel
        panelFondo.SetActive(false);

        corrutinaActual = null;
    }
}