using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private GameManager gm;
    private bool pensando = false;

    // Necesitamos acceso a tus gestores. Los buscaremos al inicio.
    private GestorTablero gestorTablero;
    private GestorTurnos gestorTurnos;
    private CalculadorMovimientos calculadorMovimientos;

    // En lugar de un tiempo fijo, usaremos un rango
    [Header("Tiempos de Reacción (Simulación Humana)")]
    public float tiempoMinimo = 1.0f; // Segundos mínimos de "pensamiento"
    public float tiempoMaximo = 6.0f; // Segundos máximos (si se pasa, pierde el bonus de oro)

    void Start()
    {
        gm = GameManager.instance;

        // Buscamos los componentes necesarios en el GameManager
        // Nota: Para que esto funcione, cambia [SerializeField] private por public
        // en las variables de tu GameManager, o usa propiedades públicas.
        // Aquí asumo que los hiciste públicos o serializados públicos.
        gestorTablero = gm.gestorTablero;
        gestorTurnos = gm.gestorTurnos;
        calculadorMovimientos = gm.calculadorMovimientos;
    }

    void Update()
    {
        // 1. SIEMPRE que sea turno de Blancas (Jugador), nos aseguramos de que el bloqueo se quite.
        if (gestorTurnos.TurnoActual == ColorPieza.BLANCO)
        {
            pensando = false;
            return; // Salimos, la IA no hace nada en turno blanco
        }

        // 2. Si es turno de Negras (IA) y NO estamos ya pensando, iniciamos el movimiento.
        // La condición 'pensando' actúa como candado para no entrar mil veces por segundo.
        if (gestorTurnos.TurnoActual == ColorPieza.NEGRO && !pensando)
        {
            StartCoroutine(PensarYMover());
        }
    }

    IEnumerator PensarYMover()
    {
        pensando = true;

        // --- NUEVO: TIEMPO ALEATORIO ---
        // Elegimos un tiempo aleatorio entre el mínimo y el máximo.
        // Ejemplo: Puede tardar 2 segundos (¡Bonus!) o 5 segundos (Sin Bonus).
        float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);

        Debug.Log($"CPU pensando... tiempo estimado: {tiempoEspera} segundos");

        yield return new WaitForSeconds(tiempoEspera);
        // -------------------------------

        // --- Lógica de búsqueda (igual que antes) ---

        AtributosPieza[] todasLasPiezas = FindObjectsOfType<AtributosPieza>();
        List<AtributosPieza> piezasNegras = new List<AtributosPieza>();
        foreach (AtributosPieza p in todasLasPiezas)
        {
            if (p.color == ColorPieza.NEGRO) piezasNegras.Add(p);
        }

        AtributosPieza mejorPieza = null;
        Vector2Int mejorMovimiento = new Vector2Int();
        int mejorValor = -9999;

        foreach (AtributosPieza pieza in piezasNegras)
        {
            List<Vector2Int> movimientos = calculadorMovimientos.CalcularMovimientos(pieza);

            foreach (Vector2Int mov in movimientos)
            {
                int valor = EvaluarMovimiento(pieza, mov);
                if (valor > mejorValor)
                {
                    mejorValor = valor;
                    mejorPieza = pieza;
                    mejorMovimiento = mov;
                }
            }
        }
        // -------------------------------------------

        if (mejorPieza != null)
        {
            gm.EjecutarMovimientoIA(mejorPieza, mejorMovimiento);
        }

        // No ponemos pensando = false aquí, el Update se encarga.
    }

    int EvaluarMovimiento(AtributosPieza pieza, Vector2Int destino)
    {
        int valor = 0;

        // ¿Podemos capturar?
        AtributosPieza piezaEnDestino = gestorTablero.GetPiezaEn(destino);
        if (piezaEnDestino != null && piezaEnDestino.color != pieza.color)
        {
            valor += ObtenerValorPieza(piezaEnDestino) * 10; // Alta prioridad a capturar
        }

        // Un poco de aleatoriedad para que no sea predecible
        valor += Random.Range(0, 5);

        return valor;
    }

    int ObtenerValorPieza(AtributosPieza p)
    {
        // IMPORTANTE: Asegúrate de que los nombres (Peon, Caballo, etc.) 
        // coincidan exactamente con cómo están escritos en tu script 'AtributosPieza' o 'TipoPieza'.
        // Si allí están en mayúsculas (PEON), cámbialos aquí a mayúsculas.

        switch (p.tipo)
        {
            case TipoPieza.Peon: return 1;
            case TipoPieza.Caballo:
            case TipoPieza.Alfil: return 3;
            case TipoPieza.Torre: return 5;
            case TipoPieza.Reina: return 9;
            default: return 0;
        }
    }
}