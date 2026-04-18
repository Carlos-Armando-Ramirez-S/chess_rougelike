using UnityEngine;

public class CrearCasillas : MonoBehaviour
{
    public GameObject CasillaPreFab;
    public int Ancho = 8;
    public int Alto = 8;

    public Material Negro;
    public Material Blanco;

    [Header("Centro del tablero (solo X y Z)")]
    public Vector3 posicionCentral = Vector3.zero;

    [Header("Altura del tablero")]
    public float alturaTablero = 0f;

    public GameObject[,] casillas;

    public void Crear()
    {
        Debug.Log("ALTURA TABLERO ACTUAL: " + alturaTablero);
        casillas = new GameObject[Ancho, Alto];

        for (int i = 0; i < Ancho; i++)
        {
            for (int x = 0; x < Alto; x++)
            {
                // Centramos correctamente el tablero
                float posicionX = i - (Ancho / 2.0f) + 0.5f;
                float posicionZ = x - (Alto / 2.0f) + 0.5f;

                Vector3 posicionCasilla = new Vector3(
                    posicionX + posicionCentral.x,
                    alturaTablero,   // 🔥 Ahora la altura se controla aquí
                    posicionZ + posicionCentral.z
                );

                GameObject casillaTemp = Instantiate(CasillaPreFab, posicionCasilla, Quaternion.identity);
                casillaTemp.name = "Casilla_" + i + "_" + x;
                casillaTemp.transform.parent = this.transform;

                Casilla scriptCasilla = casillaTemp.GetComponent<Casilla>();

                if (scriptCasilla != null)
                {
                    scriptCasilla.coordenada = new Vector2Int(i, x);

                    Renderer rendererCasilla = casillaTemp.GetComponent<Renderer>();

                    if (rendererCasilla != null)
                    {
                        Material materialAsignado = ((i + x) % 2 == 0) ? Negro : Blanco;

                        rendererCasilla.material = materialAsignado;
                        scriptCasilla.SetMaterialOriginal(materialAsignado);
                    }
                    else
                    {
                        Debug.LogError("El prefab de la casilla no tiene Renderer.");
                    }
                }
                else
                {
                    Debug.LogError("El prefab de la casilla no tiene script Casilla.");
                }

                casillas[i, x] = casillaTemp;
            }
        }

        Debug.Log("Tablero creado. Buscando GameManager...");

        if (GameManager.instance != null)
        {
            GameManager.instance.InicializarConTablero(this);
        }
        else
        {
            Debug.LogError("No se encontró GameManager en la escena.");
        }
    }
}
