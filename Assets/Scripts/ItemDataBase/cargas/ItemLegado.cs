using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItemLegado", menuName = "Items/Legado")]
public class ItemLegado : ItemData
{
    [Header("Configuración Legado")]
    public int cargasAlMorir = 50;

    public override void EjecutarEfecto(AtributosPieza pieza)
    {
        // No hace nada especial al equiparse
    }

    public override void AlMorir(AtributosPieza piezaQueMuere, AtributosPieza atacante)
    {
        if (MoneyManager.instance == null) return;

        // 1. Dar las cargas al jugador dueño de la pieza que muere
        MoneyManager.instance.SumarCarga(piezaQueMuere.color, cargasAlMorir);
        Debug.Log($"<color=red>¡Legado activado! +{cargasAlMorir} cargas por la muerte de {piezaQueMuere.name}.</color>");

        // 2. Buscar una pieza del mismo tipo y color para pasar el item
        AtributosPieza[] todasLasPiezas = FindObjectsByType<AtributosPieza>(FindObjectsSortMode.None);
        List<AtributosPieza> candidatos = new List<AtributosPieza>();

        foreach (AtributosPieza p in todasLasPiezas)
        {
            // Condiciones: Mismo color, Mismo tipo, NO es la pieza que está muriendo
            if (p.color == piezaQueMuere.color && p.tipo == piezaQueMuere.tipo && p != piezaQueMuere)
            {
                candidatos.Add(p);
            }
        }

        // 3. Si encontramos candidato, pasamos el item
        if (candidatos.Count > 0)
        {
            AtributosPieza nuevoPortador = candidatos[Random.Range(0, candidatos.Count)];

            // Le equipamos este mismo item
            nuevoPortador.EquiparItem(this);

            Debug.Log($"<color=cyan>El item {nombreItem} ha pasado a {nuevoPortador.name}.</color>");
        }
        else
        {
            Debug.Log($"El item {nombreItem} no encontró heredero y se perdió.");
        }
    }
}