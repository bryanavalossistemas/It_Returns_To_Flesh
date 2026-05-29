using UnityEditor.Rendering;
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

    public void DetonateLimb (LimbType limb, bool applyExplosionRadius = true)
    {
        float currentRadius = applyExplosionRadius ? baseExplosionRadius : 0f;
        float currentForceX = explosionForceX;
        float currentForceY = explosionForceY;
        int damageToTake = 4;
        Vector2 explosionCenter = transform.position;
        float radius = applyExplosionRadius ? currentRadius : 0f;
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(explosionCenter, radius, whatCanBePushed);
        if (limb == LimbType.Head && !_body.HasHead) return;
        if (limb == LimbType.Arms && !_body.HasArms) return;
        if (limb == LimbType.Legs && !_body.HasLegs) return;


        switch (limb)
        {
            case LimbType.Head:
                _body.HasHead = false;
                if (headObject != null) headObject.SetActive(false);
                currentRadius *= 1.5f;
                currentForceX *= 1.5f;
                currentForceY *= 1.5f;
                damageToTake = _body.MaxHealth;
                break;

            case LimbType.Arms:
                _body.HasArms = false;
                if (armsObject !=null) armsObject.SetActive(false);
                break;

            case LimbType.Legs:
                _body.HasLegs = false;
                if (legsObject !=null) legsObject.SetActive(false);
                _body.ApplyLegLoss();
                break;
        }
        switch (limb)
        {
            case LimbType.Head:
                if (headObject != null) explosionCenter = headObject.transform.position; 
                break;
            case LimbType.Arms:
                if (armsObject != null) explosionCenter = armsObject.transform.position;
                break;
            case LimbType.Legs:
                if (legsObject != null) explosionCenter = legsObject.transform.position;
                break;
        }

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

        _body.ModifyHealth(-damageToTake);
    }

    public void TriggerSkillVomit()
    {
        if (FleshRipper.SelectedRipper !=null)
        {
            FleshCaster caster = FleshRipper.SelectedRipper.GetComponent<FleshCaster>();
            if (caster != null)
            {
                caster.CastVomit();
            }
        }
    }

    void ODrawGizmos()
    {
        Gizmos.DrawSphere(headObject.transform.position, baseExplosionRadius);
    }
}
