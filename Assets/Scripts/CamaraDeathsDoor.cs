using UnityEngine;

public class CamaraDeathsDoor : MonoBehaviour
{
    public Transform objetivo; // Arrastra aquí a tu Player
    public Vector3 distanciaBase = new Vector3(0f, 12f, -10f); // Altura y distancia recta
    public float inclinacionX = 45f; // Ángulo hacia abajo tipo Death's Door
    public float suavizado = 5f;

    [Header("Vistazo con el Mouse (Requisito de clase)")]
    public float influenciaMouse = 1.5f;

    // Guarda hacia qué lado está mirando la cámara (0 = Recto, -90 = Calle Izquierda, 90 = Derecha, 180 = Atrás)
    private float anguloActualY = 0f;

    void Update()
    {
        // Cambiar la posición/orientación de la cámara SOLO al apretar las flechas
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anguloActualY = 0f; // Vista recta normal
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anguloActualY = -90f; // Gira la cámara para ver la calle de la izquierda
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anguloActualY = 90f; // Gira la cámara a la derecha
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anguloActualY = 180f; // Vista hacia atrás
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        // 1. Rotamos la distancia base según la flecha que hayas presionado
        Quaternion rotacionY = Quaternion.Euler(0f, anguloActualY, 0f);
        Vector3 offsetRotado = rotacionY * distanciaBase;

        // 2. Pequeño movimiento extra con el mouse para cumplir con la rúbrica
        float mouseX = Mathf.Clamp((Input.mousePosition.x / Screen.width) - 0.5f, -0.5f, 0.5f);
        float mouseY = Mathf.Clamp((Input.mousePosition.y / Screen.height) - 0.5f, -0.5f, 0.5f);
        Vector3 vistazoMouse = rotacionY * new Vector3(mouseX * influenciaMouse * 2f, 0f, mouseY * influenciaMouse * 2f);

        // 3. Mover y rotar la cámara suavemente a la nueva posición
        Vector3 posicionDeseada = objetivo.position + offsetRotado + vistazoMouse;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);

        Quaternion rotacionDeseada = Quaternion.Euler(inclinacionX, anguloActualY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, suavizado * Time.deltaTime);
    }
}