using UnityEngine;

[RequireComponent (typeof(FleshRipper))]
public class FleshLimbs : MonoBehaviour
{
    public enum LimbType { Head, Arms, Legs }
    private FleshRipper _body;
    [SerializeField] private GameObject headObject;
    [SerializeField] private GameObject armsObject;
    [SerializeField] private GameObject legsObject;
    [SerializeField] private float baseExplosionRadius = 3f;
    [SerializeField] private LayerMask whatCanBePushed;
    [SerializeField] private float explosionForceX = 20f;
    [SerializeField] private float explosionForceY = 5f;

    private void Awake()
    {
        _body = GetComponent<FleshRipper>();
    }

    public void TriggerSkillVomit()
    {
        if (FleshRipper.SelectedRipper != null) FleshRipper.SelectedRipper.CastVomit();
    }

    void ODrawGizmos()
    {
        Gizmos.DrawSphere(headObject.transform.position, baseExplosionRadius);
    }
}
