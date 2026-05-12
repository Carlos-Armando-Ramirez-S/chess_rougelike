using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    [Header("Referencias UI Principal (Animación)")]
    [Tooltip("Arrastra aquí el Panel principal de la tienda (El RectTransform que contiene todo)")]
    public RectTransform panelPrincipalRect;
    private CanvasGroup canvasGroup;

    [Header("Configuración Visual")]
    [Tooltip("Arrastra aquí tu ÚNICO prefab base (el molde vacío)")]
    public GameObject prefabBaseItem;

    [Tooltip("Los 3 espacios donde aparecerán los ítems en la UI")]
    public Transform[] slotsTienda;

    [Header("Datos de los Items")]
    [Tooltip("Arrastra aquí tus ScriptableObjects")]
    public List<ItemData> itemsDisponibles;

    [Header("Configuración del Reroll")]
    public int costoReroll = 5;

    [Header("Ajustes de Animación")]
    public float duracionAnimacion = 0.3f;

    public bool tiendaAbierta { get; private set; } // <--- NUEVA VARIABLE PÚBLICA

    private GameObject[] itemsActuales;
    private bool animando = false;

    void Start()
    {
        itemsActuales = new GameObject[slotsTienda.Length];

        // --- CONFIGURACIÓN INICIAL ---
        if (panelPrincipalRect != null)
        {
            canvasGroup = panelPrincipalRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panelPrincipalRect.gameObject.AddComponent<CanvasGroup>();

            // Empezamos invisibles
            canvasGroup.alpha = 0;
            panelPrincipalRect.localScale = Vector3.zero;
            panelPrincipalRect.gameObject.SetActive(false);
        }

        // CARGAMOS LOS ITEMS SOLO UNA VEZ AL INICIO
        RefrescarTienda();
        // ------------------------------
    }

    public void AbrirTienda()
    {
        if (animando) return;
        tiendaAbierta = true; // <--- NUEVO
        StartCoroutine(AnimarEntrada());
        // BORRAMOS: Time.timeScale = 0; 
    }

    public void CerrarTienda()
    {
        if (animando) return;
        tiendaAbierta = false; // <--- NUEVO
        StartCoroutine(AnimarSalida());
        // BORRAMOS: Time.timeScale = 1;
    }

    public void RefrescarTienda()
    {
        // 1. Destruir los ítems viejos
        for (int i = 0; i < itemsActuales.Length; i++)
        {
            if (itemsActuales[i] != null) Destroy(itemsActuales[i]);
        }

        // 2. Generar nuevos ítems
        for (int i = 0; i < slotsTienda.Length; i++)
        {
            if (itemsDisponibles.Count == 0) return;

            int indiceAleatorio = Random.Range(0, itemsDisponibles.Count);
            ItemData datosElegidos = itemsDisponibles[indiceAleatorio];

            GameObject nuevoItem = Instantiate(prefabBaseItem, slotsTienda[i]);
            nuevoItem.transform.localPosition = Vector3.zero;

            ItemTienda scriptVisual = nuevoItem.GetComponent<ItemTienda>();
            if (scriptVisual != null)
            {
                scriptVisual.datosDelItem = datosElegidos;
                scriptVisual.ActualizarUI();
            }

            itemsActuales[i] = nuevoItem;
        }
    }

    public void ComprarItemEnSlot(int indiceSlot)
    {
        GameObject itemAComprar = itemsActuales[indiceSlot];

        if (itemAComprar != null)
        {
            ItemTienda scriptItem = itemAComprar.GetComponent<ItemTienda>();
            if (scriptItem != null)
            {
                bool compraExitosa = scriptItem.ComprarItem();

                if (compraExitosa)
                {
                    Destroy(itemAComprar);
                    itemsActuales[indiceSlot] = null; // El slot queda vacío permanentemente hasta el reroll
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
            RefrescarTienda(); // AQUÍ SÍ SE REFRESCA, PORQUE PAGASTE
        }
    }

    // --- ANIMACIONES ---

    IEnumerator AnimarEntrada()
    {
        animando = true;
        panelPrincipalRect.gameObject.SetActive(true);
        // NOTA: Ya NO llamamos a RefrescarTienda aquí.

        float tiempo = 0f;
        panelPrincipalRect.localScale = Vector3.one * 0.5f;
        canvasGroup.alpha = 0f;

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / duracionAnimacion;

            float escala = Mathf.SmoothStep(0.5f, 1.1f, progreso);
            if (progreso > 0.8f) escala = Mathf.Lerp(1.1f, 1.0f, (progreso - 0.8f) * 5f);

            panelPrincipalRect.localScale = Vector3.one * escala;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progreso);

            yield return null;
        }

        panelPrincipalRect.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        animando = false;
    }

    IEnumerator AnimarSalida()
    {
        animando = true;
        float tiempo = 0f;
        float duracionSalida = duracionAnimacion * 0.5f;

        while (tiempo < duracionSalida)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / duracionSalida;

            panelPrincipalRect.localScale = Vector3.one * Mathf.Lerp(1f, 0.5f, progreso);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progreso);

            yield return null;
        }

        panelPrincipalRect.gameObject.SetActive(false);
        animando = false;
    }
}