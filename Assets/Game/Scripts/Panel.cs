using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI de Pausa")]
    [SerializeField] private GameObject pauseEffectPanel; // Arrastra aquí tu StaticPanel
    
    private bool isPaused = false;

    void Update()
    {
        // Detectamos si presiona la tecla Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Congela el juego por completo

        if (pauseEffectPanel != null)
        {
            pauseEffectPanel.SetActive(true); // Activa la estática de cámara vieja
        }
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Reanuda el tiempo normal

        if (pauseEffectPanel != null)
        {
            pauseEffectPanel.SetActive(false); // Apaga la estática
        }
    }
}