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
    [SerializeField] private AspectRatioFitter aspectFitter; // <- ¡Esta es la variable nueva!

    [Header("Texturas e Iconos")]
    [SerializeField] private RenderTexture videoTexture;
    [SerializeField] private Texture iconoHabilidad;

    [Header("Tiempos de Intercalado (en segundos)")]
    [SerializeField] private float tiempoVideo = 3.0f;
    [SerializeField] private float tiempoIcono = 2.0f;

    void Start()
    {
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "LabMonitor.mp4");
        
        // Buscamos el componente automáticamente si se te olvida arrastrarlo
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
            if (iconoHabilidad != null) 
            {
                monitorRawImage.texture = iconoHabilidad;
                ActualizarProporcion(iconoHabilidad); 
            }
            yield return new WaitForSeconds(tiempoIcono);
        }
    }

    // Método que ajusta las proporciones dinámicamente sin deformar la imagen
    private void ActualizarProporcion(Texture textura)
    {
        if (aspectFitter != null && textura != null)
        {
            aspectFitter.aspectRatio = (float)textura.width / textura.height;
        }
    }
}