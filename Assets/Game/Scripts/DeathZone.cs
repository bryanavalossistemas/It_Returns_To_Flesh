using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        FleshRipper ripper = col.GetComponent<FleshRipper>();

        if (ripper != null)
        {
            Destroy(ripper.gameObject);
        }
    }
}