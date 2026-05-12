// MoneyManager.cs

using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    #region Singleton y Estado
    public static MoneyManager instance { get; private set; }

    [Header("Dinero y UI")]
    public int dineroBlancas = 0, dineroNegras = 0;
    public Text DineroBNum;
    public Text textoDineroNegras;

    [Header("Tienda")]
    public GameObject tiendaPanel; // Mantenemos la referencia para saber si está activa
    public GameObject infoPanel;

    [Header("Cargas de Items")]
    public int cargasBlanco = 0;
    public int cargasNegro = 0;


    void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    void Start()
    {
        ActualizarUIDinero();
    }
    #endregion

    #region Lógica de Dinero
    public void AnadirDinero(ColorPieza jugador, int cantidad)
    {
        if (jugador == ColorPieza.BLANCO) dineroBlancas += cantidad;
        else dineroNegras += cantidad;
        ActualizarUIDinero();
    }

    public bool GastarDinero(ColorPieza jugador, int cantidad)
    {
        if (jugador == ColorPieza.BLANCO && dineroBlancas >= cantidad)
        {
            dineroBlancas -= cantidad;
            ActualizarUIDinero();
            return true;
        }
        else if (jugador == ColorPieza.NEGRO && dineroNegras >= cantidad)
        {
            dineroNegras -= cantidad;
            ActualizarUIDinero();
            return true;
        }

        return false;
    }

    private void ActualizarUIDinero()
    {
        if (DineroBNum != null) DineroBNum.text = dineroBlancas.ToString();
        if (textoDineroNegras != null) textoDineroNegras.text = dineroNegras.ToString();
    }
    #endregion

    // En MoneyManager.cs

    #region Lógica de la Tienda
    public void MostrarOcultarTienda()
    {
        // --- NUEVO: BLOQUEO DE TURNO ---
        // Si no es el turno del Jugador (Blancas), no dejamos abrir la tienda
        if (GameManager.instance.TurnoActual != ColorPieza.BLANCO)
        {
            Debug.Log("¡Solo puedes usar la tienda en tu turno!");
            return;
        }
        // -------------------------------

        if (ShopManager.instance != null && tiendaPanel != null)
        {
            if (tiendaPanel.activeSelf)
            {
                ShopManager.instance.CerrarTienda();
            }
            else
            {
                ShopManager.instance.AbrirTienda();
            }
        }
        else
        {
            if (tiendaPanel != null) tiendaPanel.SetActive(!tiendaPanel.activeSelf);
        }
    }
    #endregion

    #region Informacion
    public void MostrarOcultarInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(!infoPanel.activeSelf);
    }
    #endregion

    public int GetDinero(ColorPieza jugador)
    {
        if (jugador == ColorPieza.BLANCO)
        {
            return dineroBlancas;
        }
        else
        {
            return dineroNegras;
        }
    }

    #region Cargas de items
    public void SumarCarga(ColorPieza color, int cantidad)
    {
        if (color == ColorPieza.BLANCO)
            cargasBlanco += cantidad;
        else
            cargasNegro += cantidad;
    }

    public int GetCargas(ColorPieza color)
    {
        return (color == ColorPieza.BLANCO) ? cargasBlanco : cargasNegro;
    }
    public void RestarCarga(ColorPieza color, int cantidad)
    {
        if (color == ColorPieza.BLANCO)
            cargasBlanco -= cantidad;
        else
            cargasNegro -= cantidad;
    }

    public void ResetearCargas(ColorPieza color)
    {
        if (color == ColorPieza.BLANCO) cargasBlanco = 0;
        else cargasNegro = 0;
    }
    #endregion
}