using UnityEngine;

public class Casilla : MonoBehaviour
{
    public Vector2Int coordenada;

    private Material materialOriginal;
    private Renderer miRenderer;
    private GestorTablero gestorTablero; // Añadimos referencia al tablero

    void Awake()
    {
        miRenderer = GetComponent<Renderer>();
        // Buscamos el GestorTablero una sola vez al inicio
        gestorTablero = FindFirstObjectByType<GestorTablero>();
    }

    void OnMouseDown()
    {
        if (GameManager.instance == null || gestorTablero == null) return;

        // 1. Preguntamos: ¿Hay una pieza en esta casilla?
        AtributosPieza pieza = gestorTablero.GetPiezaEn(this.coordenada);

        if (pieza != null)
        {
            // 2. Si hay pieza, la seleccionamos.
            // Esto hace que al hacer clic en el suelo bajo la pieza, se seleccione la pieza.
            GameManager.instance.SeleccionarPieza(pieza);
        }
        else
        {
            // 3. Si NO hay pieza, intentamos movernos (lógica original)
            GameManager.instance.IntentarMoverA(this.gameObject);
        }
    }

    public void Iluminar(Material materialResaltado)
    {
        miRenderer.material = materialResaltado;
    }

    public void Apagar()
    {
        miRenderer.material = materialOriginal;
    }

    public void SetMaterialOriginal(Material mat)
    {
        materialOriginal = mat;
    }
}