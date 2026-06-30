using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GuillotineBlade : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        LogTrigger(other);

        FleshRipper ripper = other.GetComponent<FleshRipper>()
                          ?? other.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            LogRipperFound();
            ripper.RipperDead();
        }
        else
        {
            LogRipperNotFound(other);
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void LogTrigger(Collider2D other) =>
        Debug.Log("TRIGGER TOCADO por: " + other.name + " | Layer: " + LayerMask.LayerToName(other.gameObject.layer));

    [Conditional("UNITY_EDITOR")]
    private static void LogRipperFound() =>
        Debug.Log("Ripper encontrado → muriendo");

    [Conditional("UNITY_EDITOR")]
    private void LogRipperNotFound(Collider2D other) =>
        Debug.Log("NO se encontró FleshRipper en: " + other.name);
}