using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using static BehaviourPlus;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    private PhaseSO[] phases;
    private int phaseIndex;
    private RawImage leftImage, rightImage;
    [SerializeField] private float duration = 0.6f;

    public void StartLevel(int n)
    {
        uiManager.UpdateSkillsUI();

        phases = levels[n].phases;
        phaseIndex = 0;
        core.LoadScene(GetPhaseSO().sceneIndex);
    }

    private PhaseSO GetPhaseSO()
    {
        PhaseSO p = phases[phaseIndex];
        gameManager.phaseSO = p;
        return p;
    }

    public void NextPhase()
    {
        phaseIndex++;
        if (phaseIndex >= phases.Length)
        {
            core.LoadScene(SceneTypes.Menu);
            return;
        }
        GetPhaseSO();
        StartCoroutine(MakeTransition());
    }

    private IEnumerator MakeTransition()
    {
        Texture2D currentScreenshot = ScreenCapture();

        leftImage.texture = currentScreenshot;
        Texture2D preview;
        rightImage.texture = preview;

        // Ajustar escala de la preview según zoom
        float scale = nextLevel.previewZoom / Camera.main.orthographicSize;
        rightImage.rectTransform.localScale = Vector3.one * scale;

        canvas.alpha = 1;
        canvas.gameObject.SetActive(true);

        AsyncOperation load = SceneManager.LoadSceneAsync(phases[phaseIndex].sceneIndex);
        load.allowSceneActivation = false;

        float t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float progress = t / duration;

            Slide(progress);

            yield return null;
        }

        load.allowSceneActivation = true;

        while (!load.isDone)
            yield return null;


        canvas.gameObject.SetActive(false);


        Destroy(currentScreenshot);
    }


    private Texture2D ScreenCapture()
    {
        Texture2D tex = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();
        return tex;
    }
}