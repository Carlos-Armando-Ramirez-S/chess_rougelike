using UnityEngine;

public class Tablerocript : MonoBehaviour
{
    private CrearCasillas crearCasillas;

    void Start()
    {
        crearCasillas = GetComponent<CrearCasillas>();

        if (crearCasillas != null)
        {
            Debug.Log("Script CrearCasillas encontrado. Generando tablero...");
            crearCasillas.Crear();
        }
        else
        {
            Debug.LogError("Error: No se encontró el script CrearCasillas en este GameObject. Asegúrate de que ambos scripts están en el mismo objeto.");
        }
    }
}
