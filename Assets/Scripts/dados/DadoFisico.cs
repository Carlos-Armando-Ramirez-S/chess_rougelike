using UnityEngine;
using System.Collections;
using System.Linq; // Necesario para ordenar la lista fácilmente

public class DadoFisico : MonoBehaviour
{
    private Rigidbody rb;
    private bool haParado = false;
    private int resultadoFinal = 0;
    private float tiempoInicioTirada;

    [Header("Configuración")]
    public float tiempoMaximoEspera = 8f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Lanzar(Vector3 posicion, float fuerza, float torque)
    {
        transform.position = posicion;
        transform.rotation = Random.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        haParado = false;
        resultadoFinal = 0;
        tiempoInicioTirada = Time.time;

        rb.WakeUp();
        rb.AddForce(Vector3.up * (fuerza * 0.5f), ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.value, Random.value, Random.value) * torque, ForceMode.Impulse);
    }

    void Update()
    {
        if (haParado) return;

        if (Time.time - tiempoInicioTirada > tiempoMaximoEspera)
        {
            haParado = true;
            resultadoFinal = Random.Range(1, 21);
            Debug.LogWarning("Dado tardó mucho. Forzando resultado: " + resultadoFinal);
            return;
        }

        if (rb.IsSleeping())
        {
            haParado = true;
            resultadoFinal = CalcularCaraSuperior();
            Debug.Log($"Resultado: {resultadoFinal}");
        }
    }

    int CalcularCaraSuperior()
    {
        // 1. Buscamos todos los hijos (las marcas que pusimos)
        Transform[] caras = GetComponentsInChildren<Transform>(false); // False para no incluirse a sí mismo si es posible, pero nos filtramos luego

        Transform caraGanadora = null;
        float maxAltura = -999f;

        // 2. Recorremos cada hijo
        foreach (Transform cara in transform)
        {
            // Ignoramos el padre si se coló
            if (cara == transform) continue;

            // 3. ¿Cuál está más alto en el mundo (eje Y)?
            if (cara.position.y > maxAltura)
            {
                maxAltura = cara.position.y;
                caraGanadora = cara;
            }
        }

        // 4. Intentamos leer el número del nombre
        if (caraGanadora != null)
        {
            int numero;
            // Intentamos convertir el nombre "20", "1", etc. a número
            if (int.TryParse(caraGanadora.name, out numero))
            {
                return numero;
            }
            else
            {
                Debug.LogError($"El objeto '{caraGanadora.name}' no tiene un nombre numérico. ¡Renómbralo como '20', '1', etc!");
                return 0;
            }
        }

        return 0;
    }

    public int GetResultado() => resultadoFinal;
    public bool EstaListo() => haParado && resultadoFinal > 0;
}