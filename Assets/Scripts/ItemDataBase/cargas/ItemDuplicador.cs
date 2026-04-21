using UnityEngine;

[CreateAssetMenu(fileName = "NewItemDuplicador", menuName = "Items/Duplicador de Cargas")]
public class ItemDuplicador : ItemData
{
    public override void EjecutarEfecto(AtributosPieza pieza)
    {
        // No hace nada especial al equiparse, solo marca que usa cargas
    }

    public override void AlCapturarPieza(AtributosPieza atacante, int valorCapturada)
    {
        if (MoneyManager.instance != null)
        {
            int cargasGanadas = valorCapturada * 2;
            MoneyManager.instance.SumarCarga(atacante.color, cargasGanadas);
            Debug.Log($"<color=cyan>Item Duplicador: +{cargasGanadas} cargas (x2 del valor {valorCapturada}).</color>");
        }
    }
}