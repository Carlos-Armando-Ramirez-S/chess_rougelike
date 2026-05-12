using UnityEngine;

public class GeneradorTablerosFantasmas : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabTableroFantasma;
    public Transform contenedorFantasmas;
    public Transform centroTablero;

    public float radioMinimo = 80f;
    public float radioMaximo = 200f;
    public float alturaMinima = 50f;
    public float alturaMaxima = 150f;

    public float tiempoEntreGeneracion = 10f;
    public int maxTableros = 10;

    [Header("Separación")]
    [Tooltip("Distancia mínima que debe haber entre una torre y otra")]
    public float distanciaMinima = 15f;
    [Tooltip("Cuántos intentos hace para encontrar un lugar libre antes de rendirse")]
    public int intentosMaximos = 10;

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
                CrearTablero();
            }
            timer = 0;
        }
    }

    void CrearTablero()
    {
        if (centroTablero == null || prefabTableroFantasma == null) return;

        Vector3 posicionValida = Vector3.zero;
        bool encontroSitio = false;

        // Intentamos encontrar un sitio libre 'intentosMaximos' veces
        for (int i = 0; i < intentosMaximos; i++)
        {
            // 1. Generamos una posición aleatoria
            Vector2 dir2D = Random.insideUnitCircle.normalized;
            float distancia = Random.Range(radioMinimo, radioMaximo);

            float posX = centroTablero.position.x + dir2D.x * distancia;
            float posZ = centroTablero.position.z + dir2D.y * distancia;
            float posY = centroTablero.position.y + Random.Range(alturaMinima, alturaMaxima);

            Vector3 posicionCandidata = new Vector3(posX, posY, posZ);

            // 2. Verificamos si está muy cerca de otras torres
            if (EsPosicionValida(posicionCandidata))
            {
                posicionValida = posicionCandidata;
                encontroSitio = true;
                break; // ¡Encontramos sitio! Salimos del bucle
            }
        }

        // 3. Si encontramos un sitio bueno, instanciamos
        if (encontroSitio)
        {
            Instantiate(prefabTableroFantasma, posicionValida, Quaternion.identity, contenedorFantasmas);
        }
        else
        {
            // Opcional: Podrías imprimir un mensaje si no encontró sitio
            Debug.Log("No se encontró espacio libre para la torre fantasma en este intento.");
        }
    }

    // Función que revisa si la posición está lejos de todas las torres existentes
    bool EsPosicionValida(Vector3 nuevaPosicion)
    {
        foreach (Transform hijo in contenedorFantasmas)
        {
            // Ignoramos diferencia de altura (eje Y) para el cálculo, 
            // así solo comparamos la distancia en el suelo (X y Z).
            Vector3 posExistente = new Vector3(hijo.position.x, nuevaPosicion.y, hijo.position.z);

            float distancia = Vector3.Distance(nuevaPosicion, posExistente);

            if (distancia < distanciaMinima)
            {
                return false; // Está muy cerca de alguien
            }
        }
        return true; // Está libre
    }
}