using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    // Esta variable guardará el modo de juego
    public bool esModoCPU = true;

    void Awake()
    {
        // Si ya existe una instancia, destruimos esta para no tener duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject); // ¡Este objeto sobrevive al cambiar de escena!
    }

    // Función para cambiar el modo desde otros scripts
    public void SetModoCPU(bool esCPU)
    {
        esModoCPU = esCPU;
    }
}