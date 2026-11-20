using UnityEngine;

// Animasi objek: memutar objek pada sumbu Y
// (sesuai contoh skrip pada materi PDF)
public class RotateObject : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
}
