using UnityEngine;

public class GuillotineTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        FleshRipper ripper = other.GetComponent<FleshRipper>();

        if (ripper != null)
        {
            Debug.Log("Vida antes: " + ripper.Health);

            ripper.ModifyHealth(-9999);

            Debug.Log("Vida despues: " + ripper.Health);
        }
    }
}