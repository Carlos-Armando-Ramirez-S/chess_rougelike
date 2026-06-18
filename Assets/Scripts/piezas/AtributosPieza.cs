using UnityEngine;
using System.Collections.Generic; // NECESARIO para usar Listas

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

    // Variable existente (se reinicia cada turno)
    public bool seMovioEsteTurno = false;

    // --- NUEVA VARIABLE A�ADIDA ---
    // Esta variable NO se reinicia. Guarda si la pieza se ha movido alguna vez en toda la partida.
    // Es vital para que el pe�n solo pueda saltar 2 casillas en su primer movimiento.
    [Header("Estado de Movimiento Global")]
    public bool yaSeMovioEnPartida = false;
    // ------------------------------

    [Header("Movimiento Extra")]
    public int movimientosRestantesEsteTurno = 1;

    [Header("Ataque Flanqueo")]
    public bool ataqueFlanqueoActivo = false;


    private void OnMouseDown()
    {
        SoundManager.instance?.PlayPieceSound(tipo);

        if (GameManager.instance != null)
        {
            GameManager.instance.SeleccionarPieza(this);
        }
    }


    [Header("Visual Item (Opcional)")]
    public SpriteRenderer iconoItemVisual; // Arrastra aqu� un hijo que sirva de icono si quieres

    [Header("Inventario")]
    public List<ItemData> itemsEquipados = new List<ItemData>();
    public int limiteItems = 4;

    // Esta funci�n la llamar� la tienda para intentar equipar el item
    public bool EquiparItem(ItemData nuevoItem)
    {
        // 1. Verificar si alcanz� el l�mite
        if (itemsEquipados.Count >= limiteItems)
        {
            Debug.Log($"{gameObject.name} no puede cargar m�s items (L�mite: {limiteItems}).");
            return false;
        }

        // 2. A�adir a la lista
        itemsEquipados.Add(nuevoItem);

        Debug.Log($"<color=cyan>{gameObject.name} ha equipado: {nuevoItem.nombreItem} ({itemsEquipados.Count}/{limiteItems})</color>");

        return true;
    }

    // =========================
    // PROMOCI�N
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


    // =========================
    // ESTATUADORADA
    // =========================

    public bool DebeRecibirRecompensaEstatuadorada()
    {
        // Ahora solo verifica si tiene la estatua activa, sin importar si se movi�
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
            case TipoPieza.Peon:
                valorOro = 3;
                break;

            case TipoPieza.Torre:
                valorOro = 6;
                break;

            case TipoPieza.Caballo:
                valorOro = 10;
                break;

            case TipoPieza.Alfil:
                valorOro = 15;
                break;

            case TipoPieza.Reina:
                valorOro = 20;
                break;

            case TipoPieza.Rey:
                valorOro = 50;
                break;
        }
    }

    // =========================
    // SISTEMA DE CARGADOR
    // =========================

    [Header("Cargador Activo")]
    public bool tieneCargador = false;
    public int cargasActuales = 0;
    private ItemCargador datosCargador; // Referencia al ScriptableObject para leer cu�nto sumar

    // Esta funci�n la llama el ItemCargador al comprarlo
    public void ActivarCargador(ItemCargador datos)
    {
        tieneCargador = true;
        datosCargador = datos;
        Debug.Log($"Cargador equipado. Cada captura dar� {datos.cargasPorCaptura} cargas.");
    }

    // Esta funci�n la llamar� el GestorCombate cuando captures una pieza
    public void SumarCargaCaptura()
    {
        // 1. Verificamos si la pieza tiene el item equipado
        if (!tieneCargador || datosCargador == null) return;

        // 2. Sumamos las cargas al contador GLOBAL del jugador (en MoneyManager)
        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.SumarCarga(this.color, datosCargador.cargasPorCaptura);
        }

        // NOTA: Hemos eliminado la parte de "if(cargas >= necesarias)" porque 
        // ahora el item es pasivo y no se activa solo.
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
            Debug.Log($"<color=yellow>{gameObject.name} us� su ESCUDO</color>");

            tieneEscudo = false;

            return true;
        }

        return false;
    }
}