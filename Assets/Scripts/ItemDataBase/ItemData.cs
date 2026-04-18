using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Información Básica")]
    public string nombreItem = "Nombre del Ítem";
    public int costoItem = 100;
    public Sprite icono;

    // NUEVA LÍNEA:
    [TextArea(3, 10)] // Esto hace que en el Inspector se vea como una caja de texto grande
    public string descripcion = "Descripción del ítem.";

    public abstract void EjecutarEfecto(AtributosPieza piezaObjetivo);
}