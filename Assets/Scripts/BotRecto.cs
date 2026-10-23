using UnityEngine;

public class BotRecto : MonoBehaviour
{
    [Header("Configuración de Patrulla Recta")]
    public Vector3 direccionMovimiento = new Vector3(1f, 0f, 0f); 
    public float distancia = 8f;
    public float velocidad = 4f;

    private Vector3 puntoInicial;

    void Start()
    {
        puntoInicial = transform.position;
    }

    void Update()
    {
        float desplazamiento = Mathf.PingPong(Time.time * velocidad, distancia);
        transform.position = puntoInicial + (direccionMovimiento.normalized * desplazamiento);
    }
}