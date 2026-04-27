using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Rigidbody2D playerRigidbody2D;

    //[Header("Speed Setup")]
    //public Vector2 friction = new Vector2(.1f, 0);
    //public float speed;
    //public float speedRun;
    //public float forceJump = 15;

    //[Header("Animation Setup")]
    ////public float jumpScaleY = 2f;
    ////public float jumpScaleX = .5f;
    ////public float animationDuration = 0.5f;

    //public SOFloat soJumpScaleY;
    //public SOFloat soJumpScaleX;
    //public SOFloat soAnimationDuration;

    //public Ease ease = Ease.OutBack;
    ////public float landingScaleY = .5f;
    ////public float landingScaleX = 2f;

    //public SOFloat soJumpLandingScaleY;
    //public SOFloat soJumpLandingScaleX;

    //[Header("Animation Player")]
    //public string boolRun = "Run";
    //public string boolJumpUp = "JumpUp";
    //public string boolJumpDown = "JumpDown";
    //public string boolJumpLanding = "JumpLanding";

    [Header("Setup")]
    public SOPlayerSetup soPlayerSetup;

    //public Animator animator;

    private Animator _currentPlayer;

    private Vector3 _regularScale;
    private Vector3 _jumpPoint;
    private float _currentSpeed;
    private bool _colided = false;
    private bool _isInTheGround = false;
    private bool _alreadyScaled = false;

    [Header("Jump Collision Check")]
    public Collider2D collider2D;
    public float distToGround;
    public float spaceToGround = .1f;
    public ParticleSystem jumpVFX;

    private void Awake()
    {
        _currentPlayer = Instantiate(soPlayerSetup.player, transform);

        if(collider2D != null)
        {
            distToGround = collider2D.bounds.extents.y;
        }
    }

    private void Start()
    {
        _regularScale = gameObject.transform.localScale;
    }

    void Update()
    {
        isGrounded();
        HandleJump();
        HandleMoviment();
        if (!_isInTheGround)
        {
            jumpState();
        }
        
    }

    public bool isGrounded()
    {
        Debug.DrawRay(transform.position, -Vector2.up, Color.magenta, distToGround+spaceToGround);
        return Physics2D.Raycast(transform.position, -Vector2.up, distToGround + spaceToGround);
    }

    private void HandleMoviment()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _currentSpeed = soPlayerSetup.speedRun;
            _currentPlayer.speed = 2;
        }
        else
        {
            _currentSpeed = soPlayerSetup.speed;
            _currentPlayer.speed = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerRigidbody2D.velocity = new Vector2(-_currentSpeed, playerRigidbody2D.velocity.y);

            if (gameObject.transform.rotation != new Quaternion(0, 180, 0, 0))
            {
                gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
            }

            _currentPlayer.SetBool(soPlayerSetup.boolRun, true);

            //playerRigidbody2D.MovePosition(playerRigidbody2D.position + velocity*Time.deltaTime);

            //playerRigidbody2D.velocity = new Vector2(Input.GetKey(KeyCode.LeftControl)  ? -speed : -speedRun, playerRigidbody2D.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerRigidbody2D.velocity = new Vector2(_currentSpeed, playerRigidbody2D.velocity.y);
            
            if(gameObject.transform.rotation != new Quaternion(0, 0, 0, 0))
            {
                gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            }

            _currentPlayer.SetBool(soPlayerSetup.boolRun, true);

            //playerRigidbody2D.MovePosition(playerRigidbody2D.position - velocity * Time.deltaTime);

            //playerRigidbody2D.velocity = new Vector2(Input.GetKey(KeyCode.LeftControl)  ? speed : speedRun, playerRigidbody2D.velocity.y);
        }
        else
        {
            _currentPlayer.SetBool(soPlayerSetup.boolRun, false);
        }

        if(playerRigidbody2D.velocity.x > 0)
        {
            playerRigidbody2D.velocity -= soPlayerSetup.friction;
        }
        else if (playerRigidbody2D.velocity.x < 0)
        {
            playerRigidbody2D.velocity += soPlayerSetup.friction;
        }
    }

    private void jumpState()
    {
        if (_jumpPoint != null)
        {
            if(_jumpPoint.y != gameObject.transform.position.y)
            {
                if (_jumpPoint.y < gameObject.transform.position.y)
                {
                    _currentPlayer.SetBool(soPlayerSetup.boolJumpUp, true);
                    _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
                    new WaitForEndOfFrame();
                }
                else if (_jumpPoint.y > gameObject.transform.position.y)
                {
                    _currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
                    _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, true);
                    new WaitForEndOfFrame();
                }
            }
            //Debug.Log("_jumpPoint.y = " + _jumpPoint.y);
            //Debug.Log("gameObject.transform.position.y = " + gameObject.transform.position.y);
            //Debug.Log("_jumpPoint.y < gameObject.transform.position.y = " + (_jumpPoint.y < gameObject.transform.position.y));
            //Debug.Log("_jumpPoint.y > gameObject.transform.position.y = " + (_jumpPoint.y > gameObject.transform.position.y));
        }

        _jumpPoint.y = gameObject.transform.position.y;
    }

    private void ScaleCharacter(float scaleY, float scaleX)
    {
        playerRigidbody2D.transform.DOScaleY(scaleY, soPlayerSetup.animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(soPlayerSetup.ease);
        playerRigidbody2D.transform.DOScaleX(scaleX, soPlayerSetup.animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(soPlayerSetup.ease).OnComplete(() => {
            // Quando a animação terminar, volte às restrições normais
            playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            _currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, false);
        }); ;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isInTheGround)
        {
            _jumpPoint = gameObject.transform.position;

            playerRigidbody2D.velocity = Vector2.up * soPlayerSetup.forceJump;
            playerRigidbody2D.transform.localScale = Vector2.one;

            DOTween.Kill(playerRigidbody2D.transform);

            HandleScaleJump();
            PlayJumpVFX();

            _isInTheGround = false;
            _colided = false;
        }
    }

    private void PlayJumpVFX()
    {
        VFXManager.Instance.PlayVFXByType(VFXManager.VFXType.JUMP, transform.position);

        //if(jumpVFX != null)
        //{
        //    jumpVFX.Play();
        //}
    }

    private void HandleScaleJump()
    {
        ScaleCharacter(soPlayerSetup.jumpScaleY, soPlayerSetup.jumpScaleX);
        //playerRigidbody2D.transform.DOScaleY(soJumpScaleY.value, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);
        //playerRigidbody2D.transform.DOScaleX(soJumpScaleX.value, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);
    }

    private void HandleLanding()
    {
        if (_colided)
        {
            //DOTween.Kill(playerRigidbody2D.transform);

            _currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
            _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
            _currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, true);

            HandleScaleLanding();
        }

        //DOTween.Kill(playerRigidbody2D.transform);
    }

    private void HandleScaleLanding()
    {
        ScaleCharacter(soPlayerSetup.landingScaleY, soPlayerSetup.landingScaleX);
        //playerRigidbody2D.transform.DOScaleY(soJumpLandingScaleY.value, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);
        //playerRigidbody2D.transform.DOScaleX(soJumpLandingScaleX.value, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);

        playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _isInTheGround) {
            return;
        }
        else if (collision.gameObject.CompareTag("Ground") && _isInTheGround == false)
        {

            _colided = true;
            _isInTheGround = true;

            //playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            HandleLanding();
        }

        Debug.Log("Entrou em contato");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Contato ativo");
        _currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
        _currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, false);
        if (!_alreadyScaled)
        {
            ScaleCharacter(_regularScale.y, _regularScale.x);
            //playerRigidbody2D.transform.DOScaleY(_regularScale.y, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);
            //playerRigidbody2D.transform.DOScaleX(_regularScale.x, soAnimationDuration.value).SetLoops(2, LoopType.Yoyo).SetEase(ease);
            _alreadyScaled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Sem contato");
        _alreadyScaled = false;
    }
}
