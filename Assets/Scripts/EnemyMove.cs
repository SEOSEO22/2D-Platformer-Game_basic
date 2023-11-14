using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    public int nextMove;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        Invoke("Think", 3);
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) {
            nextMove *= -1;
            spriteRenderer.flipX = nextMove == 1;

            CancelInvoke();
            Invoke("Think", 2);
        }
    }

    private void Think() {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("WalkSpeed", nextMove);

        if (nextMove != 0) spriteRenderer.flipX = nextMove == 1;

        float thinkTime = Random.Range(2f, 6f);
        Invoke("Think", thinkTime);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        spriteRenderer.flipY = true;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        boxCollider.enabled = false;
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
