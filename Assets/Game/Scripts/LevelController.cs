using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BehaviourPlus;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    private int[] phases;
    private GameObject[] wrappers;
    private int faseIndex;
    private float offsetX;
    private Coroutine changeFase;

    public void LoadLevel(int n)
    {
        phases = levels[n].phases;
        offsetX = 0f;
        wrappers = new GameObject[3]; //[0]=prev, [1]=current, [2]=next
        faseIndex = 0;
        SceneManager.sceneLoaded += OnSceneLoaded;
        core.ChangeScene(phases[0]);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.buildIndex == 0) return;
        GameObject cameraLevel = scene.GetRootGameObjects()[1];
        cameraLevel.GetComponentInChildren<CinemachineCamera>().Target.TrackingTarget = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject wrapper = new("_Wrapper");
        SceneManager.MoveGameObjectToScene(wrapper, scene);
        foreach (GameObject root in scene.GetRootGameObjects()) if (root != wrapper) root.transform.SetParent(wrapper.transform);

        if (loadMode is LoadSceneMode.Single)
        {
            wrappers[1] = wrapper;
            LoadNextFase();
        }
        else
        {
            wrapper.SetActive(false);
            wrappers[wrappers[2] == null ? 2 : 0] = wrapper;
            wrapper.transform.position = Vector2.right * offsetX;
        }
        offsetX += cameraLevel.GetComponent<BoxCollider2D>().size.x;
    }

    private void LoadNextFase(bool forward = true)
    {
        bool cond = forward ? faseIndex < phases.Length - 1 : faseIndex > 0;
        if (cond)
        {
            SceneManager.LoadSceneAsync(phases[faseIndex + (forward ? 1 : -1)], LoadSceneMode.Additive);
        }
    }

    public void NextFase()
    {
        if (faseIndex + 1 == phases.Length)
        {
            core.ChangeScene(0);
            return;
        }
        if (changeFase != null) return;
        changeFase = StartCoroutine(ChangeFase(true));
    }

    public void PrevFase()
    {
        if (changeFase != null) return;
        changeFase = StartCoroutine(ChangeFase(false));
    }

    private IEnumerator ChangeFase(bool forward)
    {
        int n1 = forward ? 0 : 2, n2 = forward ? 2 : 0;
        if (wrappers[n1] != null) SceneManager.UnloadSceneAsync(wrappers[n1].scene);
        wrappers[n1] = wrappers[1];
        wrappers[1] = wrappers[n2];
        wrappers[1].SetActive(true);
        wrappers[n2] = null;

        faseIndex += forward ? 1 : -1;
        LoadNextFase(forward);

        yield return new WaitForSeconds(2f);
        wrappers[n1].SetActive(false);
        changeFase = null;
    }
    //public Action OnWrapperPlaced, OnResetFase;
    //levelManager.OnResetFase();
}