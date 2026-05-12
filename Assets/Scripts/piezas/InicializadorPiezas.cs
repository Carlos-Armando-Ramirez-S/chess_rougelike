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

    [Header("Visual Items")]
    [Tooltip("Arrastra aquí el prefab del icono que creaste")]
    public GameObject prefabIconoItem; // <--- NUEVA VARIABLE

    private Dictionary<TipoPieza, Mesh> mallaDePiezas = new Dictionary<TipoPieza, Mesh>();

    private GestorTablero gestorTablero;

    void Start()
    {
        InicializarMallas();
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

        // --- NUEVA LÓGICA: AÑADIR ICONO DE ITEM ---
        if (prefabIconoItem != null)
        {
            GameObject icono = Instantiate(prefabIconoItem, nuevaPieza.transform);

            // ALTURA: Probemos con 0.3 unidades (muy pegado a la cabeza)
            icono.transform.localPosition = new Vector3(0, 0.05f, 0);

            // ESCALA: 0.1 unidades (tamaño de una canica pequeña)
            icono.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            icono.SetActive(false);
            atributos.SetIconoVisual(icono);
        }
        // -----------------------------------------

        gestorTablero.RegistrarPieza(atributos, posicion);
    }

    public Mesh GetMallaPorTipo(TipoPieza tipo)
    {
        if (mallaDePiezas.TryGetValue(tipo, out Mesh malla))
            return malla;

        return null;
    }
}