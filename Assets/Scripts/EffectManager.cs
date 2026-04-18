using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Singleton
    public static EffectManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }
    #endregion

    // --- ESTADOS DE EFECTOS (Solo los que son globales) ---
    private bool tienePromocionGratuita = false;
    // La variable 'tieneEscudoActivo' ya no es necesaria aquí.

    // --- LÓGICA DE PROMOCIÓN (sin cambios) ---
    public void ActivarPromocionGratuita()
    {
        tienePromocionGratuita = true;
        Debug.Log("<color=red>EFECTO EJECUTADO: Promoción Gratuita ACTIVADA.</color>");
    }

    public bool UsarPromocionGratuita()
    {
        if (tienePromocionGratuita) { tienePromocionGratuita = false; return true; }
        return false;
    }

    // --- NUEVA LÓGICA DEL ESCUDO ---
    /// <summary>
    /// Activa el escudo en una pieza específica.
    /// </summary>
    /// <param name="piezaObjetivo"> La pieza que recibirá el escudo. </param>
    public void ActivarEscudo(GameObject piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            AtributosPieza scriptDeLaPieza = piezaObjetivo.GetComponent<AtributosPieza>();
            if (scriptDeLaPieza != null)
            {
                scriptDeLaPieza.ActivarEscudoEnPieza();
            }
            else
            {
                Debug.LogError("EffectManager: El objeto objetivo no tiene un script 'AtributosPieza'.");
            }
        }
        else
        {
            Debug.LogError("EffectManager: Se intentó activar un escudo en una pieza nula (no hay ninguna seleccionada).");
        }
    }

    public void ActivarMovimientoDoble(GameObject piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            AtributosPieza scriptDeLaPieza = piezaObjetivo.GetComponent<AtributosPieza>();
            if (scriptDeLaPieza != null)
            {
                scriptDeLaPieza.ActivarMovimientoDoble();
            }
            else
            {
                Debug.LogError("EffectManager: El objeto objetivo no tiene un script 'AtributosPieza' para Movimiento Doble.");
            }
        }
        else
        {
            Debug.LogError("EffectManager: Se intentó activar Movimiento Doble en una pieza nula (no hay ninguna seleccionada).");
        }
    }

    public void ActivarAtaqueFlanqueo(GameObject piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            AtributosPieza scriptDeLaPieza = piezaObjetivo.GetComponent<AtributosPieza>();
            if (scriptDeLaPieza != null)
            {
                scriptDeLaPieza.ActivarAtaqueFlanqueo();
            }
            else
            {
                Debug.LogError("EffectManager: El objeto objetivo no tiene un script 'AtributosPieza' para Ataque Flanqueo.");
            }
        }
        else
        {
            Debug.LogError("EffectManager: Se intentó activar Ataque Flanqueo en una pieza nula (no hay ninguna seleccionada).");
        }
    }

    /// <summary>
    /// Activa el efecto Estatuadorada en una pieza específica.
    /// </summary>
    /// <param name="piezaObjetivo"> La pieza que recibirá el efecto. </param>
    public void ActivarEstatuadorada(GameObject piezaObjetivo)
    {
        if (piezaObjetivo != null)
        {
            AtributosPieza scriptDeLaPieza = piezaObjetivo.GetComponent<AtributosPieza>();
            if (scriptDeLaPieza != null)
            {
                scriptDeLaPieza.ActivarEstatuadorada();
            }
            else
            {
                Debug.LogError("EffectManager: El objeto objetivo no tiene un script 'AtributosPieza' para Estatuadorada.");
            }
        }
        else
        {
            Debug.LogError("EffectManager: Se intentó activar Estatuadorada en una pieza nula (no hay ninguna seleccionada).");
        }
    }
}