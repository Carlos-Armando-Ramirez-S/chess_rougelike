using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemTienda : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración")]
    public ItemData datosDelItem;

    [Header("Referencias UI")]
    public Text textoNombreUI;
    public Text textoPrecioUI;
    public Image iconoUI;
    public Button botonComprar;

    void Start()
    {
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (datosDelItem == null) return;
        if (textoNombreUI != null) textoNombreUI.text = datosDelItem.nombreItem;
        if (textoPrecioUI != null) textoPrecioUI.text = "Costo: " + datosDelItem.costoItem;
        if (iconoUI != null) iconoUI.sprite = datosDelItem.icono;
    }

    public bool ComprarItem()
    {
        AtributosPieza pieza = GameManager.instance.ObtenerPiezaSeleccionada();

        // 1. Verificar selección
        if (pieza == null)
        {
            Debug.Log("<color=red>¡Selecciona una pieza antes de comprar!</color>");
            return false;
        }

        // 2. Verificar límite usando la lista
        if (pieza.itemsEquipados.Count >= pieza.limiteItems)
        {
            Debug.Log("<color=red>Esta pieza está llena de items.</color>");
            return false;
        }

        ColorPieza jugador = GameManager.instance.TurnoActual;

        if (MoneyManager.instance.GetDinero(jugador) >= datosDelItem.costoItem)
        {
            MoneyManager.instance.GastarDinero(jugador, datosDelItem.costoItem);

            // Añadimos a la lista
            pieza.EquiparItem(datosDelItem);

            // Ejecutamos el efecto
            datosDelItem.EjecutarEfecto(pieza);

            // Actualizamos el panel visual
            GameManager.instance.RefrescarInfoPiezaSeleccionada();

            return true;
        }
        else
        {
            Debug.Log("<color=red>Oro insuficiente.</color>");
            return false;
        }
    }

    // ... (Funciones de OnPointerEnter y OnPointerExit se quedan igual) ...
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (datosDelItem != null && GestorHerramientas.instance != null)
        {
            GestorHerramientas.instance.MostrarTooltip(datosDelItem.nombreItem, datosDelItem.descripcion);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GestorHerramientas.instance != null)
        {
            GestorHerramientas.instance.OcultarTooltip();
        }
    }
}