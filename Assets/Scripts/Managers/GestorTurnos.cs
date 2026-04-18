using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GestorTurnos : MonoBehaviour
{
    [Header("UI Visual")]
    [SerializeField] private TurnAnnouncer anunciadorTurno;
    [SerializeField] private TMP_Text textoTurnoHUD;

    [Header("Referencias")]
    [SerializeField] private ShopManager shopManager;

    // --- NUEVA REFERENCIA ---
    [Tooltip("Arrastra aquí el objeto que tiene el script IndicadorAcumulacion")]
    [SerializeField] private IndicadorAcumulacion indicadorCargador;
    // ------------------------

    [Header("Estado del turno")]
    private ColorPieza turnoActual = ColorPieza.BLANCO;
    public ColorPieza TurnoActual
    {
        get { return turnoActual; }
    }

    private Dictionary<Vector2Int, AtributosPieza> registroDePiezas;

    public void Inicializar(Dictionary<Vector2Int, AtributosPieza> registro)
    {
        registroDePiezas = registro;
    }

    public void CambiarTurno()
    {
        // 1. Lógica de Recompensas (Estatuadorada) para el jugador que termina su turno
        if (MoneyManager.instance != null && registroDePiezas != null)
        {
            foreach (var kvp in registroDePiezas)
            {
                AtributosPieza pieza = kvp.Value;
                // Verificamos si la pieza es del jugador actual y tiene el efecto
                if (pieza.color == turnoActual && pieza.DebeRecibirRecompensaEstatuadorada())
                {
                    MoneyManager.instance.AnadirDinero(turnoActual, 1);
                    Debug.Log($"{pieza.name} generó 1 de oro por Estatuadorada.");
                }
            }
        }

        // 2. Cambio de Turno Lógico
        turnoActual = (turnoActual == ColorPieza.BLANCO) ? ColorPieza.NEGRO : ColorPieza.BLANCO;
        Debug.Log($"--- Ahora es el turno de las piezas {turnoActual} ---");

        // 3. Resetear estados del nuevo jugador
        ResetearContadoresDeMovimiento(turnoActual);
        ResetearEstadoMovimiento(turnoActual);

        // 4. Actualizar UI y Tienda
        ActualizarUI();

        if (shopManager != null)
            shopManager.RefrescarTienda();

        // --- 5. ACTUALIZAR INDICADOR DE ACUMULACIÓN ---
        // Le decimos al indicador que verifique si debe mostrarse para el nuevo turno
        if (indicadorCargador != null)
        {
            indicadorCargador.VerificarYActualizar();
        }
        // ----------------------------------------------
    }

    private void ResetearContadoresDeMovimiento(ColorPieza colorJugador)
    {
        if (registroDePiezas == null) return;
        foreach (var kvp in registroDePiezas)
        {
            if (kvp.Value.color == colorJugador)
                kvp.Value.movimientosRestantesEsteTurno = 1;
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