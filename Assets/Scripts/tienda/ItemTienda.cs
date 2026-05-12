using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // <--- NECESARIO para detectar el ratón

// Añadimos las interfaces al final de la clase
public class ItemTienda : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    public void IntentarComprar()
    {
        bool exito = ComprarItem();

        if (exito)
        {
            // 1. Ocultar el Pop-up (lo arreglamos antes)
            if (GestorHerramientas.instance != null)
            {
                GestorHerramientas.instance.OcultarTooltip();
            }

            // 2. ¡NUEVO! Forzar al indicador a aparecer ahora mismo
            if (IndicadorAcumulacion.instance != null)
            {
                IndicadorAcumulacion.instance.VerificarYActualizar();
            }

            // 3. Destruir el item de la tienda
            Destroy(gameObject);
        }
    }

    // --- FUNCIÓN DE LÓGICA (Bool) ---
    public bool ComprarItem()
    {
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

        // COMPRA EXITOSA
        MoneyManager.instance.GastarDinero(jugador, datosDelItem.costoItem);
        pieza.EquiparItem(datosDelItem);
        datosDelItem.EjecutarEfecto(pieza);

        Debug.Log($"<color=green>¡Has comprado {datosDelItem.nombreItem}!</color>");

        return true;
    }

    // --- LÓGICA DEL TOOLTIP (POP-UP) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Al entrar el ratón, mostramos el tooltip
        if (datosDelItem != null && GestorHerramientas.instance != null)
        {
            GestorHerramientas.instance.MostrarTooltip(datosDelItem.nombreItem, datosDelItem.descripcion);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Al salir, ocultamos
        if (GestorHerramientas.instance != null)
        {
            GestorHerramientas.instance.OcultarTooltip();
        }
    }
}