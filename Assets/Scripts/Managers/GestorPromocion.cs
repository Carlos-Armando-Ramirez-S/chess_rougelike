using UnityEngine;

public class GestorPromocion : MonoBehaviour
{
    // Definimos los costos para subir de nivel
    [Header("Costos de Promoción")]
    public int costoPeonATorre = 50;
    public int costoTorreACaballo = 100;
    public int costoCaballoAAlfil = 200;
    public int costoAlfilAReina = 400;
    public int costoReinaARey = 800;

    // --- NUEVA VARIABLE PARA PARTÍCULAS ---
    [Header("Efectos Visuales")]
    [Tooltip("Arrastra aquí tu prefab de partículas de promoción")]
    public GameObject prefabParticulasPromocion;
    // --------------------------------------

    // 1. Función automática al llegar al final del tablero
    public void VerificarPromocion(AtributosPieza pieza)
    {
        bool debePromocionar = false;

        if (pieza.color == ColorPieza.BLANCO && pieza.posicionEnTablero.y == 7) debePromocionar = true;
        if (pieza.color == ColorPieza.NEGRO && pieza.posicionEnTablero.y == 0) debePromocionar = true;

        if (debePromocionar)
        {
            IntentarPromocionar(pieza, esAutomaticoPorTablero: true);
        }
    }

    // 2. Función conectada al BOTÓN DE LA UI
    public void AlPulsarBotonPromocion()
    {
        if (GameManager.instance == null) return;

        AtributosPieza pieza = GameManager.instance.ObtenerPiezaSeleccionada();

        if (pieza == null)
        {
            Debug.LogWarning("Selecciona una pieza primero.");
            return;
        }

        IntentarPromocionar(pieza, esAutomaticoPorTablero: false);
    }

    // 3. Lógica centralizada de Promoción
    private void IntentarPromocionar(AtributosPieza pieza, bool esAutomaticoPorTablero)
    {
        if (pieza.tipo == TipoPieza.Rey)
        {
            Debug.Log("Esta pieza ya es un Rey, no puede subir más de nivel.");
            return;
        }

        int costo = ObtenerCostoPromocion(pieza.tipo);

        bool esGratuita = false;
        if (EffectManager.instance != null)
        {
            esGratuita = EffectManager.instance.UsarPromocionGratuita();
        }

        if (!esGratuita)
        {
            if (MoneyManager.instance == null)
            {
                Debug.LogError("No hay MoneyManager en la escena.");
                return;
            }

            if (MoneyManager.instance.GetDinero(pieza.color) >= costo)
            {
                MoneyManager.instance.GastarDinero(pieza.color, costo);
                Debug.Log($"<color=green>Promoción pagada: -{costo} de oro.</color>");
            }
            else
            {
                Debug.Log($"<color=red>Oro insuficiente. Necesitas {costo}.</color>");
                return;
            }
        }
        else
        {
            Debug.Log("<color=yellow>¡Promoción Gratuita usada!</color>");
        }

        PromocionarPieza(pieza);

        if (!esAutomaticoPorTablero && GameManager.instance != null)
        {
            GameManager.instance.LimpiarSeleccion();
        }
    }

    private int ObtenerCostoPromocion(TipoPieza tipoActual)
    {
        switch (tipoActual)
        {
            case TipoPieza.Peon: return costoPeonATorre;
            case TipoPieza.Torre: return costoTorreACaballo;
            case TipoPieza.Caballo: return costoCaballoAAlfil;
            case TipoPieza.Alfil: return costoAlfilAReina;
            case TipoPieza.Reina: return costoReinaARey;
            default: return 999999;
        }
    }

    // 5. La función visual y de datos
    private void PromocionarPieza(AtributosPieza pieza)
    {
        pieza.Promocionar();
        pieza.ActualizarValorOro();

        // Actualizar modelo
        Mesh nuevaMalla = InicializadorPiezas.instance.GetMallaPorTipo(pieza.tipo);

        if (nuevaMalla != null)
        {
            MeshFilter mf = pieza.GetComponent<MeshFilter>();
            if (mf != null) mf.mesh = nuevaMalla;
            else
            {
                MeshFilter mfHijo = pieza.GetComponentInChildren<MeshFilter>();
                if (mfHijo != null) mfHijo.mesh = nuevaMalla;
            }
        }

        // Actualizar colisionador
        MeshCollider col = pieza.GetComponent<MeshCollider>();
        if (col != null && nuevaMalla != null) col.sharedMesh = nuevaMalla;

        // --- NUEVA LÓGICA: INSTANCIAR PARTÍCULAS ---
        if (prefabParticulasPromocion != null)
        {
            // Creamos las partículas en la posición de la pieza
            GameObject efecto = Instantiate(prefabParticulasPromocion, pieza.transform.position, Quaternion.identity);

            // Opcional: Hacer que las partículas sigan a la pieza si el tablero se mueve
            efecto.transform.SetParent(pieza.transform);

            // Destruir el efecto después de un tiempo
            Destroy(efecto, 3f);
        }
        // -----------------------------------------

        Debug.Log($"<color=cyan>¡Pieza promocionada a {pieza.tipo}!</color>");
    }
}