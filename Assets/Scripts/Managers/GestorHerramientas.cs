using UnityEngine;
using TMPro;

public class GestorHerramientas : MonoBehaviour
{
    public static GestorHerramientas instance;

    [SerializeField] private GameObject panelTooltip;
    [SerializeField] private TMP_Text textoTitulo;
    [SerializeField] private TMP_Text textoDescripcion;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }

    void Start()
    {
        OcultarTooltip();
    }

    // --- ELIMINA O COMENTA ESTA FUNCIÓN UPDATE ENTERA ---
    /*
    void Update()
    {
        // Hacemos que el panel siga al ratón
        if (panelTooltip.activeSelf)
        {
            panelTooltip.transform.position = Input.mousePosition;
        }
    }
    */
    // ----------------------------------------------------

    public void MostrarTooltip(string titulo, string descripcion)
    {
        textoTitulo.text = titulo;
        textoDescripcion.text = descripcion;
        panelTooltip.SetActive(true);
    }

    public void OcultarTooltip()
    {
        panelTooltip.SetActive(false);
    }
}