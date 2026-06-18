using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Hacemos que el objeto mire siempre a la cámara
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            // Rotamos 180 grados porque el sprite suele mirar hacia atrás
            transform.Rotate(0f, 180f, 0f);
        }
    }
}