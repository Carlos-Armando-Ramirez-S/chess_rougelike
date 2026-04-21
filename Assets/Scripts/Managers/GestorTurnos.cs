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
        ColorPieza jugadorQueTermina = turnoActual;

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
                        Debug.Log($"{pieza.name} generó 1 oro por Estatuadorada.");
                    }
                }
            }
        }

        // --- Cambio de Turno Lógico ---
        turnoActual = (turnoActual == ColorPieza.BLANCO) ? ColorPieza.NEGRO : ColorPieza.BLANCO;
        Debug.Log($"--- Ahora es el turno de las piezas {turnoActual} ---");

        // --- NUEVA LÓGICA: ORO AL INICIAR TURNO ---
        // Ahora le damos el oro al jugador que EMPIEZA su turno
        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.AnadirDinero(turnoActual, 1);
            Debug.Log($"<color=yellow>+1 oro por inicio de turno para {turnoActual}.</color>");
        }
        // --------------------------------------------

        ResetearContadoresDeMovimiento(turnoActual);
        ResetearEstadoMovimiento(turnoActual);

        ActualizarUI();

        if (shopManager != null)
            shopManager.RefrescarTienda();

        // Avisar al indicador de cargas
        if (indicadorCargador != null)
            indicadorCargador.VerificarYActualizar();
    }

    private void ResetearContadoresDeMovimiento(ColorPieza colorJugador)
    {
        if (registroDePiezas == null) return;
        foreach (var kvp in registroDePiezas)
        {
            if (kvp.Value.color == colorJugador)
            {
                kvp.Value.movimientosRestantesEsteTurno = 1;

                // --- NUEVA LÓGICA: AVISAR A LOS ITEMS ---
                // Revisamos si la pieza tiene items equipados
                if (kvp.Value.itemsEquipados != null)
                {
                    foreach (ItemData item in kvp.Value.itemsEquipados)
                    {
                        // Llamamos a la función del item
                        item.AlIniciarTurno(kvp.Value);
                    }
                }
                // ---------------------------------------
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