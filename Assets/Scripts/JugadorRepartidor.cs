using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class JugadorRepartidor : MonoBehaviour
{
    [Header("Movimiento (Estilo Death's Door)")]
    public float velocidad = 8f;
    public float velocidadGiro = 10f;
    public float gravedad = -9.81f;

    [Header("UI En Construcción y Batería")]
    public GameObject panelEnConstruccion;
    public float tiempoPantalla = 2f;
    public Slider barraBateria;
    public float bateriaMaxima = 100f;

    private CharacterController controller;
    private Vector3 posicionInicial;
    private Vector3 velocidadVertical;
    private float bateriaActual;
    private bool bloqueado = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        posicionInicial = transform.position;
        bateriaActual = bateriaMaxima;

        if (barraBateria != null)
        {
            barraBateria.maxValue = bateriaMaxima;
            barraBateria.value = bateriaActual;
        }

        if (panelEnConstruccion != null)
            panelEnConstruccion.SetActive(false);
    }

    void Update()
    {
        if (bloqueado) return;

        MoverPersonaje();
    }

    void MoverPersonaje()
    {
        // Usamos solo WASD para que las Flechas queden libres para cambiar la cámara
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        // Toma la dirección hacia donde mira la cámara actualmente (recto o hacia la calle izquierda)
        Transform cam = Camera.main.transform;
        Vector3 adelante = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 derecha = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        Vector3 direccion = (derecha * x + adelante * z).normalized;

        if (direccion.magnitude >= 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);

            controller.Move(direccion * velocidad * Time.deltaTime);

            if (barraBateria != null)
            {
                bateriaActual = Mathf.Clamp(bateriaActual - 2f * Time.deltaTime, 0, bateriaMaxima);
                barraBateria.value = bateriaActual;
            }
        }

        if (controller.isGrounded && velocidadVertical.y < 0)
            velocidadVertical.y = -2f;

        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && !bloqueado)
        {
            StartCoroutine(SecuenciaEnConstruccion());
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemigo") && !bloqueado)
        {
            StartCoroutine(SecuenciaEnConstruccion());
        }
    }

    IEnumerator SecuenciaEnConstruccion()
    {
        bloqueado = true;

        if (panelEnConstruccion != null)
            panelEnConstruccion.SetActive(true);

        yield return new WaitForSeconds(tiempoPantalla);

        controller.enabled = false;
        transform.position = posicionInicial;
        controller.enabled = true;

        bateriaActual = bateriaMaxima;
        if (barraBateria != null) barraBateria.value = bateriaActual;

        if (panelEnConstruccion != null)
            panelEnConstruccion.SetActive(false);

        bloqueado = false;
    }
}