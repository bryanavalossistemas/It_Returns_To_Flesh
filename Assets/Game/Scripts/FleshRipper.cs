using UnityEngine;
using static BehaviourPlus;

[RequireComponent(typeof(SpriteRenderer),typeof(Animator)), RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public class FleshRipper : MonoBehaviour, IHoverable,ISelectable
{
    [SerializeField] private RipperData ripperData;
    [Header("Stats")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private int health = 11;
    [SerializeField] private int maxHealth = 11;
    [SerializeField] private float frenzyVisionRange = 6f;
    [SerializeField] private float dashMultiplier = 3f;
    [SerializeField] private LayerMask civilianLayer;
    [SerializeField] private GameObject ripperPrefab;
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Sprite legLessSprite;
    private const float RayLength = 1f;
    [SerializeField] private LayerMask groundLayer;
         
    public static FleshRipper SelectedRipper { get; set; }

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb;
    private LineRenderer _hitboxVisualizer;
    private int _direction = 1;
    [SerializeField] private BoxCollider2D _boxCollider;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float CurrentSpeed { get; set; }
    public bool IsFrenzied { get; set; } = false;

    public bool HasHead { get; set; } = true;
    public bool HasArms { get; set; } = true;
    public bool HasLegs { get; set; } = true;
    private bool _isPushed = false;
    private float _turnCoolDown = 0f;
    private float _stunTimer = 0f;
    private int _savedDirection = 1;
    public int FacingDirection => _direction;
    
    public void SetSelectedVisual(bool selected)
    {
        if (mainSpriteRenderer == null) return;

        mainSpriteRenderer.color =
            selected ? selectedColor : normalColor;
    }

    void Awake()
    {
        CurrentSpeed = speed;
        //UpdateFacing();
    }

    void Start()
    {
        gameManager.RegisterRipper();
    }

    void FixedUpdate()
    {
        if (_turnCoolDown > 0) _turnCoolDown -= Time.fixedDeltaTime;

        if (_isPushed)
        {
            if (_stunTimer > 0) _stunTimer -= Time.fixedDeltaTime;
            return;
        }
        _rb.linearDamping = 0f;
        _rb.angularDamping = 0f;

        float appliedSpeed = CurrentSpeed;

        //Choque contra pared
        bool hitWall = TRaycast(transform, transform.right);
        if (hitWall)
        {
            _direction *= -1;
            UpdateFacing();
            if (_isPushed) _savedDirection *= -1;
            _turnCoolDown = 0.5f;
        }

        if (IsFrenzied)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * _direction, frenzyVisionRange, civilianLayer);
            if (hit.collider != null)
            {
                appliedSpeed *= dashMultiplier;
            }

        }
        
        if (!HasLegs)
        {
            appliedSpeed /= 1.5f;
        }
        _rb.linearVelocity = new Vector2(appliedSpeed * _direction, _rb.linearVelocity.y);

        RaycastHit2D TRaycast(Transform foot, Vector2 direction) => Physics2D.Raycast(foot.position, direction, RayLength, groundLayer);
    }

    public void SelectThisUnit()
    {
        if (SelectedRipper != null)
        {
            SelectedRipper.SetSelectedVisual(false);
        }

        SelectedRipper = this;

        SetSelectedVisual(true);

        uiManager.UpdateHealth(health, maxHealth);

        FleshCaster caster = GetComponent<FleshCaster>();

        if (caster != null)
        {
            //uiManager.ConfigureBiologicalLimits(caster.CanCastVomit(), caster.CanCastSores(), true, true, caster.CanCastFrenzy());
        }

        Debug.Log("flesh selected");
    }

    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (_turnCoolDown <= 0 && (col.gameObject.CompareTag("Wall") || col.gameObject.GetComponent<FleshRipper>() != null))
        {
            _direction = _direction * -1;
            if (_isPushed) _savedDirection *= -1;
            _turnCoolDown = 0.5f;
        }
    }*/

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Civilian"))
        {
            ConvertCivilian(col.gameObject);
        }
    }

    private void ConvertCivilian(GameObject civilian)
    {
        Vector3 spawnPosition = civilian.transform.position;
        Destroy(civilian);
        if (ripperPrefab != null)
        {
            GameObject newRipper =Instantiate(ripperPrefab, spawnPosition, Quaternion.identity);
        FleshRipper zombieStats = newRipper.GetComponent<FleshRipper>();           
        zombieStats.maxHealth = zombieStats.MaxHealth;
        if (zombieStats != null)
        {
            
            zombieStats.ModifyHealth(zombieStats.MaxHealth); 
        }
        }
    }

    public void ModifyHealth(int amount)
    {
        health += amount;
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;
        if (SelectedRipper == this)
        {
            uiManager.UpdateHealth(health, maxHealth);
        }
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die(){
    if (SelectedRipper == this){
        SelectedRipper = null;
        uiManager.ClearUI();
    }
    gameManager.DeadRipper();

    this.enabled = false;
    if (_rb != null) _rb.linearVelocity = Vector2.zero; _rb.bodyType = RigidbodyType2D.Kinematic;
    if (_boxCollider != null) _boxCollider.enabled = false;

    Animator animator = GetComponent<Animator>();
    if (animator != null)
    {
        animator.SetTrigger("Die");
    }
    else
    {
        Destroy(gameObject); 
    }
}
public void OnDeathAnimationComplete(){
    Destroy(gameObject);
}

    public void ApplyLegLoss()
    {
        if (!HasLegs) return;
        HasLegs = false;
        if (_boxCollider != null)
        {
            float originalHeight = _boxCollider.size.y;
            _boxCollider.offset = new Vector2(_boxCollider.offset.x, _boxCollider.offset.y - (originalHeight / 4f));
        }

        if (mainSpriteRenderer != null && legLessSprite != null)
        {
            mainSpriteRenderer.sprite = legLessSprite;
        }
        Debug.Log("no legs");
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (!_isPushed) _savedDirection = _direction;
        _isPushed = true;
        _stunTimer = 0.5f;
        _rb.linearVelocity = new Vector2(0, Mathf.Min(_rb.linearVelocity.y, 0));
        _rb.AddForce(force, ForceMode2D.Impulse);

        if (force.x > 0) _direction = 1;
        else if (force.x < 0) _direction = -1;
        //UpdateFacing();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (_isPushed && _stunTimer <= 0)
        {
            if (Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
            {
                _isPushed = false;
                _direction = _savedDirection;
            }
        }
        return;
        if (_turnCoolDown <= 0)
        {
            bool hitWall = false;

            foreach (ContactPoint2D contact in col.contacts)
            {
                if (_direction == 1 && contact.normal.x < -0.8f) hitWall = true;
                else if (_direction == -1 && contact.normal.x > 0.8f) hitWall = true;
            }

            if (hitWall || col.gameObject.CompareTag("Wall") || col.gameObject.GetComponent<FleshRipper>() != null)
            {
                _direction *= -1;
                UpdateFacing();
                if (_isPushed) _savedDirection *= -1;
                _turnCoolDown = 0.5f;
                if (mainSpriteRenderer != null) transform.localScale = new Vector3(_direction, 1f, 1f);
            }
        }
    }

    private void UpdateFacing()
    {
        transform.InvertAxis();
    }

    public void EnterHover()
    {
        uiManager.UpdateHealth(Health, MaxHealth);
    }

    public void ExitHover()
    {
        uiManager.ClearUI();
    }

    public void Select()
    {
Debug.Log("Clic detectado en el Ripper. Target actual: " + gameManager.selectionTarget + " | Habilidad: " + gameManager.selectedSkill);        switch (gameManager.selectionTarget)
        {
            case GameManager.SelectionTarget.None:
                //_lockedTarget = ripperToLock.transform;
                SelectThisUnit();
                break;
            case GameManager.SelectionTarget.Ripper:
                ExecuteQuickCast(GetComponent<FleshCaster>());
                break;
            case GameManager.SelectionTarget.Limb:
                if (TryGetComponent(out FleshLimbs limbs))
                {
                    limbs.DetonateLimb(FleshLimbs.LimbType.Arms);
                }
                break;
        }
    }

    private void ExecuteQuickCast(FleshCaster caster)
    {
        if (caster == null) return;
        switch (gameManager.selectedSkill)
        {
            case 0:
                if (caster.CanCastVomit()) caster.CastVomit();
                break;
            case 1:
                if (caster.CanCastSores()) caster.CastSores();
                break;
            case 3:
                Die();
                break;
            case 4:
                if (caster.CanCastFrenzy()) caster.CastFrenzy();
                break;
        }
    }

    public void Deselect()
    {
        
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position+transform.right * RayLength);
    }
#endif
}