using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float rotationSpeed = 200f;

    void Update()
    {
        if (Input.GetMouseButton(1)) // Click derecho
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
