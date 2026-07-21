using UnityEngine;

public class CerebroAnimado : MonoBehaviour
{
    [Header("Efecto de Latido (Escala)")]
    [Tooltip("Qué tan rápido palpita el cerebro")]
    [SerializeField] private float velocidadLatido = 5f;
    [Tooltip("Qué tanto crece y se achica")]
    [SerializeField] private float tamañoLatido = 0.1f;

    [Header("Efecto de Flote (Movimiento)")]
    [Tooltip("Qué tan rápido se mueve de lado a lado")]
    [SerializeField] private float velocidadFlote = 2f;
    [Tooltip("Qué tanta distancia recorre hacia los lados")]
    [SerializeField] private float distanciaFlote = 0.2f;

    // Guardamos los valores originales para no deformar el objeto permanentemente
    private Vector3 escalaInicial;
    private Vector3 posicionInicial;

    void Start()
    {
        // Al iniciar, guardamos cómo era originalmente el cerebro
        escalaInicial = transform.localScale;
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // 1. Lógica del Latido (Agrandar y Achicar suavemente)
        // Mathf.Sin crea una ola que sube y baja entre -1 y 1 a lo largo del tiempo
        float factorEscala = Mathf.Sin(Time.time * velocidadLatido) * tamañoLatido;
        transform.localScale = escalaInicial + new Vector3(factorEscala, factorEscala, 0f);

        // 2. Lógica del Movimiento de lado a lado
        float factorMovimientoX = Mathf.Sin(Time.time * velocidadFlote) * distanciaFlote;
        
        // Aplicamos el movimiento sumándolo a su posición original
        transform.localPosition = posicionInicial + new Vector3(factorMovimientoX, 0f, 0f);
    }
}