using UnityEngine;
using System.Collections.Generic;

public class InicializadorPiezas : MonoBehaviour
{
    #region Singleton
    public static InicializadorPiezas instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        gestorTablero = FindFirstObjectByType<GestorTablero>();

        if (gestorTablero == null)
        {
            Debug.LogError("ERROR: No se pudo obtener la referencia del GestorTablero.");
            return;
        }
    }
    #endregion

    [Header("Prefabs de Piezas")]
    public GameObject peonPrefab, torrePrefab, caballoPrefab, alfilPrefab, reinaPrefab, reyPrefab;

    [Header("Materiales para las Piezas")]
    public Material materialBlanco, materialNegro;

    private Dictionary<TipoPieza, Mesh> mallaDePiezas = new Dictionary<TipoPieza, Mesh>();

    private GestorTablero gestorTablero;

    void Start()
    {
        /*gestorTablero = FindObjectOfType<GestorTablero>();

        if (gestorTablero == null)
        {
            Debug.LogError("ERROR: No se pudo obtener la referencia del GestorTablero.");
            return;
        }*/

        InicializarMallas();

        //Invoke(nameof(ColocarPiezas), 0.1f);
    }
    public void IniciarColocacion()
    {
        ColocarPiezas();
    }

    void InicializarMallas()
    {
        mallaDePiezas.Add(TipoPieza.Peon, peonPrefab.GetComponent<MeshFilter>().sharedMesh);
        mallaDePiezas.Add(TipoPieza.Torre, torrePrefab.GetComponent<MeshFilter>().sharedMesh);
        mallaDePiezas.Add(TipoPieza.Caballo, caballoPrefab.GetComponent<MeshFilter>().sharedMesh);
        mallaDePiezas.Add(TipoPieza.Alfil, alfilPrefab.GetComponent<MeshFilter>().sharedMesh);
        mallaDePiezas.Add(TipoPieza.Reina, reinaPrefab.GetComponent<MeshFilter>().sharedMesh);
        mallaDePiezas.Add(TipoPieza.Rey, reyPrefab.GetComponent<MeshFilter>().sharedMesh);
    }

    void ColocarPiezas()
    {
        for (int i = 0; i < 8; i++)
        {
            CrearPiezaEn(i, 1, peonPrefab, TipoPieza.Peon, ColorPieza.BLANCO);
            CrearPiezaEn(i, 6, peonPrefab, TipoPieza.Peon, ColorPieza.NEGRO);
        }

        CrearPiezaEn(0, 0, torrePrefab, TipoPieza.Torre, ColorPieza.BLANCO);
        CrearPiezaEn(7, 0, torrePrefab, TipoPieza.Torre, ColorPieza.BLANCO);
        CrearPiezaEn(1, 0, caballoPrefab, TipoPieza.Caballo, ColorPieza.BLANCO);
        CrearPiezaEn(6, 0, caballoPrefab, TipoPieza.Caballo, ColorPieza.BLANCO);
        CrearPiezaEn(2, 0, alfilPrefab, TipoPieza.Alfil, ColorPieza.BLANCO);
        CrearPiezaEn(5, 0, alfilPrefab, TipoPieza.Alfil, ColorPieza.BLANCO);
        CrearPiezaEn(3, 0, reinaPrefab, TipoPieza.Reina, ColorPieza.BLANCO);
        CrearPiezaEn(4, 0, reyPrefab, TipoPieza.Rey, ColorPieza.BLANCO);

        CrearPiezaEn(0, 7, torrePrefab, TipoPieza.Torre, ColorPieza.NEGRO);
        CrearPiezaEn(7, 7, torrePrefab, TipoPieza.Torre, ColorPieza.NEGRO);
        CrearPiezaEn(1, 7, caballoPrefab, TipoPieza.Caballo, ColorPieza.NEGRO);
        CrearPiezaEn(6, 7, caballoPrefab, TipoPieza.Caballo, ColorPieza.NEGRO);
        CrearPiezaEn(2, 7, alfilPrefab, TipoPieza.Alfil, ColorPieza.NEGRO);
        CrearPiezaEn(5, 7, alfilPrefab, TipoPieza.Alfil, ColorPieza.NEGRO);
        CrearPiezaEn(3, 7, reinaPrefab, TipoPieza.Reina, ColorPieza.NEGRO);
        CrearPiezaEn(4, 7, reyPrefab, TipoPieza.Rey, ColorPieza.NEGRO);
    }

    /*void CrearPiezaEn(int x, int y, GameObject prefabPieza, TipoPieza tipo, ColorPieza color)
    {
        if (prefabPieza == null) return;

        Vector2Int posicion = new Vector2Int(x, y);
        GameObject casilla = gestorTablero.GetCasilla(posicion);

        if (casilla == null) return;

        Vector3 posicionCasilla = casilla.transform.position;

        Renderer rendererCasilla = casilla.GetComponent<Renderer>();
        float topCasilla = rendererCasilla.bounds.max.y;

        GameObject nuevaPieza = Instantiate(prefabPieza);

        Renderer rendererPieza = nuevaPieza.GetComponent<Renderer>();
        float bottomPieza = rendererPieza.bounds.min.y;

        float offset = topCasilla - bottomPieza;

        nuevaPieza.transform.position += new Vector3(
            posicionCasilla.x - nuevaPieza.transform.position.x,
            offset,
            posicionCasilla.z - nuevaPieza.transform.position.z
        );

        nuevaPieza.transform.SetParent(casilla.transform, true);

        Rigidbody rb = nuevaPieza.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        nuevaPieza.AddComponent<MeshCollider>();

        Renderer renderer = nuevaPieza.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = (color == ColorPieza.BLANCO)
                ? materialBlanco
                : materialNegro;
        }

        AtributosPieza atributos = nuevaPieza.AddComponent<AtributosPieza>();
        atributos.tipo = tipo;
        atributos.color = color;
        atributos.posicionEnTablero = posicion;
        atributos.ActualizarValorOro();

        // Registro correcto en el tablero
        gestorTablero.RegistrarPieza(atributos, posicion);
    }*/

    void CrearPiezaEn(int x, int y, GameObject prefabPieza, TipoPieza tipo, ColorPieza color)
    {
        if (prefabPieza == null) return;

        Vector2Int posicion = new Vector2Int(x, y);
        GameObject casilla = gestorTablero.GetCasilla(posicion);

        // Si no encuentra la casilla, lo avisamos y salimos
        if (casilla == null)
        {
            Debug.LogWarning($"No se encontró casilla en {posicion}");
            return;
        }

        // 1. Calculamos la posición base de la casilla
        Vector3 posicionCasilla = casilla.transform.position;
        float alturaDestino = posicionCasilla.y + 0.5f; // Altura por defecto (por si no hay renderer)

        // 2. Intentamos obtener la altura real del Renderer (buscando también en hijos)
        Renderer rendererCasilla = casilla.GetComponentInChildren<Renderer>();

        if (rendererCasilla != null)
        {
            // Si tiene renderer, usamos la altura real de la superficie
            alturaDestino = rendererCasilla.bounds.max.y;
        }
        else
        {
            // Si no tiene renderer, avisamos pero continuamos con la altura por defecto
            Debug.LogWarning($"La casilla en {posicion} no tiene Renderer. Usando altura flotante.");
        }

        // 3. Creamos la pieza
        GameObject nuevaPieza = Instantiate(prefabPieza);

        // 4. Calculamos el "piso" de la pieza para centrarla verticalmente
        Renderer rendererPieza = nuevaPieza.GetComponentInChildren<Renderer>();
        float offsetVertical = 0f;

        if (rendererPieza != null)
        {
            float bottomPieza = rendererPieza.bounds.min.y;
            offsetVertical = alturaDestino - bottomPieza;
        }
        else
        {
            Debug.LogWarning($"El prefab de {tipo} no tiene Renderer.");
        }

        // 5. Posicionamos la pieza
        // Nota: nuevaPieza se crea en (0,0,0) o en la posición del prefab, así que movemos
        nuevaPieza.transform.position = new Vector3(
            posicionCasilla.x,
            nuevaPieza.transform.position.y + offsetVertical, // Ajustamos la altura
            posicionCasilla.z
        );

        nuevaPieza.transform.SetParent(casilla.transform, true);

        Rigidbody rb = nuevaPieza.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        nuevaPieza.AddComponent<BoxCollider>();

        Renderer renderer = nuevaPieza.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = (color == ColorPieza.BLANCO)
                ? materialBlanco
                : materialNegro;
        }

        AtributosPieza atributos = nuevaPieza.AddComponent<AtributosPieza>();
        atributos.tipo = tipo;
        atributos.color = color;
        atributos.posicionEnTablero = posicion;
        atributos.ActualizarValorOro();

        gestorTablero.RegistrarPieza(atributos, posicion);
    }

    public Mesh GetMallaPorTipo(TipoPieza tipo)
    {
        if (mallaDePiezas.TryGetValue(tipo, out Mesh malla))
            return malla;

        return null;
    }
}