using UnityEngine;
using System.Collections.Generic;

public class GestorTablero : MonoBehaviour
{
    [Header("Referencia al creador del tablero")]
    public CrearCasillas creador;

    private GameObject[,] casillas;

    private Dictionary<Vector2Int, AtributosPieza> registroDePiezas = new Dictionary<Vector2Int, AtributosPieza>();


    void Start()
    {
        // SOLUCIÓN: Solo asignar si casillas es null (es decir, si no nos configuraron antes).
        // Si ya tenemos casillas (pasadas por GameManager), nos quedamos con ellas.
        if (casillas == null && creador != null && creador.casillas != null)
        {
            casillas = creador.casillas;
        }
    }
    public void ConfigurarCasillas(GameObject[,] nuevasCasillas)
    {
        this.casillas = nuevasCasillas;
        Debug.Log("GestorTablero: Casillas configuradas correctamente.");
    }

    // ===============================
    // CONSULTAS DEL TABLERO
    // ===============================

    public bool EstaDentroDelTablero(Vector2Int posicion)
    {
        return posicion.x >= 0 &&
               posicion.x < 8 &&
               posicion.y >= 0 &&
               posicion.y < 8;
    }

    public AtributosPieza GetPiezaEn(Vector2Int posicion)
    {
        if (!EstaDentroDelTablero(posicion))
            return null;

        if (registroDePiezas.TryGetValue(posicion, out AtributosPieza pieza))
            return pieza;

        return null;
    }


    // ===============================
    // REGISTRO DE PIEZAS
    // ===============================

    public void RegistrarPieza(AtributosPieza pieza, Vector2Int posicion)
    {
        registroDePiezas[posicion] = pieza;
    }

    public void EliminarPieza(Vector2Int posicion)
    {
        if (registroDePiezas.ContainsKey(posicion))
            registroDePiezas.Remove(posicion);
    }

    public void MoverRegistro(Vector2Int posicionAnterior, Vector2Int nuevaPosicion)
    {
        if (!registroDePiezas.ContainsKey(posicionAnterior))
            return;

        AtributosPieza pieza = registroDePiezas[posicionAnterior];

        registroDePiezas.Remove(posicionAnterior);

        registroDePiezas[nuevaPosicion] = pieza;
    }


    // ===============================
    // ACCESO A CASILLAS
    // ===============================

    public GameObject GetCasilla(Vector2Int posicion)
    {
        if (!EstaDentroDelTablero(posicion))
            return null;

        return casillas[posicion.x, posicion.y];
    }


    // ===============================
    // UTILIDADES
    // ===============================

    public Dictionary<Vector2Int, AtributosPieza> ObtenerRegistroCompleto()
    {
        return registroDePiezas;
    }
}