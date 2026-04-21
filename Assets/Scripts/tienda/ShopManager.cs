using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    #region Singleton
    public static ShopManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }
    #endregion

    [Header("Configuración Visual")]
    [Tooltip("Arrastra aquí tu ÚNICO prefab base (el molde vacío)")]
    public GameObject prefabBaseItem; // <--- CAMBIO: Ahora es uno solo, no una lista

    [Tooltip("Los 3 espacios donde aparecerán los ítems en la UI")]
    public Transform[] slotsTienda;

    [Header("Datos de los Items")]
    [Tooltip("Arrastra aquí tus ScriptableObjects (Item_Bateria, Item_Legado, etc.)")]
    public List<ItemData> itemsDisponibles; // <--- CAMBIO: Lista de datos, no de prefabs

    [Header("Configuración del Reroll")]
    public int costoReroll = 5;

    private GameObject[] itemsActuales;

    void Start()
    {
        itemsActuales = new GameObject[slotsTienda.Length];
        RefrescarTienda();
    }

    public void RefrescarTienda()
    {
        // 1. Destruir los ítems viejos
        for (int i = 0; i < itemsActuales.Length; i++)
        {
            if (itemsActuales[i] != null) Destroy(itemsActuales[i]);
        }

        // 2. Generar 3 nuevos ítems usando el MISMO prefab
        for (int i = 0; i < slotsTienda.Length; i++)
        {
            // Verificamos que haya items en la lista de datos
            if (itemsDisponibles.Count == 0) return;

            // Elegimos los DATOS al azar
            int indiceAleatorio = Random.Range(0, itemsDisponibles.Count);
            ItemData datosElegidos = itemsDisponibles[indiceAleatorio];

            // Instanciamos el PREFAB BASE (el molde)
            GameObject nuevoItem = Instantiate(prefabBaseItem, slotsTienda[i]);
            nuevoItem.transform.localPosition = Vector3.zero;

            // --- LA MAGIA: INYECTAR LOS DATOS ---
            ItemTienda scriptVisual = nuevoItem.GetComponent<ItemTienda>();
            if (scriptVisual != null)
            {
                scriptVisual.datosDelItem = datosElegidos; // Le ponemos el alma
                scriptVisual.ActualizarUI(); // Le decimos que se dibuje
            }
            // -------------------------------------

            itemsActuales[i] = nuevoItem;
        }

        Debug.Log("Tienda refrescada con nuevos ítems.");
    }
    public void ComprarItemEnSlot(int indiceSlot)
    {
        // ...
        GameObject itemAComprar = itemsActuales[indiceSlot];

        if (itemAComprar != null)
        {
            ItemTienda scriptItem = itemAComprar.GetComponent<ItemTienda>();
            if (scriptItem != null)
            {
                // Ahora esto vuelve a funcionar porque devuelve bool
                bool compraExitosa = scriptItem.ComprarItem();

                if (compraExitosa)
                {
                    Destroy(itemAComprar);
                    itemsActuales[indiceSlot] = null;
                }
            }
        }
    }

    public void RerollTienda()
    {
        ColorPieza jugadorActual = GameManager.instance.TurnoActual;
        if (MoneyManager.instance.GetDinero(jugadorActual) >= costoReroll)
        {
            MoneyManager.instance.GastarDinero(jugadorActual, costoReroll);
            RefrescarTienda();
        }
    }
}