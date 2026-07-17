using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoTutorialMonitor : MonoBehaviour
{
    [Header("Componentes de Video y UI")]
    [SerializeField] private RawImage monitorRawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AspectRatioFitter aspectFitter;

    [Header("Texturas e Iconos")]
    [SerializeField] private RenderTexture videoTexture;
    
    // 1. Cambiamos Texture por un arreglo de Texturas (Texture[])
    [SerializeField] private Texture[] iconosHabilidad; 

    [Header("Tiempos de Intercalado (en segundos)")]
    [SerializeField] private float tiempoVideo = 3.0f;
    [SerializeField] private float tiempoIcono = 2.0f;

    // 2. Variable para saber qué imagen de la lista nos toca mostrar
    private int indiceIconoActual = 0; 

    void Start()
    {
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "LabMonitor.mp4");
        
        if (aspectFitter == null) 
            aspectFitter = monitorRawImage.GetComponent<AspectRatioFitter>();

        StartCoroutine(BucleMonitorVideo());
    }

    private IEnumerator BucleMonitorVideo()
    {
        while (true)
        {
            // --- MODO VIDEO ---
            monitorRawImage.texture = videoTexture;
            ActualizarProporcion(videoTexture); 
            videoPlayer.Play();
            yield return new WaitForSeconds(tiempoVideo);

            // --- MODO ICONO HABILIDAD ---
            videoPlayer.Pause();
            
            // 3. Verificamos que el arreglo tenga al menos una imagen
            if (iconosHabilidad != null && iconosHabilidad.Length > 0) 
            {
                // Obtenemos la imagen actual usando nuestro índice
                Texture texturaActual = iconosHabilidad[indiceIconoActual];
                
                if (texturaActual != null)
                {
                    monitorRawImage.texture = texturaActual;
                    ActualizarProporcion(texturaActual); 
                }

                // 4. Aumentamos el índice. El "% iconosHabilidad.Length" hace que 
                // cuando llegue al final, vuelva automáticamente a 0.
                indiceIconoActual = (indiceIconoActual + 1) % iconosHabilidad.Length;
            }
            
            yield return new WaitForSeconds(tiempoIcono);
        }
    }

    private void ActualizarProporcion(Texture textura)
    {
        if (aspectFitter != null && textura != null)
        {
            aspectFitter.aspectRatio = (float)textura.width / textura.height;
        }
    }
}