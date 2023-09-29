using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    public float MaxSpeed;
    public float jumpSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Player jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Stop player moving
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);

        //Direction of sprite
        if (Input.GetButtonDown("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Changing animation parameter while player is walking
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    private void FixedUpdate()
    {
        //Player moving
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max speed
        if (rigid.velocity.x >= MaxSpeed)   //Right Max Speed
            rigid.velocity = new Vector2(MaxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x <= MaxSpeed * (-1))   //Left Max Speed
            rigid.velocity = new Vector2(MaxSpeed * (-1), rigid.velocity.y);

        //Raycast for stopping player jumping animation
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null && rayHit.distance < 0.6f)
            {
                anim.SetBool("isJumping", false);
            }
        }
    }
}
