using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject VVRed;
    [SerializeField]
    GameObject VVGreen;
    [SerializeField]
    GameObject VVYellow;
    [SerializeField]
    GameObject VVBlue;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    public LayerMask groundLayer;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;

    private void Start()
    {
        VVGreen.gameObject.SetActive(false);
        VVYellow.gameObject.SetActive(false);
        VVBlue.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        
        direction = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.E) && VVRed.gameObject.activeInHierarchy)
        {
            VVGreen.gameObject.SetActive(true);
            VVRed.gameObject.SetActive(false);
        }

       else if (Input.GetKeyDown(KeyCode.E) && VVGreen.gameObject.activeInHierarchy)
        {
            VVYellow.gameObject.SetActive(true);
            VVGreen.gameObject.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.E) && VVYellow.gameObject.activeInHierarchy)
        {
            VVBlue.gameObject.SetActive(true);
            VVYellow.gameObject.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.E) && VVBlue.gameObject.activeInHierarchy)
        {
            VVRed.gameObject.SetActive(true);
            VVBlue.gameObject.SetActive(false);
        }



        if (Input.GetButton("Jump") )
        {
            jumpTimer = Time.time + jumpDelay;
        }

    }
    private void FixedUpdate()
    {
        moveCharacter(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }
        modifyPhysics();
    }
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }

        
    }
    void Flip()
    {
        facingRight = !facingRight;
        VVRed.gameObject.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        VVGreen.gameObject.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        VVYellow.gameObject.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        VVBlue.gameObject.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
