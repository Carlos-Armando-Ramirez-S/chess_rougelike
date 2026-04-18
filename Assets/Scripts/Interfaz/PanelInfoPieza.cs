using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PanelInfoPieza : MonoBehaviour
{
    [Header("Referencias del Panel Principal")]
    [SerializeField] private GameObject panelContenedor;
    [SerializeField] private TMP_Text textoNombrePieza;

    [Header("Configuración de Lista")]
    [SerializeField] private Transform contenedorDeItems; // Donde se instanciarán los iconos (El Content del ScrollView)
    [SerializeField] private GameObject prefabSlotItem;   // El prefab que creaste en el paso 3

    void Start()
    {
        if (panelContenedor != null) panelContenedor.SetActive(false);
    }

    public void MostrarInfo(AtributosPieza pieza)
    {
        if (pieza == null)
        {
            OcultarInfo();
            return;
        }

        // 1. Activar panel principal
        if (panelContenedor != null) panelContenedor.SetActive(true);

        // 2. Nombre de la pieza
        if (textoNombrePieza != null) textoNombrePieza.text = $"{pieza.tipo} {pieza.color}";

        // 3. Limpiar items antiguos
        LimpiarContenedor();

        // 4. Generar items nuevos
        if (pieza.itemsEquipados.Count > 0)
        {
            foreach (ItemData item in pieza.itemsEquipados)
            {
                CrearSlotVisual(item);
            }
        }
    }

    void CrearSlotVisual(ItemData item)
    {
        if (prefabSlotItem == null || contenedorDeItems == null) return;

        // 1. Instanciar
        GameObject nuevoSlot = Instantiate(prefabSlotItem, contenedorDeItems);

        // 2. Buscar TODAS las imágenes (Padre e Hijos)
        Image[] todasLasImagenes = nuevoSlot.GetComponentsInChildren<Image>();

        bool imagenAsignada = false;

        foreach (Image img in todasLasImagenes)
        {
            // TRUCO: Si esta imagen NO es la del objeto raíz (transform), entonces es el icono hijo
            if (img.transform != nuevoSlot.transform)
            {
                // Encontramos la imagen hija (el icono)
                img.sprite = item.icono;
                img.SetNativeSize(); // Ajustar tamaño al sprite
                imagenAsignada = true;

                // Rompemos el bucle porque solo queríamos la primera imagen hija
                break;
            }
        }

        if (!imagenAsignada)
        {
            Debug.LogWarning("No se encontró una imagen HIJA para asignar el icono. ¿Tu prefab tiene hijos con Image?");
        }

        // 3. Buscar el Texto (igual que antes)
        TMPro.TMP_Text texto = nuevoSlot.GetComponentInChildren<TMPro.TMP_Text>();
        if (texto != null)
        {
            texto.text = item.nombreItem;
        }

        // 4. Reconstruir layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(contenedorDeItems.GetComponent<RectTransform>());
    }

    void LimpiarContenedor()
    {
        // Destruye todos los hijos (items anteriores) para no duplicar
        foreach (Transform hijo in contenedorDeItems)
        {
            Destroy(hijo.gameObject);
        }
    }

    public void OcultarInfo()
    {
        if (panelContenedor != null) panelContenedor.SetActive(false);
    }
}