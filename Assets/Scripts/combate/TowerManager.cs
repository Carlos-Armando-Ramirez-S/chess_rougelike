using UnityEngine;
using System.Collections;

public class TowerManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform towerMesh;
    [SerializeField] private Transform boardRoot;

    [Header("Configuración de Daño")]
    [SerializeField] private float duracionHundimiento = 0.3f;

    [Header("Configuración de Caída")]
    [SerializeField] private float gravedad = 15f;
    [SerializeField] private float tiempoFlotacion = 0.2f;

    [Header("Configuración de Temblores")]
    [SerializeField] private float intensidadTemblorImpacto = 0.15f;
    [SerializeField] private float duracionTemblorImpacto = 0.2f;
    [SerializeField] private float factorTemblorCaida = 0.05f;

    // --- NUEVA VARIABLE PARA PARTÍCULAS ---
    [Header("Efectos Visuales")]
    [SerializeField] private GameObject prefabParticulasDerrumbe;
    // --------------------------------------

    private Coroutine rutinaActual;
    private float alturaActual;
    private float objetivoCaidaY;
    private Vector3 posicionBaseBoard;

    void Start()
    {
        alturaActual = towerMesh.localScale.y;
        objetivoCaidaY = boardRoot.position.y;
        posicionBaseBoard = boardRoot.position;
    }

    public float AlturaActual => alturaActual;

    public void AplicarDanio(float cantidad)
    {
        float alturaAnterior = alturaActual;
        alturaActual -= cantidad;
        if (alturaActual < 0) alturaActual = 0;

        objetivoCaidaY -= cantidad;

        // Animar la torre visual
        StartCoroutine(AnimarTorre(alturaAnterior, alturaActual));

        // --- NUEVO: INSTANCIAR PARTÍCULAS DE DERRUMBE ---
        if (prefabParticulasDerrumbe != null && cantidad > 0)
        {
            // Creamos el efecto en el centro del tablero
            GameObject efecto = Instantiate(prefabParticulasDerrumbe, boardRoot.position, Quaternion.identity);

            // Lo hacemos hijo del tablero para que se mueva con él
            efecto.transform.SetParent(boardRoot);

            // Opcional: Si quieres que la intensidad varíe con el daño,
            // podrías acceder al ParticleSystem aquí y cambiar el emission burst.

            // Destruir después de un tiempo
            Destroy(efecto, 3f);
        }
        // -----------------------------------------------

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        rutinaActual = StartCoroutine(SecuenciaDeEventos());
    }

    // ... (El resto de tus funciones: SecuenciaDeEventos, ShakeBoard, AnimarTorre se quedan IGUAL) ...

    private IEnumerator SecuenciaDeEventos()
    {
        // ... (código igual) ...
        yield return StartCoroutine(ShakeBoard(intensidadTemblorImpacto, duracionTemblorImpacto));
        yield return new WaitForSeconds(tiempoFlotacion);

        float distanciaCaida = posicionBaseBoard.y - objetivoCaidaY;

        if (distanciaCaida > 0.1f)
        {
            float velocidad = 0f;
            while (posicionBaseBoard.y > objetivoCaidaY)
            {
                velocidad += gravedad * Time.deltaTime;
                posicionBaseBoard.y -= velocidad * Time.deltaTime;
                boardRoot.position = posicionBaseBoard;
                yield return null;
            }
            posicionBaseBoard.y = objetivoCaidaY;
            boardRoot.position = posicionBaseBoard;
        }

        float intensidadAterrizaje = intensidadTemblorImpacto + (distanciaCaida * factorTemblorCaida);
        intensidadAterrizaje = Mathf.Min(intensidadAterrizaje, 1.0f);
        yield return StartCoroutine(ShakeBoard(intensidadAterrizaje, 0.25f));
    }

    private IEnumerator ShakeBoard(float intensidad, float duracion)
    {
        Vector3 posicionInicioTemblor = posicionBaseBoard;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensidad;
            float y = Random.Range(-1f, 1f) * intensidad * 0.5f;
            float z = Random.Range(-1f, 1f) * intensidad;
            boardRoot.position = posicionInicioTemblor + new Vector3(x, y, z);
            yield return null;
        }
        boardRoot.position = posicionInicioTemblor;
    }

    private IEnumerator AnimarTorre(float alturaInicial, float alturaFinal)
    {
        // ... (código igual) ...
        float tiempo = 0f;
        Vector3 escalaInicial = towerMesh.localScale;
        Vector3 posInicialMalla = towerMesh.localPosition;

        while (tiempo < duracionHundimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionHundimiento;
            t = t * t;
            float alturaInterpolada = Mathf.Lerp(alturaInicial, alturaFinal, t);
            Vector3 escala = towerMesh.localScale;
            escala.y = alturaInterpolada;
            towerMesh.localScale = escala;
            float diferencia = (alturaInicial - alturaInterpolada) / 2f;
            towerMesh.localPosition = posInicialMalla - new Vector3(0, diferencia, 0);
            yield return null;
        }
        Vector3 escalaFinal = towerMesh.localScale;
        escalaFinal.y = alturaFinal;
        towerMesh.localScale = escalaFinal;
    }
}