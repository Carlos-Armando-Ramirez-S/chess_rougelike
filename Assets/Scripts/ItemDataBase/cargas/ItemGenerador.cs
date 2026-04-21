using UnityEngine;

[CreateAssetMenu(fileName = "NewItemGenerador", menuName = "Items/Generador Pasivo")]
public class ItemGenerador : ItemData
{
    [Header("Configuración del Generador")]
    [Tooltip("Cuántas cargas da al comprarlo/equiparlo.")]
    public int cargasIniciales = 50;

    [Tooltip("Cuántas cargas da al inicio de cada turno.")]
    public int cargasPorTurno = 1;

    // 1. EFECTO INMEDIATO AL COMPRAR
    public override void EjecutarEfecto(AtributosPieza pieza)
    {
        if (pieza == null || MoneyManager.instance == null) return;

        // Sumamos las cargas iniciales al jugador
        MoneyManager.instance.SumarCarga(pieza.color, cargasIniciales);
        
        Debug.Log($"<color=green>¡Batería equipada! +{cargasIniciales} cargas iniciales para {pieza.color}.</color>");
    }

    // 2. EFECTO PERIÓDICO (AUTOMÁTICO)
    // Sobrescribimos la función que añadimos en ItemData
    public override void AlIniciarTurno(AtributosPieza pieza)
    {
        if (pieza == null || MoneyManager.instance == null) return;

        // Sumamos las cargas de mantenimiento
        MoneyManager.instance.SumarCarga(pieza.color, cargasPorTurno);
        
        // Opcional: Mensaje sutil para no spamme la consola
        // Debug.Log($"{pieza.color} recibe +{cargasPorTurno} carga por turno.");
    }
}