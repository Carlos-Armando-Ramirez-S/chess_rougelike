using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    #region Singleton
    // --- ESTA ES LA PARTE QUE FALTABA ---
    public static ShopManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }
    #endregion

    [Header("Configuración de la Tienda")]
    [Tooltip("Los 3 espacios donde aparecerán los ítems en la UI")]
    public Transform[] slotsTienda;

    [Tooltip("Una lista con TODOS los prefabs de ítems que pueden salir")]
    public List<GameObject> todosLosItemsPrefabs;

    [Header("Configuración del Reroll")]
    [Tooltip("¿Cuánto cuesta refrescar los ítems de la tienda?")]
    public int costoReroll = 5; // <-- NUEVA VARIABLE

    // Guardamos los ítems que están actualmente en la tienda para poder destruirlos
    private GameObject[] itemsActuales;

    void Start()
    {
        itemsActuales = new GameObject[slotsTienda.Length]; // Inicializamos el array
        RefrescarTienda();
    }

    /// <summary>
    /// Destruye los ítems actuales y genera 3 nuevos al azar.
    /// </summary>
    public void RefrescarTienda()
    {
        // 1. Destruir los ítems viejos si existen
        for (int i = 0; i < itemsActuales.Length; i++)
        {
            if (itemsActuales[i] != null) Destroy(itemsActuales[i]);
        }

        // 2. Generar 3 nuevos ítems
        for (int i = 0; i < slotsTienda.Length; i++)
        {
            // Elegimos un ítem al azar de la lista de todos los posibles
            int indiceAleatorio = Random.Range(0, todosLosItemsPrefabs.Count);
            GameObject prefabElegido = todosLosItemsPrefabs[indiceAleatorio];

            // Lo creamos (instanciamos) en el slot correspondiente
            GameObject nuevoItem = Instantiate(prefabElegido, slotsTienda[i]);
            nuevoItem.transform.localPosition = Vector3.zero; // Lo centramos en el slot

            // Lo añadimos a nuestro array de ítems actuales
            itemsActuales[i] = nuevoItem;
        }

        Debug.Log("Tienda refrescada con nuevos ítems.");
    }

    /// <summary>
    /// Es llamado por un botón de la UI para comprar el ítem de un slot específico.
    /// </summary>
    /// <param name="indiceSlot">El índice del slot (0, 1 o 2).</param>
    public void ComprarItemEnSlot(int indiceSlot)
    {
        // Verificamos que el índice sea válido
        if (indiceSlot < 0 || indiceSlot >= itemsActuales.Length)
        {
            Debug.LogError("ShopManager: Índice de slot inválido.");
            return;
        }

        // Obtenemos el ítem del array
        GameObject itemAComprar = itemsActuales[indiceSlot];

        if (itemAComprar != null)
        {
            Debug.Log($"<color=cyan>--- DETECTIVE: Botón del slot {indiceSlot} presionado ---</color>");
            Debug.Log($"<color=cyan>El objeto en ese slot se llama: '{itemAComprar.name}'</color>");

            ItemTienda scriptItem = itemAComprar.GetComponent<ItemTienda>();
            if (scriptItem != null)
            {
                // Llamamos a ComprarItem y guardamos el resultado
                bool compraExitosa = scriptItem.ComprarItem();

                // --- NUEVA LÓGICA: Si la compra fue exitosa, destruimos el ítem ---
                if (compraExitosa)
                {
                    Debug.Log($"<color=green>COMPRA EXITOSA: Destruyendo el ítem del slot {indiceSlot}.</color>");

                    // 1. Destruimos el objeto visual del ítem
                    Destroy(itemAComprar);

                    // 2. Actualizamos nuestro array para que sepa que el slot está vacío
                    itemsActuales[indiceSlot] = null;
                }
            }
            else
            {
                Debug.LogError("ShopManager: El ítem en el slot " + indiceSlot + " no tiene un script ItemTienda.");
            }
        }
        else
        {
            Debug.Log("ShopManager: No hay ningún ítem en el slot " + indiceSlot + " para comprar.");
        }
    }


    public void RerollTienda()
    {
        ColorPieza jugadorActual = GameManager.instance.TurnoActual;

        // 1. Comprobar si el jugador tiene suficiente oro
        if (MoneyManager.instance.GetDinero(jugadorActual) >= costoReroll)
        {
            // 2. Si tiene oro, gastarlo y refrescar
            MoneyManager.instance.GastarDinero(jugadorActual, costoReroll);
            RefrescarTienda();
            Debug.Log($"<color=yellow>El jugador {jugadorActual} ha pagado {costoReroll} de oro para refrescar la tienda.</color>");
        }
        else
        {
            // 3. Si no tiene oro, mostrar un mensaje de error
            int oroActual = MoneyManager.instance.GetDinero(jugadorActual);
            Debug.Log($"<color=red>Oro insuficiente para refrescar la tienda. Tienes {oroActual} y necesitas {costoReroll}.</color>");
            // Opcional: Aquí podrías mostrar un mensaje de error en la UI del juego.
        }
    }
}