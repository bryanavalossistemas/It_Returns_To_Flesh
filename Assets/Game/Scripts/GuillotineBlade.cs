using UnityEngine;

public class GuillotineBlade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER TOCADO por: " + other.name + " | Layer: " + LayerMask.LayerToName(other.gameObject.layer));

        FleshRipper ripper = other.GetComponent<FleshRipper>() 
                          ?? other.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            Debug.Log("Ripper encontrado → muriendo");
            ripper.Die();
        }
        else
        {
            Debug.Log("NO se encontró FleshRipper en: " + other.name);
        }
    }
}