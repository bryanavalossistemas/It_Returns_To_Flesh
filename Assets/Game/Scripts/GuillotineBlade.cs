using UnityEngine;

public class GuillotineBlade : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("TRIGGER TOCADO por: " + collider.name + " | Layer: " + LayerMask.LayerToName(collider.gameObject.layer));

        FleshRipper ripper = collider.GetComponent<FleshRipper>()
                          ?? collider.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            Debug.Log("Ripper encontrado → muriendo");
            ripper.RipperDead();
        }
        else Debug.Log("NO se encontró FleshRipper en: " + collider.name);
    }
}