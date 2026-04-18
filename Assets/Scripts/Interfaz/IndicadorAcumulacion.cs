using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necesario para detectar el ratón

// Añadimos las interfaces de ratón
public class IndicadorAcumulacion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias UI")]
    public GameObject contenedorVisual;
    public Image imagenRelleno;

    [Header("Configuración")]
    public int puntosPorIntervalo = 200;
    public int maximoAbsoluto = 1500;

    [Header("Colores")]
    public Color colorInicial = Color.blue;
    public Color colorMedio = Color.green;
    public Color colorAlto = Color.yellow;
    public Color colorMaximo = Color.red;
    public Color colorUltimo = new Color(1f, 0.5f, 0f);

    void Start()
    {
        if (contenedorVisual != null) contenedorVisual.SetActive(false);
    }

    public void VerificarYActualizar()
    {
        if (GameManager.instance == null || MoneyManager.instance == null) return;

        ColorPieza turnoActual = GameManager.instance.TurnoActual;
        bool tieneItemEquipado = false;

        AtributosPieza[] todasLasPiezas = FindObjectsByType<AtributosPieza>(FindObjectsSortMode.None);

        foreach (AtributosPieza pieza in todasLasPiezas)
        {
            if (pieza.color == turnoActual && pieza.tieneCargador)
            {
                tieneItemEquipado = true;
                break;
            }
        }

        if (tieneItemEquipado)
        {
            contenedorVisual.SetActive(true);
            int cargas = MoneyManager.instance.GetCargas(turnoActual);
            ActualizarVisual(cargas);
        }
        else
        {
            contenedorVisual.SetActive(false);
        }
    }

    void ActualizarVisual(int cantidadActual)
    {
        int intervalos = cantidadActual / puntosPorIntervalo;
        int sobrante = cantidadActual % puntosPorIntervalo;
        float porcentaje = (float)sobrante / puntosPorIntervalo;

        if (cantidadActual >= maximoAbsoluto) porcentaje = 1f;

        imagenRelleno.fillAmount = porcentaje;
        imagenRelleno.color = ObtenerColorPorIntervalo(intervalos);

        if (cantidadActual >= maximoAbsoluto)
        {
            float pulso = 1f + (Mathf.Sin(Time.time * 5f) * 0.1f);
            imagenRelleno.transform.localScale = Vector3.one * pulso;
        }
        else
        {
            imagenRelleno.transform.localScale = Vector3.one;
        }
    }

    Color ObtenerColorPorIntervalo(int intervalo)
    {
        if (intervalo < 1) return colorInicial;
        if (intervalo < 3) return colorMedio;
        if (intervalo < 5) return colorAlto;
        if (intervalo < 7) return colorMaximo;
        return colorUltimo;
    }

    // --- LÓGICA DE RATÓN (TOOLTIP) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ratón entró al indicador");
        // Al entrar el ratón, mostramos el tooltip con el número exacto
        if (GameManager.instance == null || MoneyManager.instance == null || GestorHerramientas.instance == null) return;

        ColorPieza turno = GameManager.instance.TurnoActual;
        int cantidad = MoneyManager.instance.GetCargas(turno);

        // Mostramos: Título "Cargas" y Descripción "Cantidad"
        GestorHerramientas.instance.MostrarTooltip("Cargas Acumuladas", cantidad.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Al salir el ratón, ocultamos el tooltip
        if (GestorHerramientas.instance != null)
        {
            GestorHerramientas.instance.OcultarTooltip();
        }
    }
}