using UnityEngine;

public class GeneradorTablerosFantasmas : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabTableroFantasma;
    public Transform contenedorFantasmas;
    public Transform centroTablero; // Tu tablero real

    [Header("Posiciones")]
    [Tooltip("Qué tan lejos aparecen (horizontalmente)")]
    public float radioMinimo = 80f; // Mucho más lejos
    public float radioMaximo = 200f;

    [Tooltip("Diferencia de altura")]
    public float offsetAlturaMin = -50f; // Pueden estar abajo
    public float offsetAlturaMax = 100f; // O arriba

    [Header("Comportamiento")]
    public float velocidadSubida = 10f; // Simulan que tú caes rápido
    public float tiempoEntreGeneracion = 5f;
    public int maxTableros = 10;

    private float timer;

    void Start()
    {
        if (contenedorFantasmas == null)
        {
            GameObject cont = new GameObject("ContenedorFantasmas");
            contenedorFantasmas = cont.transform;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tiempoEntreGeneracion)
        {
            if (contenedorFantasmas.childCount < maxTableros)
            {
                CrearTableroFantasma();
            }
            timer = 0;
        }
    }

    void CrearTableroFantasma()
    {
        if (centroTablero == null || prefabTableroFantasma == null) return;

        Vector3 posTablero = centroTablero.position;

        // 1. Posición en anillo lejano
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        float distancia = Random.Range(radioMinimo, radioMaximo);

        float posX = posTablero.x + dir2D.x * distancia;
        float posZ = posTablero.z + dir2D.y * distancia;
        float posY = posTablero.y + Random.Range(offsetAlturaMin, offsetAlturaMax);

        Vector3 spawnPos = new Vector3(posX, posY, posZ);

        // 2. Instanciar
        GameObject nuevoFantasma = Instantiate(prefabTableroFantasma, spawnPos, Quaternion.identity, contenedorFantasmas);

        // 3. Rotación Variada (CLAVE)
        // Rotamos en Y para que no miren al mismo lado
        float rotY = Random.Range(0, 360);
        // Añadimos una pequeña inclinación (X o Z) para que parezca que está flotando inestable
        float rotX = Random.Range(-5, 5);
        float rotZ = Random.Range(-5, 5);
        nuevoFantasma.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);

        // 4. Movimiento (Hacia ARRIBA)
        MovimientoSimple movimiento = nuevoFantasma.AddComponent<MovimientoSimple>();
        movimiento.Configurar(Vector3.up, velocidadSubida);

        Destroy(nuevoFantasma, 45f);
    }
}