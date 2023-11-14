using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float MaxSpeed;
    public float jumpSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //Player jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("Jump");
        }

        //Stop player moving
        if (Input.GetButtonUp("Horizontal"))
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);

        //Direction of sprite
        if (Input.GetButton("Horizontal"))
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Interactive with Enemy
        if (collision.gameObject.CompareTag("Enemy")) {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //If Player steps on Enemy
                OnAttack(collision.transform);
            }
            else
                //When an enemy hits the player
                OnDamaged(collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            //Give Stage Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            PlaySound("Item");

            if (isBronze) gameManager.stagePoint += 50;
            else if (isSilver) gameManager.stagePoint += 100;
            else if (isGold) gameManager.stagePoint += 300;

            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            PlaySound("Finish");
            //next Stage
            gameManager.nextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        PlaySound("Attack");

        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();

        enemyMove.OnDamaged();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }

    void OnDamaged(Vector2 targetPos) {
        gameManager.healthDown();
        PlaySound("Damaged");

        gameObject.layer = 11;
        int dirc = targetPos.x - this.transform.position.x > 0 ? -1 : 1;

        rigid.AddForce(new Vector2(dirc, 1) * 10, ForceMode2D.Impulse);

        spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        anim.SetTrigger("isDamaged");

        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void Ondie() {
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        PlaySound("Die");
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void velocityZero()
    {
        this.rigid.velocity = Vector2.zero;
    }

    //Setting Sound
    void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Attack":
                audioSource.clip = audioAttack;
                break;
            case "Damaged":
                audioSource.clip = audioDamaged;
                break;
            case "Item":
                audioSource.clip = audioItem;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "Finish":
                audioSource.clip = audioFinish;
                break;
        }

        audioSource.Play();
    }
}
