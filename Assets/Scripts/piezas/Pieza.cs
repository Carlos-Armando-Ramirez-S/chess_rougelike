using UnityEngine;

public class Pieza : MonoBehaviour
{
    void OnMouseDown()
    {
        AtributosPieza atributos = GetComponent<AtributosPieza>();
        GameManager.instance.SeleccionarPieza(atributos);
    }
}