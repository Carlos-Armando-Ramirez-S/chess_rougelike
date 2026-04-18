using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gestores")]
    [SerializeField] private GestorTurnos gestorTurnos;
    [SerializeField] private GestorTablero gestorTablero;
    [SerializeField] private CalculadorMovimientos calculadorMovimientos;
    [SerializeField] private GestorMovimiento gestorMovimiento;
    [SerializeField] private GestorPromocion gestorPromocion;

    [Header("UI Info Pieza")]
    [SerializeField] private PanelInfoPieza panelInfoPieza; // Arrastra aquí tu panel en el Inspector

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
    }


    // ============================
    // SELECCIÓN DE PIEZA
    // ============================

    public void SeleccionarPieza(AtributosPieza pieza)
    {
        // 1. CASO ESPECIAL: Si hacemos clic en la pieza que ya está seleccionada...
        // La deseleccionamos y apagamos las luces.
        if (piezaSeleccionada == pieza)
        {
            LimpiarSeleccion();
            return;
        }

        // 2. LIMPIAR SELECCIÓN ANTERIOR
        // Esto apaga las luces de la pieza vieja antes de encender las nuevas
        LimpiarSeleccion();

        // 3. VALIDACIONES BÁSICAS
        if (pieza == null)
            return;

        // Verificamos si es el turno correcto
        if (!gestorTurnos.EsTurnoDe(pieza.color))
            return;

        // 4. NUEVA SELECCIÓN
        piezaSeleccionada = pieza;

        // NUEVO: Avisar al panel para que muestre la info
        if (panelInfoPieza != null)
        {
            panelInfoPieza.MostrarInfo(pieza);
        }

        movimientosPosibles = calculadorMovimientos.CalcularMovimientos(pieza);

        // 5. MOSTRAR NUEVOS MOVIMIENTOS
        MostrarMovimientos();
    }


    // ============================
    // CLICK EN CASILLA
    // ============================

    public void IntentarMoverA(GameObject casillaDestinoObj)
    {
        if (piezaSeleccionada == null) return;

        Casilla casilla = casillaDestinoObj.GetComponent<Casilla>();
        if (casilla == null) return;

        Vector2Int destino = casilla.coordenada;

        if (!movimientosPosibles.Contains(destino)) return;

        // 1. Movemos la pieza (y capturamos si hay)
        gestorMovimiento.MoverPieza(piezaSeleccionada, destino);

        // 2. Verificamos promoción
        gestorPromocion.VerificarPromocion(piezaSeleccionada);

        // 3. Marcar que la pieza se movió este turno (importante para Estatuadorada)
        piezaSeleccionada.seMovioEsteTurno = true;

        // 4. LÓGICA DE DOBLE TURNO
        // Restamos un movimiento al contador
        piezaSeleccionada.movimientosRestantesEsteTurno--;

        // Si todavía le quedan movimientos, NO cambiamos de turno
        if (piezaSeleccionada.movimientosRestantesEsteTurno > 0)
        {
            Debug.Log($"A {piezaSeleccionada.name} le quedan {piezaSeleccionada.movimientosRestantesEsteTurno} movimientos.");
            // Recalculamos movimientos posibles para el segundo movimiento
            movimientosPosibles = calculadorMovimientos.CalcularMovimientos(piezaSeleccionada);
            MostrarMovimientos(); // Actualizamos luces
            return; // Salimos sin cambiar de turno
        }

        // 5. Si no le quedan movimientos, cambiamos de turno
        gestorTurnos.CambiarTurno();
        LimpiarSeleccion();
    }


    // ============================
    // VISUALIZACIÓN MOVIMIENTOS
    // ============================

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
                    // LÓGICA PARA CAMBIAR DE COLOR:

                    // Usamos la función GetPiezaEn que SÍ existe en tu GestorTablero
                    if (gestorTablero.GetPiezaEn(pos) != null)
                    {
                        // Si hay una pieza, es una CAPTURA -> Color Rojo (o el que elijas)
                        script.Iluminar(materialCaptura);
                    }
                    else
                    {
                        // Si no hay pieza, es un MOVIMIENTO NORMAL -> Color Verde (o el que tengas)
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
                    script.Apagar(); // CORRECCIÓN
            }
        }

        movimientosPosibles.Clear();
        piezaSeleccionada = null;

        // NUEVO: Ocultar el panel al deseleccionar
        if (panelInfoPieza != null)
        {
            panelInfoPieza.OcultarInfo();
        }
    }


    // ============================
    // INICIALIZACIÓN TABLERO
    // ============================
    public void InicializarConTablero(CrearCasillas creador)
    {
        Debug.Log("GameManager inicializado con tablero");

        // 1. Buscamos el GestorTablero si no está asignado
        if (gestorTablero == null)
        {
            gestorTablero = FindFirstObjectByType<GestorTablero>();
        }

        // 2. PASAMOS LAS CASILLAS MANUALMENTE AL GESTOR
        if (gestorTablero != null && creador.casillas != null)
        {
            gestorTablero.ConfigurarCasillas(creador.casillas);
            gestorTablero.enabled = true;

            // 3. AVISAMOS AL INICIALIZADOR DE PIEZAS
            if (InicializadorPiezas.instance != null)
            {
                InicializadorPiezas.instance.IniciarColocacion();
            }

            // --- NUEVA LÍNEA A AÑADIR ---
            // 4. CONECTAMOS EL GESTOR DE TURNOS CON EL TABLERO
            if (gestorTurnos != null)
            {
                gestorTurnos.Inicializar(gestorTablero.ObtenerRegistroCompleto());
            }
            // -----------------------------

        }
        else
        {
            Debug.LogError("Falta el GestorTablero o el creador no tiene casillas.");
        }
    }

    // ============================
    // ACCESO PUBLICO A SISTEMAS
    // ============================

    public ColorPieza TurnoActual
    {
        get
        {
            // Esto asume que 'gestorTurnos' no es nulo y tiene una propiedad pública llamada 'TurnoActual'
            return gestorTurnos.TurnoActual;
        }
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

    // ============================
    // Finalizacion de partida
    // ============================

    public void FinalizarPartida(ColorPieza perdedor)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Debug.Log("PARTIDA TERMINADA");

        ColorPieza ganador = (perdedor == ColorPieza.BLANCO) ? ColorPieza.NEGRO : ColorPieza.BLANCO;

        if (panelFinDeJuego != null)
        {
            panelFinDeJuego.SetActive(true);
            if (textoGanador != null)
            {
                textoGanador.text = "¡Ganan las " + ganador + "!";
            }
        }
        else
        {
            Debug.LogWarning("El panel de fin de juego no está asignado en el GameManager.");
        }
    }

}