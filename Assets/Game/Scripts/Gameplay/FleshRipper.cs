using UnityEngine;
using static BehaviourPlus;

[RequireComponent(typeof(SpriteRenderer),typeof(Animator)), RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public partial class FleshRipper : MonoBehaviour_UM,IFixedUpdatable,ILateUpdatable, IHoverable,ISelectable, IPool
{
    [SerializeField] private RipperSO ripperSO;
    [Header("Punteros")]
    [SerializeField] private Transform Tbottom, Tforward;
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D col;
    private float frenzyTimer, stunTimer;
    private bool isGrounded, isVomiting;
    //public bool HasHead = true, HasArms = true, HasLegs = true;
    private bool _isPushed;
    private float _turnCoolDown;
    private int _savedDirection = 1;
    public static FleshRipper SelectedRipper;
    [SerializeField] private GameObject cancelNode;

    public void PoolStart()
    {
        gameManager.OnAA += AA;
        gameManager.OnAA2 += AA2;
        gameManager.RegisterRipper();
        cancelNode.SetActive(false);
    }

    public void PoolEnd()
    {
        gameManager.OnAA -= AA;
        gameManager.OnAA2 -= AA2;
    }

    public void OnFixedUpdate()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.fixedDeltaTime;
            return;
        }
        isGrounded = TRaycast(Tbottom, -transform.up);

        //Movimiento automático
        float speed = isVomiting? 0f : ripperSO.speed;

        //Choque contra pared
        bool hitWall = TRaycast(Tforward, transform.right);
        if (hitWall) transform.InvertAxis();
        //if (_isPushed) _savedDirection *= -1;
        //_turnCoolDown = 0.5f;

        //if (_turnCoolDown > 0) _turnCoolDown -= Time.fixedDeltaTime;

        //Frenzy
        if (frenzyTimer > 0)
        {
            frenzyTimer -= Time.fixedDeltaTime;
            bool hitCivilian = TRaycast(Tforward, transform.right * ripperSO.visionRange, GameManager.CivilianLayer);
            speed *= hitCivilian? ripperSO.frenzySpeed : ripperSO.speedMultiplier;
        }

        //No legs
        //if (!HasLegs) speed *= 0.5f; //nLegs == 0

        //audioManager.UpdateSound(rb.linearVelocity.x != 0 && isGrounded);
        Vector2 v = rb.linearVelocity;
        v.x = speed;
        //if (canJump) v.y = ripperData.jumpForce;
        //rb.gravityScale = ripperData.gravityScale * (v.y < 0f ? 1f : gameManager.fallScale);
        rb.linearVelocity = v;

        static RaycastHit2D TRaycast(Transform t, Vector2 direction, LayerMask layerMask = default) => Physics2D.Raycast(t.position, direction, GameManager.RayLength, layerMask == default? gameManager.groundLayer : layerMask);
    }

    public void OnLateUpdate()
    {
        anim.SetBool("IsGrounded", isGrounded);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.layer)
        {
            case GameManager.CivilianLayer:
                ConvertCivilian(col.transform);
                break;
            case GameManager.InstakillLayer:
                RipperDead();
                break;
        }
    }

    private void ConvertCivilian(Transform civilian)
    {
        Destroy(civilian.gameObject);
        gameManager.SpawnRipper(civilian);
    }

    public void RipperDead()
    {
        if (SelectedRipper == this)
        {
            SelectedRipper = null;
            uiManager.ClearUI();
        }
        Destroy(gameObject);
        gameManager.RipperDead();
    }

    public void EnterHover() { }
    public void ExitHover() { }
    
    public void Select()
    {
        switch (gameManager.selectionTarget)
        {
            case GameManager.SelectionTarget.None:
                //_lockedTarget = ripperToLock.transform;
                SelectedRipper = this;
                sp.material = gameManager.ripperSelectedMat;
                //uiManager.ConfigureBiologicalLimits(caster.CanCastVomit(), caster.CanCastSores(), true, true, caster.CanCastFrenzy());
                break;
            case GameManager.SelectionTarget.Ripper:
                ExecuteQuickCast();
                break;
            case GameManager.SelectionTarget.Limb:
                if (TryGetComponent(out FleshLimbs limbs))
                {
                    limbs.DetonateLimb(FleshLimbs.LimbType.Arms);
                }
                break;
        }
    }

    public void Deselect()
    {
        sp.material = gameManager.normalMat;
    }

    public void ApplyLegLoss()
    {
        //if (nLegs == 0) return;
        //nLegs = 0;
        Vector2 offset = col.offset;
        offset.y *= 0.75f;
        col.offset = offset;
        //Update Animator
    }

    public void ApplyKnockback(Vector2 force)
    {
        //if (!_isPushed) _savedDirection = _direction;
        _isPushed = true;
        stunTimer = 0.5f;
        rb.linearVelocity = new Vector2(0, Mathf.Min(rb.linearVelocity.y, 0));
        rb.AddForce(force, ForceMode2D.Impulse);

        force.x *= transform.right.x;
        //UpdateFacing();
    }

    /*void OnCollisionStay2D(Collision2D col)
    {
        if (_isPushed && stunTimer <= 0)
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                _isPushed = false;
                _direction = _savedDirection;
            }
        }
    }*/

    private void ExecuteQuickCast()
    {
        switch (gameManager.selectedSkill)
        {
            case 0:
                CastVomit();
                break;
            case 1:
                CastSores();
                break;
            case 3:
                RipperDead();
                break;
            case 4:
                CastFrenzy();
                break;
        }
    }

    public void AA()
    {
        if (SelectedRipper != null)
        {
            SelectedRipper.sp.material = gameManager.normalMat;
            SelectedRipper = null;
        }
    }

    public void AA2()
    {
        SelectedRipper = null;
    }


    public bool CanCastVomit() => !isVomiting;
    public void CastVomit()
    {
        if (!CanCastVomit()) return;
        isVomiting = true;
        gameManager.ModifyHP(-1);
        cancelNode.SetActive(true);
    }
    public void CancelVomit()
    {
        cancelNode.SetActive(false);
        isVomiting = false;
    }

    public bool CanCastFrenzy() => !isVomiting;
    public void CastFrenzy()
    {
        if (!CanCastFrenzy()) return;
        gameManager.ModifyHP(-3);
        frenzyTimer += ripperSO.frenzyDuration;
    }

    public bool CanCastSores() => !isVomiting && isGrounded;
    public void CastSores()
    {
        if (!CanCastSores()) return;
        gameManager.ModifyHP(-2);
        //anim.SetFloat("Llagas", _soresCastCount);
        anim.SetTrigger("Jump");
        ApplyKnockback(ripperSO.jumpForce);
        if (SelectedRipper == this)
        {
            //SelectThisUnit();
            uiManager.ResetSkillHighlight();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * GameManager.RayLength);
    }
#endif
}