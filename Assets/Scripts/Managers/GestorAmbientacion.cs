using UnityEngine;

public class GestorAmbientacion : MonoBehaviour
{
    [Header("Referencias")]
    public Light sol;                 // Arrastra tu Luz Direccional
    public Camera camaraPrincipal;    // Arrastra tu C�mara principal aqu�

    [Header("Referencias Adicionales")]
    public Renderer abismoRenderer; // Arrastra aqu� tu plano "AbismoNubes"
    private Material materialAbismo;

    [Header("L�mites de Piezas (Cantidad Restante)")]
    [Tooltip("Si quedan menos de estas piezas, empieza la fase Fuerte")]
    public int limitePiezasFuerte = 20;

    [Tooltip("Si quedan menos de estas piezas, empieza el Caos final")]
    public int limitePiezasCaos = 8;

    [Header("Colores del Fondo")]
    [Tooltip("El color inicial (Gris o Azul Cielo)")]
    public Color colorTranquilo = new Color(0.6f, 0.6f, 0.6f); // Gris claro por defecto

    [Tooltip("Color de advertencia (Rojizo)")]
    public Color colorFuerte = new Color(0.6f, 0.2f, 0.1f);    // Rojo oscuro

    [Tooltip("Color final (Negro absoluto)")]
    public Color colorCaos = new Color(0.05f, 0.0f, 0.0f);     // Negro

    [Header("Objetos de Ambiente")]
    public GameObject[] objetosTranquilos;
    public GameObject[] objetosFuertes;
    public GameObject[] objetosCaos;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip musicaTranquila;
    public AudioClip musicaFuerte;
    public AudioClip musicaCaos;

    private int estadoActual = 0;
    private float timer;

    void Start()
    {
        // Si no arrastraste la c�mara, intentamos buscarla autom�ticamente
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        // Inicializar colores
        if (camaraPrincipal != null)
            camaraPrincipal.backgroundColor = colorTranquilo; // CORREGIDO AQU�

        // Guardamos referencia al material del abismo
        if (abismoRenderer != null)
        {
            materialAbismo = abismoRenderer.material;
        }

        // Configurar niebla para que combine (opcional)
        RenderSettings.fog = true;
        RenderSettings.fogColor = colorTranquilo;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 50f;
        RenderSettings.fogEndDistance = 200f;

        CambiarEstadoVisual(0);
        CambiarMusica(0);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f) // Revisar cada 1 segundo
        {
            timer = 0;
            VerificarCantidadDePiezas();
        }

        // Transici�n suave de colores
        Color colorObjetivo = ObtenerColorObjetivo();

        // 1. Cambiar Fondo de C�mara
        if (camaraPrincipal != null)
        {
            camaraPrincipal.backgroundColor = Color.Lerp(camaraPrincipal.backgroundColor, colorObjetivo, Time.deltaTime * 0.5f);
        }

        // 2. Cambiar Niebla
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, colorObjetivo, Time.deltaTime * 0.5f);

        // 3. Cambiar Luz y efectos locos
        if (sol != null)
        {
            sol.color = Color.Lerp(sol.color, Color.white, Time.deltaTime); // Intentamos mantener la luz blanca, o cambia a colorObjetivo si prefieres

            // Efecto de temblor en el caos
            if (estadoActual == 2)
            {
                // Movemos la luz un poco para que parezca inestable
                sol.transform.rotation = Quaternion.Euler(
                    50 + Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0
                );
            }
        }
        // --- NUEVO: TRANSICI�N DEL ABISMO ---
        if (materialAbismo != null)
        {
            // Cambiamos el color del material
            materialAbismo.color = Color.Lerp(materialAbismo.color, colorObjetivo, Time.deltaTime * 0.5f);
        }

        if (materialAbismo != null)
        {
            // Movemos la textura lentamente
            float velocidad = 0.02f; // Velocidad lenta para tranquilidad
            Vector2 offset = new Vector2(Time.time * velocidad, 0);
            materialAbismo.mainTextureOffset = offset;

            // Tu c�digo anterior de color...
            // materialAbismo.color = Color.Lerp...
        }
    }

    void VerificarCantidadDePiezas()
    {
        AtributosPieza[] piezasVivas = FindObjectsByType<AtributosPieza>(FindObjectsSortMode.None);
        int cantidadTotal = piezasVivas.Length;


        if (cantidadTotal < limitePiezasCaos && estadoActual != 2)
        {
            CambiarEstadoVisual(2);
        }
        else if (cantidadTotal < limitePiezasFuerte && cantidadTotal >= limitePiezasCaos && estadoActual != 1)
        {
            CambiarEstadoVisual(1);
        }
        else if (cantidadTotal >= limitePiezasFuerte && estadoActual != 0)
        {
            CambiarEstadoVisual(0);
        }
    }

    Color ObtenerColorObjetivo()
    {
        switch (estadoActual)
        {
            case 1: return colorFuerte;
            case 2: return colorCaos;
            default: return colorTranquilo;
        }
    }
    void CambiarMusica(int estado)
    {
        if (audioSource == null) return;

        AudioClip nuevaMusica = null;

        switch (estado)
        {
            case 0:
                nuevaMusica = musicaTranquila;
                break;
            case 1:
                nuevaMusica = musicaFuerte;
                break;
            case 2:
                nuevaMusica = musicaCaos;
                break;
        }

        if (audioSource.clip == nuevaMusica) return;

        audioSource.clip = nuevaMusica;
        audioSource.Play();
    }

    void CambiarEstadoVisual(int nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        estadoActual = nuevoEstado;
        Debug.Log($"<color=yellow>AMBIENTE: Cambiando a Fase {nuevoEstado}</color>");

        ActivarGrupo(objetosTranquilos, nuevoEstado == 0);
        ActivarGrupo(objetosFuertes, nuevoEstado == 1);
        ActivarGrupo(objetosCaos, nuevoEstado == 2);
        CambiarMusica(nuevoEstado);
    }

    void ActivarGrupo(GameObject[] lista, bool estado)
    {
        if (lista == null) return;
        foreach (var obj in lista) if (obj != null) obj.SetActive(estado);
    }

}