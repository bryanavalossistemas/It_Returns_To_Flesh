using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D))]
public class Laser2D : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EdgeCollider2D triggerCollider; 
    [SerializeField] private Transform firePoint; 
    [SerializeField] private float maxLength = 20f;
    
    [Tooltip("AQUÍ SOLO PON LAS PAREDES/SUELO. ¡No pongas al jugador!")]
    [SerializeField] private LayerMask obstacleLayers; 

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem beamParticles; 

    [Header("Configuración de Parpadeo")]
    [SerializeField] private float tiempoEncendido = 2f;   
    [SerializeField] private float tiempoApagado = 1.5f;   
    private bool isLaserActive = true; 

    // NUEVO: Guardamos la última longitud del láser para no volver loco al sistema de partículas
    private float lastDistance = -1f;

    void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (triggerCollider == null) triggerCollider = GetComponent<EdgeCollider2D>();

        triggerCollider.isTrigger = true; 
        triggerCollider.edgeRadius = 0.1f; 
        lineRenderer.useWorldSpace = false;
    }

    void Start()
    {
        StartCoroutine(RutinaParpadeo());
    }

    void Update()
    {
        if (isLaserActive)
        {
            ShootLaser();
        }
        else
        {
            // Apagamos el GameObject entero. Es la forma más segura y sin bugs.
            if (beamParticles != null && beamParticles.gameObject.activeSelf)
            {
                beamParticles.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator RutinaParpadeo()
    {
        while (true) 
        {
            isLaserActive = true;
            lineRenderer.enabled = true; 
            triggerCollider.enabled = true; 
            yield return new WaitForSeconds(tiempoEncendido); 

            isLaserActive = false;
            lineRenderer.enabled = false;
            triggerCollider.enabled = false; 
            yield return new WaitForSeconds(tiempoApagado); 
        }
    }

    private void ShootLaser()
    {
        Vector3 startPoint = firePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, firePoint.right, maxLength, obstacleLayers);
        Vector3 endPoint = hit.collider != null ? (Vector3)hit.point : startPoint + firePoint.right * maxLength;

        Vector2 localStart = transform.InverseTransformPoint(startPoint);
        Vector2 localEnd = transform.InverseTransformPoint(endPoint);
        
        lineRenderer.SetPosition(0, localStart);
        lineRenderer.SetPosition(1, localEnd);
        triggerCollider.points = new Vector2[] { localStart, localEnd };

        if (beamParticles != null)
        {
            // Encendemos el objeto de partículas si estaba apagado
            if (!beamParticles.gameObject.activeSelf)
            {
                beamParticles.gameObject.SetActive(true);
            }

            float distance = Vector3.Distance(startPoint, endPoint);

            // MAGIA AQUÍ: Solo modificamos las partículas si la longitud cambió por más de un milímetro.
            // Esto elimina el bug de "velocidad súper rápida" porque dejamos que Unity respire.
            if (Mathf.Abs(lastDistance - distance) > 0.01f)
            {
                beamParticles.transform.position = Vector3.Lerp(startPoint, endPoint, 0.5f);
                beamParticles.transform.right = endPoint - startPoint;
                
                var shape = beamParticles.shape;
                shape.radius = distance / 2f; 
                
                lastDistance = distance; // Guardamos la nueva longitud
            }
        }
    }
}