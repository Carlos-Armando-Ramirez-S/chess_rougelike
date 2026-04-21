using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Información Básica")]
    public string nombreItem = "Nombre del Ítem";
    public int costoItem = 100;
    public Sprite icono;
    [TextArea(3, 10)]
    public string descripcion = "Descripción del ítem.";

    // Para el indicador de cargas
    public bool usaSistemaDeCargas = false;

    // Función principal al comprar
    public abstract void EjecutarEfecto(AtributosPieza piezaObjetivo);

    // --- FUNCIONES VIRTUALES (Eventos) ---

    // Se ejecuta al inicio del turno
    public virtual void AlIniciarTurno(AtributosPieza pieza) { }

    // Se ejecuta cuando esta pieza captura a otra
    public virtual void AlCapturarPieza(AtributosPieza atacante, int valorCapturada) { }

    // --- NUEVA FUNCIÓN: Se ejecuta cuando esta pieza muere ---
    public virtual void AlMorir(AtributosPieza piezaQueMuere, AtributosPieza atacante) { }
}