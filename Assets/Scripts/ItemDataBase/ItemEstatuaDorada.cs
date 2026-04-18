using UnityEngine;

// Esto hace que aparezca en el menú Create -> Tienda -> Estatua
[CreateAssetMenu(fileName = "ItemEstatua", menuName = "Tienda/Estatua Dorada")]
public class ItemEstatua : ItemData
{
    public override void EjecutarEfecto(AtributosPieza piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            Debug.Log($"<color=purple>Estatua Dorada activada en {piezaObjetivo.name}</color>");

            // Llamamos a la función que ya tienes en AtributosPieza
            piezaObjetivo.ActivarEstatuadorada();
        }
        else
        {
            Debug.LogWarning("<color=orange>¡Debes seleccionar una pieza para usar la Estatua!</color>");
        }
    }
}