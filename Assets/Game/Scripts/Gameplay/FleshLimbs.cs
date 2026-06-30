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

    private GameObject GetLimbObject(LimbType limb) => limb switch
    {
        LimbType.Head => headObject,
        LimbType.Arms => armsObject,
        LimbType.Legs => legsObject,
        _ => null
    };

    public void DetonateLimb (LimbType limb, bool applyExplosionRadius = true)
    {
        float currentForceX = explosionForceX;
        float currentForceY = explosionForceY;

        GameObject limbObj = GetLimbObject(limb);
        if (limbObj != null) limbObj.SetActive(false);

        switch (limb)
        {
            case LimbType.Head:
                currentForceX *= 1.5f;
                currentForceY *= 1.5f;
                break;
            case LimbType.Legs:
                _body.ApplyLegLoss();
                break;
        }

        Vector2 explosionCenter = limbObj != null ? (Vector2)limbObj.transform.position : (Vector2)transform.position;
        float radius = applyExplosionRadius ? baseExplosionRadius * (limb == LimbType.Head ? 1.5f : 1f) : 0f;
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(explosionCenter, radius, whatCanBePushed);

        System.Collections.Generic.HashSet<Rigidbody2D> pushedBodies = new System.Collections.Generic.HashSet<Rigidbody2D>();

        foreach (Collider2D col in objectsInRange)
        {
            if (col.gameObject.layer == 8)
            {
                Destroy(col.gameObject);
                continue;
            }
            Rigidbody2D rb = col.GetComponentInParent<Rigidbody2D>();
            if (rb !=null && rb.gameObject != gameObject && !pushedBodies.Contains(rb))
            {
                pushedBodies.Add(rb);
                float directionX = Mathf.Sign(rb.transform.position.x - explosionCenter.x);
                if (Mathf.Abs(rb.transform.position.x - explosionCenter.x) < 0.1f)
                {
                    directionX = Random.Range(0f, 1f) > 0.5f ? 1f : -1;
                }

                Vector2 finalForce = new Vector2(directionX * currentForceX, currentForceY);

                FleshRipper victim = rb.GetComponent<FleshRipper>();
                if (victim != null)
                {
                    victim.ApplyKnockback(finalForce);
                }
                else
                {
                    rb.AddForce(finalForce, ForceMode2D.Impulse);
                }
            }
        }

        //_body.ModifyHealth(-damageToTake);
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
