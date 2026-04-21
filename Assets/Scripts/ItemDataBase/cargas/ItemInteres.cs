using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInteres", menuName = "Items/Interés Pasivo")]
public class ItemInteres : ItemData
{
    [Header("Configuración del Interés")]
    [Tooltip("Cargas necesarias para generar 1 moneda de oro por turno.")]
    public int cargasPorMoneda = 200;

    [Tooltip("Cargas que genera este item pasivamente cada turno.")]
    public int cargasPorTurno = 1;

    // Al equipar, nos aseguramos de que el indicador se muestre
    public override void EjecutarEfecto(AtributosPieza pieza)
    {
        this.usaSistemaDeCargas = true;
        Debug.Log($"<color=yellow>Item Interés equipado. Genera oro pasivo basado en tus cargas.</color>");
    }

    // Lógica que ocurre al inicio de cada turno
    public override void AlIniciarTurno(AtributosPieza pieza)
    {
        if (pieza == null || MoneyManager.instance == null) return;

        // 1. Sumamos la acumulación pasiva (el item te da +1 carga por tenerlo)
        MoneyManager.instance.SumarCarga(pieza.color, cargasPorTurno);

        // 2. Leemos el total de cargas actual
        int totalCargas = MoneyManager.instance.GetCargas(pieza.color);

        // 3. Calculamos cuánto oro nos toca (Interés)
        // Ejemplo: Si tienes 450 cargas y la tasa es 200 -> 450/200 = 2 monedas.
        int monedasGanadas = totalCargas / cargasPorMoneda;

        // 4. Damos el oro
        if (monedasGanadas > 0)
        {
            MoneyManager.instance.AnadirDinero(pieza.color, monedasGanadas);

            // IMPORTANTE: NO restamos las cargas. Se quedan ahí para seguir generando dinero.
            Debug.Log($"<color=green>¡Interés generado! +{monedasGanadas} oro (Capital: {totalCargas} cargas).</color>");
        }
    }
}