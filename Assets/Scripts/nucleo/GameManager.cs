using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gestores")]
    [SerializeField] public GestorTurnos gestorTurnos;
    [SerializeField] public GestorTablero gestorTablero;
    [SerializeField] public CalculadorMovimientos calculadorMovimientos;
    [SerializeField] public GestorMovimiento gestorMovimiento;
    [SerializeField] public GestorPromocion gestorPromocion;

    // --- NUEVA VARIABLE AQUÍ (Fuera de cualquier función) ---
    [Header("IA Enemiga")]
    [SerializeField] private EnemyAI enemyAI;
    // -------------------------------------------------------

    [Header("UI Info Pieza")]
    [SerializeField] private PanelInfoPieza panelInfoPieza;

    [Header("Visual")]
    [SerializeField] private Material materialMovimiento;
    [SerializeField] private Material materialCaptura;

    private AtributosPieza piezaSeleccionada;
    private List<Vector2Int> movimientosPosibles = new List<Vector2Int>();

    [Header("Estado de partida")]
    private bool juegoTerminado = false;

    [Header("UI Fin de partida")]
    [SerializeField] private GameObject panelFinDeJuego;
    [SerializeField] private UnityEngine.UI.Text textoGanador;

    void Awake()
    {
        instance = this;

        // --- LÓGICA DE MODO DE JUEGO (DENTRO DE AWAKE) ---
        VerificarModoDeJuego();
        // -------------------------------------------------
    }

    // ... El resto de tus funciones (SeleccionarPieza, etc.) se quedan igual ...

    // ============================
    // SELECCIÓN DE PIEZA
    // ============================

    public void SeleccionarPieza(AtributosPieza pieza)
    {
        if (pieza == null) return;

        // --- BLOQUEO DE INPUT PARA CPU ---
        // Cambiamos 'esPvCPU' por 'esModoCPU' para que coincida con tu script
        if (GameController.Instance != null && GameController.Instance.esModoCPU)
        {
            if (gestorTurnos.TurnoActual == ColorPieza.NEGRO)
            {
                return; // Bloqueamos el clic si es turno de la CPU
            }
        }
        // ----------------------------------

        if (!gestorTurnos.EsTurnoDe(pieza.color))
        {
            if (piezaSeleccionada != null)
            {
                GameObject casillaEnemiga = gestorTablero.GetCasilla(pieza.posicionEnTablero);
                if (casillaEnemiga != null)
                {
                    IntentarMoverA(casillaEnemiga);
                }
            }
            return;
        }

        if (piezaSeleccionada == pieza)
        {
            LimpiarSeleccion();
            return;
        }

        LimpiarSeleccion();
        piezaSeleccionada = pieza;

        if (panelInfoPieza != null) panelInfoPieza.MostrarInfo(pieza);
        movimientosPosibles = calculadorMovimientos.CalcularMovimientos(pieza);
        MostrarMovimientos();
    }

    public void IntentarMoverA(GameObject casillaDestinoObj)
    {
        if (piezaSeleccionada == null) return;

        Casilla casilla = casillaDestinoObj.GetComponent<Casilla>();
        if (casilla == null) return;

        Vector2Int destino = casilla.coordenada;

        if (!movimientosPosibles.Contains(destino)) return;

        StartCoroutine(EjecutarMovimiento(casilla, destino));
    }

    private IEnumerator EjecutarMovimiento(Casilla casilla, Vector2Int destino)
    {
        yield return StartCoroutine(gestorMovimiento.MoverPieza(piezaSeleccionada, destino));

        gestorPromocion.VerificarPromocion(piezaSeleccionada);
        piezaSeleccionada.seMovioEsteTurno = true;

        piezaSeleccionada.movimientosRestantesEsteTurno--;

        if (piezaSeleccionada.movimientosRestantesEsteTurno > 0)
        {
            movimientosPosibles = calculadorMovimientos.CalcularMovimientos(piezaSeleccionada);
            MostrarMovimientos();
        }
        else
        {
            gestorTurnos.CambiarTurno();
            LimpiarSeleccion();
        }
    }

    void MostrarMovimientos()
    {
        foreach (Vector2Int pos in movimientosPosibles)
        {
            GameObject casilla = gestorTablero.GetCasilla(pos);

            if (casilla != null)
            {
                Casilla script = casilla.GetComponent<Casilla>();

                if (script != null)
                {
                    if (gestorTablero.GetPiezaEn(pos) != null)
                    {
                        script.Iluminar(materialCaptura);
                    }
                    else
                    {
                        script.Iluminar(materialMovimiento);
                    }
                }
            }
        }
    }

    public void LimpiarSeleccion()
    {
        foreach (Vector2Int pos in movimientosPosibles)
        {
            GameObject casilla = gestorTablero.GetCasilla(pos);

            if (casilla != null)
            {
                Casilla script = casilla.GetComponent<Casilla>();

                if (script != null)
                    script.Apagar();
            }
        }

        movimientosPosibles.Clear();
        piezaSeleccionada = null;

        if (panelInfoPieza != null)
        {
            panelInfoPieza.OcultarInfo();
        }
    }

    public void InicializarConTablero(CrearCasillas creador)
    {
        if (gestorTablero == null)
        {
            gestorTablero = FindFirstObjectByType<GestorTablero>();
        }

        if (gestorTablero != null && creador.casillas != null)
        {
            gestorTablero.ConfigurarCasillas(creador.casillas);
            gestorTablero.enabled = true;

            if (InicializadorPiezas.instance != null)
            {
                InicializadorPiezas.instance.IniciarColocacion();
            }

            if (gestorTurnos != null)
            {
                gestorTurnos.Inicializar(gestorTablero.ObtenerRegistroCompleto());
            }
        }
        else
        {
            Debug.LogError("Falta el GestorTablero o el creador no tiene casillas.");
        }
    }

    public ColorPieza TurnoActual
    {
        get { return gestorTurnos.TurnoActual; }
    }

    public AtributosPieza ObtenerPiezaSeleccionada()
    {
        return piezaSeleccionada;
    }

    public void RefrescarInfoPiezaSeleccionada()
    {
        if (piezaSeleccionada != null && panelInfoPieza != null)
        {
            panelInfoPieza.MostrarInfo(piezaSeleccionada);
        }
    }

    public void FinalizarPartida(ColorPieza perdedor)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        ColorPieza ganador = (perdedor == ColorPieza.BLANCO) ? ColorPieza.NEGRO : ColorPieza.BLANCO;

        if (panelFinDeJuego != null)
        {
            panelFinDeJuego.SetActive(true);
            if (textoGanador != null)
            {
                textoGanador.text = "¡Ganan las " + ganador + "!";
            }
        }
    }

    // --- NUEVAS FUNCIONES AL FINAL ---

    private void VerificarModoDeJuego()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.esModoCPU)
            {
                Debug.Log("Modo: Player vs CPU");
                if (enemyAI != null) enemyAI.enabled = true;
            }
            else
            {
                Debug.Log("Modo: Player vs Player");
                if (enemyAI != null) enemyAI.enabled = false;
            }
        }
    }

    public void EjecutarMovimientoIA(AtributosPieza pieza, Vector2Int destino)
    {
        piezaSeleccionada = pieza;
        movimientosPosibles = calculadorMovimientos.CalcularMovimientos(pieza);

        GameObject casillaObj = gestorTablero.GetCasilla(destino);
        if (casillaObj != null && movimientosPosibles.Contains(destino))
        {
            StartCoroutine(EjecutarMovimiento(casillaObj.GetComponent<Casilla>(), destino));
        }
    }
}