using UnityEngine;

// ESTE SÍ SE PUEDE CREAR EN UNITY
// La línea de abajo es la que hace aparecer la opción en el menú
[CreateAssetMenu(fileName = "ItemEscudo", menuName = "Tienda/Escudo")]
public class ItemEscudo : ItemData // Aquí dice: "Yo soy un ItemData, pero con funciones propias"
{
    [Header("Configuración Específica del Escudo")]
    public int durabilidad = 1;

    public override void EjecutarEfecto(AtributosPieza piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            Debug.Log($"Escudo aplicado a {piezaObjetivo.name}");
            piezaObjetivo.ActivarEscudoEnPieza();
        }
        else
        {
            Debug.LogWarning("¡No seleccionaste ninguna pieza!");
        }
    }
}