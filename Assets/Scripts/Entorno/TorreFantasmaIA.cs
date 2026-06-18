using UnityEngine;
using System.Collections;

public class TorreFantasmaIA : MonoBehaviour
{
    [Header("Referencias Internas")]
    public Transform meshTorre;
    public Transform tapa;

    [Header("Configuración Automática")]
    [Tooltip("Tiempo base entre colapsos. Se añadirá aleatoriedad.")]
    public float intervaloDanoBase = 5f;

    [Header("Configuración de Comportamiento")]
    [Tooltip("Cantidad base de daño. Se añadirá aleatoriedad.")]
    [SerializeField] private float cantidadDanoBase = 10f;
    [SerializeField] private float duracionHundimiento = 0.3f;
    [SerializeField] private float gravedadCaida = 15f;
    [SerializeField] private float tiempoFlotacion = 0.2f;

    [Header("Temblores")]
    [SerializeField] private float intensidadTemblor = 0.15f;
    [SerializeField] private float duracionTemblor = 0.2f;

    private float alturaActual;
    private float alturaMaxima;
    private float velocidadVerticalTapa = 0f;
    private Coroutine rutinaActual;

    void Start()
    {
        if (meshTorre == null) meshTorre = transform;

        alturaMaxima = meshTorre.localScale.y;
        alturaActual = alturaMaxima;

        // Iniciamos la rutina de vida
        StartCoroutine(RutinaDeVida());
    }

    IEnumerator RutinaDeVida()
    {
        // --- 1. DESINCRONIZACIÓN INICIAL ---
        // Esperamos un tiempo aleatorio al principio para que no todas empiecen a la vez.
        // Ejemplo: Si el intervalo es 5, esperan entre 0 y 5 segundos para empezar su primer ciclo.
        yield return new WaitForSeconds(Random.Range(0f, intervaloDanoBase));

        while (alturaActual > 1.0f)
        {
            // --- 2. TIEMPO ALEATORIO ENTRE EVENTOS ---
            // Calculamos cuánto esperaremos para el SIGUIENTE evento.
            // Hacemos que varíe entre el 50% y el 150% del tiempo base.
            float tiempoEspera = intervaloDanoBase * Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(tiempoEspera);

            // --- 3. DAÑO ALEATORIO ---
            // Calculamos cuánto se reducirá la torre en este evento.
            // Hacemos que varíe entre el 50% y el 150% de la cantidad base.
            float danoAplicar = cantidadDanoBase * Random.Range(0.5f, 1.5f);

            IniciarEventoColapso(danoAplicar);
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void IniciarEventoColapso(float cantidad)
    {
        float alturaAnterior = alturaActual;
        alturaActual -= cantidad;
        if (alturaActual < 0) alturaActual = 0;

        StartCoroutine(AnimarTorre(alturaAnterior, alturaActual));

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        rutinaActual = StartCoroutine(SecuenciaDeEventos());
    }

    private IEnumerator SecuenciaDeEventos()
    {
        yield return StartCoroutine(ShakeObject(intensidadTemblor, duracionTemblor));
        yield return new WaitForSeconds(tiempoFlotacion);

        if (tapa != null)
        {
            yield return StartCoroutine(CaidaTapa());
        }

        yield return StartCoroutine(ShakeObject(intensidadTemblor * 0.5f, 0.15f));
    }

    private IEnumerator AnimarTorre(float alturaIni, float alturaFin)
    {
        float tiempo = 0f;
        Vector3 escalaIni = meshTorre.localScale;
        Vector3 posBaseIni = meshTorre.localPosition;
        float diferenciaAltura = (alturaIni - alturaFin) / 2f;

        while (tiempo < duracionHundimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionHundimiento;
            t = t * t;

            float nuevaAltura = Mathf.Lerp(alturaIni, alturaFin, t);
            meshTorre.localScale = new Vector3(escalaIni.x, nuevaAltura, escalaIni.z);
            meshTorre.localPosition = posBaseIni - new Vector3(0, diferenciaAltura * t, 0);

            yield return null;
        }

        meshTorre.localScale = new Vector3(escalaIni.x, alturaFin, escalaIni.z);
        meshTorre.localPosition = posBaseIni - new Vector3(0, diferenciaAltura, 0);
    }

    private IEnumerator CaidaTapa()
    {
        velocidadVerticalTapa = 0f;

        while (tapa.localPosition.y > alturaActual)
        {
            velocidadVerticalTapa += gravedadCaida * Time.deltaTime;

            Vector3 pos = tapa.localPosition;
            pos.y -= velocidadVerticalTapa * Time.deltaTime;

            if (pos.y < alturaActual) pos.y = alturaActual;

            tapa.localPosition = pos;

            yield return null;
        }
    }

    private IEnumerator ShakeObject(float intensidad, float duracion)
    {
        Vector3 posicionInicio = transform.localPosition;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensidad;
            float z = Random.Range(-1f, 1f) * intensidad;

            transform.localPosition = posicionInicio + new Vector3(x, 0, z);
            yield return null;
        }

        transform.localPosition = posicionInicio;
    }
}