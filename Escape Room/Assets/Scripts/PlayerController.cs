using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool alive;
    private static bool playerExists;
    //public CheckpointManager checkpointManager;

    [Header("Horizontal Movement Settings")]
    Rigidbody2D rb;
    public float speed;
    int direction = 1;
    float originalXScale;

    [Header("Vertical Movement Settings")]
    public float jumpForce;
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float rememberGroundedFor;
    float lastTimeGrounded;
    public float boing;
    public float boingLength;
    public float boingCount;

    [Header("Graphics")]
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private int jumper;
    private float walker;
    //public ParticleSystem dust;

    //[Header("Inventory")]
    //public static int collectedAmount = 0;
    //public Text collectedText;

    [Header("Audio")]
    AudioSource runSound;
    private bool moving;
    //public GameObject jumpSound;

    [Header("Damage & Death")]
    public float knockback;
    public float knockbackLength;
    public float knockbackCount;
    public bool knockFromRight;
    private bool coolDown;
    public bool death = false;
    public bool rot = false;

    [Header("Work in Progress")]
    public int defaultAdditionalJumps = 2;
    int additionalJumps;
    public Color colorToTurnTo = Color.white;


    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        //checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalXScale = transform.localScale.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        anim.SetFloat("verticalVel", 0);
        //runSound = GetComponent<AudioSource>();
        anim.SetBool("idle", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            Move();
            CheckIfGrounded();
            if (isGrounded)
            {
                jumper = 0;
                Jump();
                BetterJump();
            }
            else
            {
                if (rb.velocity.y > 0)
                {
                    jumper = 1;
                }
                else if (rb.velocity.y < 0)
                {
                    jumper = 0;
                }
            }
            if (Input.GetAxisRaw("Horizontal") * direction > 0f)
            {
                FlipCharacterDirection();
            }
            anim.SetFloat("verticalVel", jumper);
            walker = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
            //if (Health <= 0)
            //{
            //    StartCoroutine(KillPlayer());
            //}
        }
        //anim.SetBool("idle", false);
        //collectedText.text = "Collected: " + collectedAmount;
    }


    void Move()
    {
        if (knockbackCount <= 0 && boingCount <= 0)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float moveBy = x * speed;
            rb.velocity = new Vector2(moveBy, rb.velocity.y);
            anim.SetFloat("horizontalVel", walker);
            anim.SetBool("moving", moving);
            if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
            {
                if (isGrounded)
                {
                    moving = true;
                }
                else
                {
                    moving = false;
                }
            }
            else
            {
                moving = false;
            }

            //if (moving)
            //{
            //    if (!runSound.isPlaying)
            //    {
            //        runSound.Play();
            //    }
            //}
            //else
            //{
            //    runSound.Stop();
            //}
        }
        else if (knockbackCount > 0)
        {
            if (knockFromRight)
            {
                rb.velocity = new Vector2(-knockback, knockback);
            }
            if (!knockFromRight)
            {
                rb.velocity = new Vector2(knockback, knockback);
            }
            knockbackCount -= Time.deltaTime;
        }
        if (boingCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, boing);
            boingCount -= Time.deltaTime;
        }
    }

    void FlipCharacterDirection()
    {
        //Turn the character by flipping the direction
        direction *= -1;

        //Record the current scale
        Vector3 scale = transform.localScale;

        //Set the X scale to be the original times the direction
        scale.x = originalXScale * direction;

        //Apply the new scale
        transform.localScale = scale;

        //if (isGrounded)
        //{
        //    CreateDust();
        //}

        if (transform.localScale.x >= 0)
        {
            knockFromRight = false;
        }
        else if (transform.localScale.x <= 0)
        {
            knockFromRight = true;
        }
    }

    void Jump()
    {
        if (Input.GetKey("up") || Input.GetKey("w") && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0))
        {
            //CreateDust();
            //Instantiate(jumpSound, transform.position, Quaternion.identity);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps--;
            jumper = 1;
        }
    }

    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey("up") || !Input.GetKey("w"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
        if (colliders != null)
        {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
                anim.SetBool("grounded", true);
            }
            isGrounded = false;
        }
    }

    //void CreateDust()
    //{
    //    dust.Play();
    //}

    //public IEnumerator KillPlayer()
    //{
    //    float delaydeath = 0.5f;
    //    death = true;
    //    alive = false;
    //    anim.SetBool("dead", true);
    //    //playerCheck.SetActive(false);
    //    while (death)
    //    {
    //        rot = true;
    //        while (rot)
    //        {
    //            yield return new WaitForSeconds(delaydeath);
    //            rot = false;
    //        }
    //        //bugs.enabled = false;
    //        yield return new WaitForSeconds(delaydeath);
    //        death = false;
    //    }
    //    //this.enabled = false;
    //    yield return new WaitForSeconds(0.5f);
    //    checkpointManager.Respawn();
    //    GameController.Health = 10;
    //}

    void Tester()
    {
        //anim.Play("Jump(Down)");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            //GameController.Health = 1;
        }
        //if (Traps.damaged)
        //{
        //    spriteRenderer.material.color = colorToTurnTo;
        //} else
        //{
        //    spriteRenderer.material.color = Color.white;
        //}
    }
}
