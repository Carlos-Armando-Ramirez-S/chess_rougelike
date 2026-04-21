using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTienda : MonoBehaviour
{
    [Header("Configuración")]
    public ItemData datosDelItem;

    [Header("Referencias UI")]
    public TMP_Text textoNombreUI;
    public TMP_Text textoPrecioUI;
    public Image iconoUI;

    void Start()
    {
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (datosDelItem == null) return;

        if (textoNombreUI != null) textoNombreUI.text = datosDelItem.nombreItem;
        if (textoPrecioUI != null) textoPrecioUI.text = "Costo: " + datosDelItem.costoItem;
        if (iconoUI != null) iconoUI.sprite = datosDelItem.icono;
    }

    // --- FUNCIÓN PARA EL BOTÓN (Void) ---
    // Esta es la que seleccionarás en el OnClick del botón
    public void IntentarComprar()
    {
        bool exito = ComprarItem();

        // Si la compra fue exitosa, el item se destruye solo
        if (exito)
        {
            Destroy(gameObject);
        }
    }

    // --- FUNCIÓN DE LÓGICA (Bool) ---
    // Esta la usa el ShopManager
    public bool ComprarItem()
    {
        // 1. Verificaciones
        if (datosDelItem == null || GameManager.instance == null || MoneyManager.instance == null) return false;

        AtributosPieza pieza = GameManager.instance.ObtenerPiezaSeleccionada();

        if (pieza == null)
        {
            Debug.LogWarning("Selecciona una pieza primero.");
            return false;
        }

        if (pieza.itemsEquipados.Count >= pieza.limiteItems)
        {
            Debug.LogWarning("Esta pieza está llena.");
            return false;
        }

        ColorPieza jugador = GameManager.instance.TurnoActual;
        int dinero = MoneyManager.instance.GetDinero(jugador);

        if (dinero < datosDelItem.costoItem)
        {
            Debug.LogWarning("Oro insuficiente.");
            return false;
        }

        // --- COMPRA EXITOSA ---
        MoneyManager.instance.GastarDinero(jugador, datosDelItem.costoItem);
        pieza.EquiparItem(datosDelItem);
        datosDelItem.EjecutarEfecto(pieza);

        Debug.Log($"<color=green>¡Has comprado {datosDelItem.nombreItem}!</color>");

        return true; // Devolvemos TRUE
    }
}