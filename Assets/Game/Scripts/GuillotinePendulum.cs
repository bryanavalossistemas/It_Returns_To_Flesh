using UnityEngine;

public class GuillotinePendulum : MonoBehaviour
{
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float swingSpeed = 2f;

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}