using UnityEngine;

[RequireComponent (typeof(FleshRipper))]
public class FleshCaster :MonoBehaviour
{
    public enum CastState { Normal, Vomiting }
    public CastState CurrentCastState { get; private set; } = CastState.Normal;
    [SerializeField] private GameObject cancelNodeObject;
    [SerializeField] private float frenzySpeedMultiplier = 2.5f;
    [SerializeField] private float soresJumpForceX = 10f;
    [SerializeField] private float soresJumpForceY = 5f;
    [SerializeField] float _sizeDetector;

    [SerializeField] LayerMask _groundMask;
    private FleshRipper _body;
    private float _savedPatrolSpeed;
    private float _frenzyTimer = 0f;
    private bool _isFrenzied = false;
    private int _soresCastCount = 0;
    public int SoresCastCount => _soresCastCount;
    private Animator _animator;
    Transform _floorDetector;
    bool _isGrounded;
    private void Awake()
    {
        _body = GetComponent<FleshRipper>();
        _floorDetector = transform.Find("FloorDetect");  
        if (cancelNodeObject !=null) cancelNodeObject.SetActive (false);
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isFrenzied)
        {
            _frenzyTimer -= Time.deltaTime;
            if (_frenzyTimer <= 0f)
            {
                EndFrenzy();
            }
        }
    }
     void FixedUpdate()

    {

       Collider2D floor = Physics2D.OverlapCircle(_floorDetector.position, _sizeDetector, _groundMask);

        _isGrounded = floor != null;

    }

    public void CastVomit()
    {
        if (CurrentCastState == CastState.Vomiting || _body.Health < 1) return;

        CurrentCastState = CastState.Vomiting;
        _savedPatrolSpeed = _body.CurrentSpeed;
        _body.CurrentSpeed = 0f;
        _body.ModifyHealth(-1);
        Debug.Log("vomit");

        if (cancelNodeObject != null) cancelNodeObject.SetActive (true);
    }

    public void CancelVomit()
    {
        if (CurrentCastState == CastState.Vomiting)
        {
            CurrentCastState = CastState.Normal;
            _body.CurrentSpeed = _savedPatrolSpeed;
            if (cancelNodeObject != null) cancelNodeObject.SetActive(false);
            Debug.Log("no vomit :c");
        }
    }
    public void CastFrenzy()
    {
        if (CurrentCastState == CastState.Vomiting || _body.Health < 1) return;
        _body.ModifyHealth(-3);
        _frenzyTimer += 3f;
        if(!_isFrenzied)
        {
            _isFrenzied = true;
            _body.IsFrenzied = true;
            _body.CurrentSpeed *= frenzySpeedMultiplier;
            Debug.Log("Flesh frenzied");
        }
    }

    private void EndFrenzy()
    {
        _isFrenzied = false;
        _body.IsFrenzied = false;
        _frenzyTimer = 0f;
        if (CurrentCastState == CastState.Normal)
        {
            _body.CurrentSpeed /= frenzySpeedMultiplier;
        }
        else
        {
            _savedPatrolSpeed /= frenzySpeedMultiplier;
        }
        Debug.Log("Flesh Calmed again...");
    }

    public bool CanCastVomit()
    {
        return CurrentCastState != CastState.Vomiting && _body.Health >= 1;
    }

    public bool CanCastFrenzy()
    {
        return CurrentCastState != CastState.Vomiting && _body.Health >=1;
    }

    public void CastSores()
    {
        if (CurrentCastState == CastState.Vomiting || _body.Health < 2 || _soresCastCount >= 3 || !_isGrounded) return;

        _soresCastCount++;
        _body.ModifyHealth(-2);
        if (_animator != null)
        {
            _animator.SetInteger("Llagas", _soresCastCount);
        }
        Vector2 jumpForce = new Vector2(soresJumpForceX * _body.FacingDirection, soresJumpForceY);
        _body.ApplyKnockback(jumpForce);
        if (_soresCastCount == 3)
        {
            FleshLimbs limbs = GetComponent<FleshLimbs>();
            if (limbs != null)
            {
                limbs.DetonateLimb(FleshLimbs.LimbType.Legs, false);
            }
            if (FleshRipper.SelectedRipper == _body && UIManager.Instance != null)
            {
                _body.SelectThisUnit();
                if (UIManager.Instance != null) UIManager.Instance.ResetSkillHighlight();
            }
        }
    }

    public bool CanCastSores()
    {
        
        return CurrentCastState != CastState.Vomiting && _body.Health >= 2 && _soresCastCount < 3 && _isGrounded;
    }
}
