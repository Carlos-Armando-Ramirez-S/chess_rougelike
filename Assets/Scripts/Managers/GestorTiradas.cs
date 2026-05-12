using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GestorTiradas : MonoBehaviour
{
    public static GestorTiradas instance;

    [Header("Referencias (Arrastra aquí)")]
    public DadoFisico prefabDado;       // Tu prefab del dado
    public Transform spawnPoint;        // El punto de spawn dentro de DiceStudio
    public RawImage pantallaDadoUI;     // La RawImage de tu Canvas

    [Header("Ajustes de Física")]
    [Tooltip("Fuerza hacia arriba para que rebote")]
    public float fuerzaLanzamiento = 3f;
    [Tooltip("Fuerza de giro (bájalo si gira mucho)")]
    public float torqueGiro = 20f;

    [Header("Tiempos")]
    [Tooltip("Tiempo máximo que el dado puede estar moviéndose")]
    public float tiempoMaximoEspera = 8f;
    [Tooltip("Pausa después de obtener el resultado")]
    public float pausaFinal = 1.5f;

    void Awake()
    {
        instance = this;
    }

    public IEnumerator TirarD20(System.Action<int> callback)
    {
        // 1. Mostramos la interfaz
        pantallaDadoUI.gameObject.SetActive(true);

        // 2. Creamos el dado en el estudio secreto
        DadoFisico nuevoDado = Instantiate(prefabDado, spawnPoint.position, Random.rotation);

        // Configuramos la capa para que la cámara lo vea
        nuevoDado.gameObject.layer = LayerMask.NameToLayer("DiceUI");

        // 3. Lanzamos el dado con los valores ajustados
        nuevoDado.Lanzar(spawnPoint.position, fuerzaLanzamiento, torqueGiro);

        // 4. Esperamos a que termine
        yield return new WaitUntil(() => nuevoDado.EstaListo());

        int resultado = nuevoDado.GetResultado();
        Debug.Log($"Resultado obtenido: {resultado}");

        // 5. Pausa dramática
        yield return new WaitForSeconds(pausaFinal);

        // 6. Limpieza
        Destroy(nuevoDado.gameObject);
        pantallaDadoUI.gameObject.SetActive(false);

        // 7. Devolver resultado al item
        callback.Invoke(resultado);
    }
}