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
    
    [Header("Texturas e Iconos")]
    [SerializeField] private RenderTexture videoTexture;
    [SerializeField] private Texture iconoHabilidad;

    [Header("Tiempos de Intercalado (en segundos)")]
    [SerializeField] private float tiempoVideo = 3.0f;
    [SerializeField] private float tiempoIcono = 2.0f;

    void Start()
    {
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "LabMonitor.mp4");
        StartCoroutine(BucleMonitorVideo());
    }
    private IEnumerator BucleMonitorVideo()
    {
        while (true)
        {
            // --- MODO VIDEO ---
            monitorRawImage.texture = videoTexture;
            videoPlayer.Play();
            yield return new WaitForSeconds(tiempoVideo);

            // --- MODO ICONO HABILIDAD ---
            videoPlayer.Pause();
            if (iconoHabilidad != null) monitorRawImage.texture = iconoHabilidad;
            yield return new WaitForSeconds(tiempoIcono);
        }
    }
}