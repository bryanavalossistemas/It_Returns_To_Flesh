using System;
using UnityEditor.Rendering.Universal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FleshRipper : MonoBehaviour
{
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
         
    public static FleshRipper SelectedRipper { get; set; }

    private Rigidbody2D _rb;
    private LineRenderer _hitboxVisualizer;
    private int _direction = 1;
    private Collider2D _col;
    private BoxCollider2D _boxCollider;
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

    private void SetupHitboxVisualizer()
    {
        GameObject lineObj = new GameObject("HitboxVisualizer_Debug");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition =Vector3.zero;

        _hitboxVisualizer = lineObj.AddComponent<LineRenderer>();
        _hitboxVisualizer.startWidth = 0.05f;
        _hitboxVisualizer.endWidth = 0.05f;
        _hitboxVisualizer.positionCount = 5;
        _hitboxVisualizer.useWorldSpace = true;
        Material debugMat = new Material(Shader.Find("Sprites/Default"));
        _hitboxVisualizer.material = debugMat;
        _hitboxVisualizer.startColor = Color.green;
        _hitboxVisualizer.endColor = Color.green;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        CurrentSpeed = speed;
        SetupHitboxVisualizer();
        
    }

    private void Update()
    {
        DrawHitboxLines();
    }

    private void DrawHitboxLines()
    {
        if (_boxCollider == null || _hitboxVisualizer == null) return;
        Bounds bounds = _boxCollider.bounds;
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
        corners[2] = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        corners[4] = corners[0];

        _hitboxVisualizer.SetPositions(corners);
    }
    private void FixedUpdate()
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
    }

    public void SelectThisUnit()
    {
        if (SelectedRipper != null)
        {
            SelectedRipper.SetSelectedVisual(false);
        }

        SelectedRipper = this;

        SetSelectedVisual(true);

        UIManager.Instance.UpdateHealth(health, maxHealth);

        FleshCaster caster = GetComponent<FleshCaster>();

        if (caster != null)
        {
            UIManager.Instance.ConfigureBiologicalLimits(
                caster.CanCastVomit(),
                caster.CanCastSores(),
                true,
                true,
                caster.CanCastFrenzy()
            );
        }

        Debug.Log("flesh selected");
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_turnCoolDown <= 0 && (col.gameObject.CompareTag("Wall") || col.gameObject.GetComponent<FleshRipper>() != null))
        {
            _direction = _direction * -1;
            if (_isPushed) _savedDirection *= -1;
            _turnCoolDown = 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Civilian"))
        {
            if (IsFrenzied)
            {
                ConvertCivilian(col.gameObject);
            }
            else
            {
                ConvertCivilian(col.gameObject);
            }
        }
    }

    private void ConvertCivilian(GameObject civilian)
    {
        Vector3 spawnPosition = civilian.transform.position;
        Destroy(civilian);
        if (ripperPrefab != null)
        {
            Instantiate(ripperPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void ModifyHealth(int amount)
    {
        health += amount;
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;
        if (SelectedRipper == this)
        {
            UIManager.Instance.UpdateHealth(health, maxHealth);
        }
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (SelectedRipper == this)
        {
            SelectedRipper = null;
            UIManager.Instance.ClearUI();
        }

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
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (_isPushed && _stunTimer <= 0)
        {
            if (Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
            {
                _isPushed = false;
                _direction = _savedDirection;
            }
        }
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
                if (_isPushed) _savedDirection *= -1;
                _turnCoolDown = 0.5f;
            }

        }
    }
}
