using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GestorTurnos : MonoBehaviour
{
    [Header("UI Visual")]
    [SerializeField] private TurnAnnouncer anunciadorTurno;
    [SerializeField] private TMP_Text textoTurnoHUD;

    // --- NUEVA UI: TEMPORIZADOR ---
    [Tooltip("Arrastra aquí un texto para mostrar el tiempo/bonus")]
    [SerializeField] private TMP_Text textoTemporizador;
    // ------------------------------

    [Header("Referencias")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private IndicadorAcumulacion indicadorCargador;

    [Header("Configuración de Velocidad")]
    [Tooltip("Tiempo máximo en segundos para obtener bonus.")]
    public float tiempoMaximo = 15f;
    [Tooltip("Oro máximo por mover rápido.")]
    public int oroMaximo = 5;

    // Variables de estado
    private ColorPieza turnoActual = ColorPieza.BLANCO;
    public ColorPieza TurnoActual => turnoActual;

    private Dictionary<Vector2Int, AtributosPieza> registroDePiezas;

    // --- NUEVAS VARIABLES DE TIEMPO ---
    private float tiempoInicioTurno;
    private bool turnoActivo = false;
    // ----------------------------------

    void Update()
    {
        if (turnoActivo)
        {
            float tiempoTranscurrido = Time.time - tiempoInicioTurno;
            float proporcion = tiempoTranscurrido / tiempoMaximo;
            if (proporcion > 1f) proporcion = 1f;

            // Usamos la misma fórmula que arriba para que el jugador vea lo que va a ganar
            int oroActual = 1 + Mathf.FloorToInt((oroMaximo - 1) * (1f - proporcion));

            if (textoTemporizador != null)
            {
                textoTemporizador.text = $"Bonus: +{oroActual}g";

                // Colores: Verde al inicio, Rojo al final
                if (proporcion > 0.8f)
                    textoTemporizador.color = Color.red;
                else if (proporcion > 0.4f)
                    textoTemporizador.color = Color.yellow;
            }
        }
    }

    public void Inicializar(Dictionary<Vector2Int, AtributosPieza> registro)
    {
        registroDePiezas = registro;
        // Inicializamos el turno (asumimos que empieza Blanco, pero no activamos el timer hasta el primer cambio)
    }

    public void CambiarTurno()
    {
        ColorPieza jugadorQueTermina = turnoActual;

        // --- CÁLCULO DE ORO (Mín 1, Máx 5) ---
        float tiempoTotal = Time.time - tiempoInicioTurno;

        // 1. Calculamos el porcentaje de tiempo usado (0.0 a 1.0)
        float proporcion = tiempoTotal / tiempoMaximo;

        // 2. Limitamos para no pasarnos
        if (proporcion > 1f) proporcion = 1f;

        // 3. Fórmula invertida con mínimo:
        // Rango de oro = 5 (Max) - 1 (Min) = 4 puntos variables.
        // Oro = 1 (Base) + (4 * (1 - proporcion))

        // Ejemplo:
        // t=0s (prop=0) -> 1 + 4 = 5 oro.
        // t=5s (prop=1) -> 1 + 0 = 1 oro.

        int oroGanado = 1 + Mathf.FloorToInt((oroMaximo - 1) * (1f - proporcion));

        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.AnadirDinero(jugadorQueTermina, oroGanado);
            Debug.Log($"<color=yellow>{jugadorQueTermina} tardó {tiempoTotal:F1}s. Ganó: {oroGanado} oro.</color>");
        }
        // ------------------------------------

        // --- Lógica de Recompensas (Estatua Dorada) ---
        if (registroDePiezas != null)
        {
            foreach (var kvp in registroDePiezas)
            {
                AtributosPieza pieza = kvp.Value;
                if (pieza.color == jugadorQueTermina)
                {
                    if (pieza.DebeRecibirRecompensaEstatuadorada())
                    {
                        MoneyManager.instance.AnadirDinero(jugadorQueTermina, 1);
                    }
                }
            }
        }

        // --- Cambio de Turno Lógico ---
        turnoActual = (turnoActual == ColorPieza.BLANCO) ? ColorPieza.NEGRO : ColorPieza.BLANCO;
        Debug.Log($"--- Ahora es el turno de las piezas {turnoActual} ---");

        ResetearContadoresDeMovimiento(turnoActual);
        ResetearEstadoMovimiento(turnoActual);

        ActualizarUI();
        if (shopManager != null) shopManager.RefrescarTienda();
        if (indicadorCargador != null) indicadorCargador.VerificarYActualizar();

        // Reiniciar Temporizador
        tiempoInicioTurno = Time.time;
        turnoActivo = true;
    }

    private void ResetearContadoresDeMovimiento(ColorPieza colorJugador)
    {
        if (registroDePiezas == null) return;
        foreach (var kvp in registroDePiezas)
        {
            if (kvp.Value.color == colorJugador)
            {
                kvp.Value.movimientosRestantesEsteTurno = 1;

                // Avisar a los items
                if (kvp.Value.itemsEquipados != null)
                {
                    foreach (ItemData item in kvp.Value.itemsEquipados)
                    {
                        item.AlIniciarTurno(kvp.Value);
                    }
                }
            }
        }
    }

    private void ResetearEstadoMovimiento(ColorPieza colorJugador)
    {
        if (registroDePiezas == null) return;
        foreach (var kvp in registroDePiezas)
        {
            if (kvp.Value.color == colorJugador)
                kvp.Value.seMovioEsteTurno = false;
        }
    }

    public bool EsTurnoDe(ColorPieza color)
    {
        return turnoActual == color;
    }

    public void DetenerTurnos()
    {
        turnoActivo = false;
        enabled = false;
    }

    private void ActualizarUI()
    {
        if (textoTurnoHUD != null)
        {
            textoTurnoHUD.text = "Turno: " + turnoActual.ToString();
        }

        if (anunciadorTurno != null)
        {
            anunciadorTurno.MostrarAviso(turnoActual);
        }
    }
}