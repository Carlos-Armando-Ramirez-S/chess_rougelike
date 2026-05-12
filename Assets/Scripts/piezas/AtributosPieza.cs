using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems; // <--- NECESARIO

public enum ColorPieza
{
    BLANCO,
    NEGRO
}

public enum TipoPieza
{
    Peon,
    Torre,
    Caballo,
    Alfil,
    Reina,
    Rey
}

public class AtributosPieza : MonoBehaviour
{
    [Header("Atributos de la Pieza")]
    public ColorPieza color;
    public TipoPieza tipo;
    public Vector2Int posicionEnTablero;
    public int valorOro;

    [Header("Estado del Escudo")]
    public bool tieneEscudo = false;

    [Header("Estado de Estatuadorada")]
    public bool estatuadoradaActiva = false;

    [Header("Estado de Movimiento Global")]
    public bool yaSeMovioEnPartida = false;
    public bool seMovioEsteTurno = false;

    [Header("Movimiento Extra")]
    public int movimientosRestantesEsteTurno = 1;

    [Header("Ataque Flanqueo")]
    public bool ataqueFlanqueoActivo = false;

    [Header("Inventario")]
    public List<ItemData> itemsEquipados = new List<ItemData>();
    public int limiteItems = 4;

    [Header("Visual Item")]
    private GameObject iconoItemVisual; // Referencia al sprite 2D

    private void OnMouseDown()
    {
        // --- AÑADE ESTA LÍNEA ---
        // Si el ratón está sobre algún elemento UI (Panel, Botón, Texto), salimos.
        if (EventSystem.current.IsPointerOverGameObject()) return;
        // ------------------------

        if (GameManager.instance != null)
        {
            GameManager.instance.SeleccionarPieza(this);
        }
    }

    // Función para que InicializadorPiezas nos pase el icono
    public void SetIconoVisual(GameObject icono)
    {
        iconoItemVisual = icono;
        iconoItemVisual.SetActive(false); // Empieza apagado
    }

    // Función para equipar items
    public bool EquiparItem(ItemData nuevoItem)
    {
        if (itemsEquipados.Count >= limiteItems)
        {
            Debug.Log($"{gameObject.name} no puede cargar más items (Límite: {limiteItems}).");
            return false;
        }

        itemsEquipados.Add(nuevoItem);

        // ENCENDER ICONO
        if (iconoItemVisual != null)
        {
            iconoItemVisual.SetActive(true);
        }

        Debug.Log($"<color=cyan>{gameObject.name} ha equipado: {nuevoItem.nombreItem} ({itemsEquipados.Count}/{limiteItems})</color>");
        return true;
    }

    // =========================
    // PROMOCIÓN
    // =========================

    public void Promocionar()
    {
        if (tipo < TipoPieza.Rey)
        {
            tipo++;
            ActualizarValorOro();
        }
    }

    // =========================
    // EFECTOS
    // =========================

    public void ActivarEstatuadorada()
    {
        estatuadoradaActiva = true;
        Debug.Log($"<color=purple>Efecto Estatuadorada ACTIVADO en {gameObject.name}</color>");
    }

    public void ActivarMovimientoDoble()
    {
        movimientosRestantesEsteTurno = 2;
        Debug.Log($"<color=orange>Movimiento doble ACTIVADO en {gameObject.name}</color>");
    }

    public void ActivarAtaqueFlanqueo()
    {
        ataqueFlanqueoActivo = true;
        Debug.Log($"<color=red>Ataque Flanqueo ACTIVADO en {gameObject.name}</color>");
    }

    public bool DebeRecibirRecompensaEstatuadorada()
    {
        if (estatuadoradaActiva)
        {
            Debug.Log($"<color=green>Recompensa Estatuadorada para {gameObject.name}</color>");
            return true;
        }
        return false;
    }

    // =========================
    // VALOR EN ORO
    // =========================

    public void ActualizarValorOro()
    {
        switch (tipo)
        {
            case TipoPieza.Peon: valorOro = 3; break;
            case TipoPieza.Torre: valorOro = 6; break;
            case TipoPieza.Caballo: valorOro = 10; break;
            case TipoPieza.Alfil: valorOro = 15; break;
            case TipoPieza.Reina: valorOro = 20; break;
            case TipoPieza.Rey: valorOro = 50; break;
        }
    }

    // =========================
    // SISTEMA DE CARGADOR
    // =========================

    [Header("Cargador Activo")]
    public bool tieneCargador = false;
    public int cargasActuales = 0;
    private ItemCargador datosCargador;

    public void ActivarCargador(ItemCargador datos)
    {
        tieneCargador = true;
        datosCargador = datos;
        Debug.Log($"Cargador equipado. Cada captura dará {datos.cargasPorCaptura} cargas.");
    }

    public void SumarCargaCaptura()
    {
        if (!tieneCargador || datosCargador == null) return;
        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.SumarCarga(this.color, datosCargador.cargasPorCaptura);
        }
    }

    // =========================
    // ESCUDO
    // =========================

    public void ActivarEscudoEnPieza()
    {
        tieneEscudo = true;
        Debug.Log($"<color=cyan>Escudo ACTIVADO en {gameObject.name}</color>");
    }

    public bool IntentarUsarEscudo()
    {
        if (tieneEscudo)
        {
            Debug.Log($"<color=yellow>{gameObject.name} usó su ESCUDO</color>");
            tieneEscudo = false;
            return true;
        }
        return false;
    }
}