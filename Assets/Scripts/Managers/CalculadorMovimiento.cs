using UnityEngine;
using System.Collections.Generic;

public class CalculadorMovimientos : MonoBehaviour
{
    [SerializeField] private GestorTablero gestorTablero;

    public List<Vector2Int> CalcularMovimientos(AtributosPieza pieza)
    {
        switch (pieza.tipo)
        {
            case TipoPieza.Peon:
                return MovimientosPeon(pieza);

            case TipoPieza.Torre:
                return MovimientosLinea(pieza, new Vector2Int[]
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                });

            case TipoPieza.Alfil:
                return MovimientosLinea(pieza, new Vector2Int[]
                {
                    new Vector2Int(1,1),
                    new Vector2Int(1,-1),
                    new Vector2Int(-1,1),
                    new Vector2Int(-1,-1)
                });

            case TipoPieza.Reina:
                return MovimientosLinea(pieza, new Vector2Int[]
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right,
                    new Vector2Int(1,1),
                    new Vector2Int(1,-1),
                    new Vector2Int(-1,1),
                    new Vector2Int(-1,-1)
                });

            case TipoPieza.Caballo:
                return MovimientosCaballo(pieza);

            case TipoPieza.Rey:
                return MovimientosRey(pieza);
        }

        return new List<Vector2Int>();
    }

    // =============================
    // MOVIMIENTO PEÓN (CORREGIDO)
    // =============================

    List<Vector2Int> MovimientosPeon(AtributosPieza pieza)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        int direccion = pieza.color == ColorPieza.BLANCO ? 1 : -1;

        // 1. Movimiento simple hacia adelante
        Vector2Int frente = pieza.posicionEnTablero + new Vector2Int(0, direccion);

        if (gestorTablero.EstaDentroDelTablero(frente) && gestorTablero.GetPiezaEn(frente) == null)
        {
            movimientos.Add(frente);

            // 2. Movimiento doble (Solo si es el primer movimiento de la pieza en toda la partida)
            if (!pieza.yaSeMovioEnPartida) // Usamos la variable que creamos en AtributosPieza
            {
                Vector2Int frenteDoble = pieza.posicionEnTablero + new Vector2Int(0, direccion * 2);

                // Verificamos que la casilla doble esté dentro del tablero y esté vacía
                // Nota: No hace falta revisar la casilla del medio porque ya entramos en el if anterior
                if (gestorTablero.EstaDentroDelTablero(frenteDoble) && gestorTablero.GetPiezaEn(frenteDoble) == null)
                {
                    movimientos.Add(frenteDoble);
                }
            }
        }

        // 3. Capturas diagonales
        Vector2Int diag1 = pieza.posicionEnTablero + new Vector2Int(1, direccion);
        Vector2Int diag2 = pieza.posicionEnTablero + new Vector2Int(-1, direccion);

        RevisarCaptura(pieza, diag1, movimientos);
        RevisarCaptura(pieza, diag2, movimientos);

        return movimientos;
    }

    // =============================
    // MOVIMIENTO LINEAL
    // =============================

    List<Vector2Int> MovimientosLinea(AtributosPieza pieza, Vector2Int[] direcciones)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        foreach (Vector2Int dir in direcciones)
        {
            Vector2Int pos = pieza.posicionEnTablero;

            while (true)
            {
                pos += dir;

                if (!gestorTablero.EstaDentroDelTablero(pos))
                    break;

                AtributosPieza piezaEn = gestorTablero.GetPiezaEn(pos);

                if (piezaEn == null)
                {
                    movimientos.Add(pos);
                }
                else
                {
                    if (piezaEn.color != pieza.color)
                        movimientos.Add(pos);

                    break;
                }
            }
        }

        return movimientos;
    }

    // =============================
    // MOVIMIENTO CABALLO
    // =============================

    List<Vector2Int> MovimientosCaballo(AtributosPieza pieza)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int[] saltos =
        {
            new Vector2Int(2,1),
            new Vector2Int(2,-1),
            new Vector2Int(-2,1),
            new Vector2Int(-2,-1),
            new Vector2Int(1,2),
            new Vector2Int(-1,2),
            new Vector2Int(1,-2),
            new Vector2Int(-1,-2)
        };

        foreach (Vector2Int salto in saltos)
        {
            Vector2Int destino = pieza.posicionEnTablero + salto;

            if (!gestorTablero.EstaDentroDelTablero(destino))
                continue;

            AtributosPieza piezaEn = gestorTablero.GetPiezaEn(destino);

            if (piezaEn == null || piezaEn.color != pieza.color)
                movimientos.Add(destino);
        }

        return movimientos;
    }

    // =============================
    // MOVIMIENTO REY
    // =============================

    List<Vector2Int> MovimientosRey(AtributosPieza pieza)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,1),
            new Vector2Int(-1,-1)
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int destino = pieza.posicionEnTablero + dir;

            if (!gestorTablero.EstaDentroDelTablero(destino))
                continue;

            AtributosPieza piezaEn = gestorTablero.GetPiezaEn(destino);

            if (piezaEn == null || piezaEn.color != pieza.color)
                movimientos.Add(destino);
        }

        return movimientos;
    }

    // =============================
    // UTILIDAD
    // =============================

    void RevisarCaptura(AtributosPieza pieza, Vector2Int posicion, List<Vector2Int> lista)
    {
        if (!gestorTablero.EstaDentroDelTablero(posicion))
            return;

        AtributosPieza piezaEn = gestorTablero.GetPiezaEn(posicion);

        if (piezaEn != null && piezaEn.color != pieza.color)
            lista.Add(posicion);
    }
}