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
    public GameObject tiendaPanel;
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

    // --- NUEVO MÉTODO AÑADIDO ---
    /// <summary>
    /// Intenta gastar una cantidad de dinero para un jugador específico.
    /// </summary>
    /// <param name="jugador">El jugador que gasta el dinero.</param>
    /// <param name="cantidad">La cantidad a gastar.</param>
    /// <returns>True si el gasto fue exitoso, False si no había suficiente dinero.</returns>
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

        return false; // No hay suficiente dinero
    }

    private void ActualizarUIDinero()
    {
        if (DineroBNum != null) DineroBNum.text = dineroBlancas.ToString();
        if (textoDineroNegras != null) textoDineroNegras.text = dineroNegras.ToString();
    }
    #endregion

    #region Lógica de la Tienda
    public void MostrarOcultarTienda()
    {
        if (tiendaPanel != null) tiendaPanel.SetActive(!tiendaPanel.activeSelf);
    }
    #endregion

    #region Informacion
    public void MostrarOcultarInfo()
    {
        if (infoPanel != null) infoPanel.SetActive(!infoPanel.activeSelf);
    }
    #endregion

    // --- NUEVO MÉTODO ---
    /// <summary>
    /// Devuelve la cantidad de oro que tiene un jugador específico.
    /// </summary>
    public int GetDinero(ColorPieza jugador)
    {
        if (jugador == ColorPieza.BLANCO)
        {
            return dineroBlancas;
        }
        else // Asumimos que solo hay BLANCO y NEGRO
        {
            return dineroNegras;
        }
    }

    // --- NUEVAS FUNCIONES Cargas de items ---
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
}