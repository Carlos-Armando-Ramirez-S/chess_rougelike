using UnityEngine;

public class GeneradorNubes : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabNube;
    public float radioGeneracion = 30f;

    [Tooltip("Altura relativa al tablero. Ej: -5 significa 5 unidades debajo del tablero.")]
    public float offsetAlturaMinima = -5f;

    [Tooltip("Altura relativa al tablero. Ej: 30 significa 30 unidades por encima.")]
    public float offsetAlturaMaxima = 30f;

    public float tiempoEntreNubes = 4f;
    public float velocidadNube = 3f;

    [Header("Límites")]
    [Tooltip("Máximo de nubes que pueden existir al mismo tiempo.")]
    public int maxNubesActivas = 50;

    [Header("Referencias")]
    public Transform centroTablero;
    public Transform contenedorNubes;

    private float timer;

    void Start()
    {
        if (centroTablero == null)
        {
            GameObject temp = new GameObject("CentroTemporal");
            temp.transform.position = Vector3.zero;
            centroTablero = temp.transform;
        }

        if (contenedorNubes == null)
        {
            GameObject contenedor = new GameObject("ContenedorNubes");
            contenedorNubes = contenedor.transform;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tiempoEntreNubes)
        {
            timer = 0;
            CrearNube();
        }
    }

    void CrearNube()
    {
        // Verificar límite
        if (contenedorNubes.childCount >= maxNubesActivas)
        {
            return;
        }

        Vector3 posTablero = centroTablero.position;

        // 1. Posición relativa
        Vector2 puntoAleatorio = Random.insideUnitCircle.normalized * radioGeneracion;

        float posX = posTablero.x + puntoAleatorio.x;
        float posZ = posTablero.z + puntoAleatorio.y;
        float posY = posTablero.y + Random.Range(offsetAlturaMinima, offsetAlturaMaxima);

        Vector3 posicionSpawn = new Vector3(posX, posY, posZ);

        // 2. Instanciar
        GameObject nuevaNube = Instantiate(prefabNube, posicionSpawn, Quaternion.identity, contenedorNubes);

        // 3. Dirección segura
        Vector3 direccionAlCentro = (posTablero - posicionSpawn).normalized;
        Vector3 direccionAleatoria = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        if (Vector3.Dot(direccionAleatoria, direccionAlCentro) > 0)
        {
            direccionAleatoria = -direccionAleatoria;
        }

        // 4. Velocidad
        Rigidbody rb = nuevaNube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direccionAleatoria * velocidadNube;
        }
        else
        {
            MovimientoSimple ms = nuevaNube.GetComponent<MovimientoSimple>();
            if (ms == null) ms = nuevaNube.AddComponent<MovimientoSimple>();
            // CORRECCIÓN: Pasamos velocidadNube (float) correctamente
            ms.Configurar(direccionAleatoria, velocidadNube);
        }

        Destroy(nuevaNube, 25f);
    }
}

// Clase auxiliar corregida
public class MovimientoSimple : MonoBehaviour
{
    private Vector3 direccion;
    private float velocidad;

    // CORRECCIÓN: El segundo parámetro debe ser 'float', no 'Vector3'
    public void Configurar(Vector3 dir, float vel)
    {
        direccion = dir;
        velocidad = vel;
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
    }
}