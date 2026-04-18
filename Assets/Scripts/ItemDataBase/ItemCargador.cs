using UnityEngine;

[CreateAssetMenu(fileName = "NewItemCargador", menuName = "Items/Cargador")]
public class ItemCargador : ItemData
{
    [Header("Configuración de Cargas")]
    [Tooltip("Cuántas cargas da al jugador por cada pieza capturada.")]
    public int cargasPorCaptura = 5;

    // Esta función es OBLIGATORIA porque hereda de ItemData.
    // Se ejecuta en el momento exacto en que compras el item en la tienda.
    public override void EjecutarEfecto(AtributosPieza pieza)
    {
        if (pieza != null)
        {
            // Le decimos a la pieza: "Activa tu capacidad de cargar y usa ESTE script como referencia"
            pieza.ActivarCargador(this);
        }
    }
}