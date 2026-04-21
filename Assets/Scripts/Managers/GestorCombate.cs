using UnityEngine;
using System.Collections.Generic; // <--- AÑADE ESTO

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

    public bool ProcesarCaptura(AtributosPieza atacante, AtributosPieza capturada)
    {
        // ... (código del escudo igual) ...
        if (capturada.IntentarUsarEscudo())
        {
            Debug.Log($"<color=cyan>¡{capturada.name} bloqueó el ataque con su ESCUDO!</color>");
            return false;
        }

        // ... (código de daño y oro igual) ...
        float danio = ObtenerDanio(capturada);
        towerManager.AplicarDanio(danio);

        // --- NUEVO: AVISAR A LOS ITEMS ---
        if (atacante != null && atacante.itemsEquipados != null)
        {
            // Recorremos todos los items del atacante
            foreach (ItemData item in atacante.itemsEquipados)
            {
                // Les decimos: "Oye, capturaste una pieza que valía X oro"
                item.AlCapturarPieza(atacante, capturada.valorOro);
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

        // --- LÓGICA DE PARTÍCULAS CORREGIDA (MÉTODO SEGURO) ---
        if (prefabParticulasCaptura != null)
        {
            // 1. Instanciamos el objeto (aún sin preocuparnos del padre)
            GameObject efecto = Instantiate(prefabParticulasCaptura);

            // 2. Forzamos la posición EXACTA de la pieza capturada
            efecto.transform.position = capturada.transform.position + Vector3.up * 0.5f;

            // 3. Lo hacemos hijo del tablero (esto hará que se mueva con él)
            // El parámetro 'true' le dice a Unity: "Mantenlo en su posición mundial actual"
            if (contenedorTablero != null)
            {
                efecto.transform.SetParent(contenedorTablero, true);
            }

            // 4. Resetear escala local por si acaso
            efecto.transform.localScale = Vector3.one;

            Destroy(efecto, 3f);
        }
        // ---------------------------------------
        if (capturada.itemsEquipados != null)
        {
            // Creamos una lista temporal para evitar errores si el item modifica la lista original
            List<ItemData> itemsVictima = new List<ItemData>(capturada.itemsEquipados);
            foreach (ItemData item in itemsVictima)
            {
                item.AlMorir(capturada, atacante);
            }
        }
        gestorTablero.EliminarPieza(capturada.posicionEnTablero);
        Destroy(capturada.gameObject);

        return true;
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