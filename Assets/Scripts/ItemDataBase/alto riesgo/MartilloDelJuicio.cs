using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewMartillo", menuName = "Items/High Stakes/Martillo del Juicio")]
public class ItemMartillo : ItemData
{
    [Header("Riesgo y Recompensa")]
    public int falloCritico = 1;
    public int jackpot = 20;
    public int oroJackpot = 15;

    public override void EjecutarEfecto(AtributosPieza pieza) { }

    // Esta es la función que llama GestorCombate
    public IEnumerator EjecutarEfectoCaptura(AtributosPieza atacante, AtributosPieza victima, System.Action<bool> callback)
    {
        bool capturaExitosa = true;

        // Llamamos al Gestor de Tiradas
        yield return GestorTiradas.instance.TirarD20((resultado) =>
        {
            if (resultado == falloCritico)
            {
                Debug.Log("<color=red>¡FALLO CRÍTICO! La pieza esquiva.</color>");
                capturaExitosa = false;
            }
            else if (resultado == jackpot)
            {
                Debug.Log("<color=yellow>¡JACKPOT! +15 Oro.</color>");
                MoneyManager.instance.AnadirDinero(atacante.color, oroJackpot);
            }
            else
            {
                Debug.Log($"Dado: {resultado}. Captura normal.");
            }
        });

        callback.Invoke(capturaExitosa);
    }
}