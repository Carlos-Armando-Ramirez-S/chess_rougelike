using UnityEngine;

public class Casilla : MonoBehaviour
{
    public Vector2Int coordenada;

    private Material materialOriginal;
    private Renderer miRenderer;

    void Awake()
    {
        miRenderer = GetComponent<Renderer>();
        // Ya NO guardamos el material aquí. Lo haremos después de que CrearCasillas lo asigne.
    }

    void OnMouseDown()
    {
        GameManager.instance.IntentarMoverA(this.gameObject);
    }

    public void Iluminar(Material materialResaltado)
    {
        miRenderer.material = materialResaltado;
    }

    public void Apagar()
    {
        // Volvemos al material original que nos pasó CrearCasillas
        miRenderer.material = materialOriginal;
    }

    // --- ¡MÉTODO CLAVE QUE DEBE EXISTIR! ---
    // Este método es llamado por CrearCasillas para guardar el color inicial de la casilla.
    public void SetMaterialOriginal(Material mat)
    {
        materialOriginal = mat;
    }
}