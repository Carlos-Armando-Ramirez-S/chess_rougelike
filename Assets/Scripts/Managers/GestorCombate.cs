using UnityEngine;
using System.Collections;       // <--- NECESARIA para Corrutinas (IEnumerator normal)
using System.Collections.Generic; // <--- NECESARIA para Listas (List<>)

public class GestorCombate : MonoBehaviour
{
    [SerializeField] private GestorTablero gestorTablero;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private GestorReyes gestorReyes;

    // --- NUEVA VARIABLE ---
    [Header("Referencias Visuales")]
    [Tooltip("Arrastra aquí el objeto 'Tablero' o 'Board' de tu escena")]
    [SerializeField] private Transform contenedorTablero;
    [SerializeField] private GameObject prefabParticulasCaptura;
    // ----------------------

    // CAMBIO: Ahora es una Corrutina (IEnumerator) para poder esperar el dado
    public IEnumerator ProcesarCaptura(AtributosPieza atacante, AtributosPieza capturada, System.Action<bool> callback = null)
    {
        // 1. Verificar Escudo
        if (capturada.IntentarUsarEscudo())
        {
            Debug.Log($"<color=cyan>¡{capturada.name} bloqueó el ataque con su ESCUDO!</color>");
            callback?.Invoke(false); // Avisamos que falló
            yield break; // Salimos
        }

        // --- NUEVO: CHECK DE ITEMS HIGH STAKES (DADO) ---
        bool capturaCancelada = false;

        if (atacante.itemsEquipados != null)
        {
            foreach (ItemData item in atacante.itemsEquipados)
            {
                // Si es el Martillo, ejecutamos la tirada
                if (item is ItemMartillo martillo)
                {
                    bool exito = true;
                    // Ejecutamos la lógica del dado y ESPERAMOS (yield return)
                    yield return StartCoroutine(martillo.EjecutarEfectoCaptura(atacante, capturada, (res) => exito = res));

                    if (!exito)
                    {
                        Debug.Log("<color=red>El dado falló. La captura se cancela.</color>");
                        capturaCancelada = true;
                        break; // Salimos del foreach
                    }
                }
            }
        }

        // Si el dado canceló, no hacemos nada más
        if (capturaCancelada)
        {
            callback?.Invoke(false);
            yield break;
        }
        // ---------------------------------------------

        // --- LÓGICA NORMAL DE CAPTURA (Tu código original) ---
        float danio = ObtenerDanio(capturada);
        towerManager.AplicarDanio(danio);

        // Avisar a items normales (Cargador, etc)
        if (atacante != null && atacante.itemsEquipados != null)
        {
            foreach (ItemData item in atacante.itemsEquipados)
            {
                // Llamamos a la función normal para los items que no son dados
                if (!(item is ItemMartillo)) // Evitamos llamar doble al martillo
                {
                    item.AlCapturarPieza(atacante, capturada.valorOro);
                }
            }
        }

        int oroGanado = capturada.valorOro;
        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.AnadirDinero(atacante.color, oroGanado);
            Debug.Log($"{atacante.color} capturó un {capturada.tipo} y ganó {oroGanado} de oro.");
        }

        if (capturada.tipo == TipoPieza.Rey)
        {
            if (gestorReyes != null) gestorReyes.ReyCapturado(capturada);
        }

        // Partículas
        if (prefabParticulasCaptura != null)
        {
            GameObject efecto = Instantiate(prefabParticulasCaptura);
            efecto.transform.position = capturada.transform.position + Vector3.up * 0.5f;
            if (contenedorTablero != null) efecto.transform.SetParent(contenedorTablero, true);
            efecto.transform.localScale = Vector3.one;
            Destroy(efecto, 3f);
        }

        // Items de la víctima al morir
        if (capturada.itemsEquipados != null)
        {
            List<ItemData> itemsVictima = new List<ItemData>(capturada.itemsEquipados);
            foreach (ItemData item in itemsVictima) item.AlMorir(capturada, atacante);
        }

        gestorTablero.EliminarPieza(capturada.posicionEnTablero);
        Destroy(capturada.gameObject);

        callback?.Invoke(true); // Avisamos que fue éxito
    }

    // ... (función ObtenerDanio igual) ...
    float ObtenerDanio(AtributosPieza pieza)
    {
        // ... (switch igual) ...
        switch (pieza.tipo)
        {
            case TipoPieza.Peon: return 1;
            case TipoPieza.Torre: return 3;
            case TipoPieza.Caballo: return 5;
            case TipoPieza.Alfil: return 8;
            case TipoPieza.Reina: return 10;
            case TipoPieza.Rey: return 15;
        }
        return 1;
    }
}