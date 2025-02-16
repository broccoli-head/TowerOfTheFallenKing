using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class UniversalAnimationController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spriteR;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteR = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Vector2 MoveDirection = rb.velocity.normalized;

        if (MoveDirection.y >= 0.4f)
            anim.SetBool("WalkingBack", true);
        else
            anim.SetBool("WalkingBack", false);
        
        if (MoveDirection.y <= -0.4f)
            anim.SetBool("WalkingFront", true);
        else
            anim.SetBool("WalkingFront", false);

        if(MoveDirection.x != 0)
        {
            anim.SetBool("WalkingSide", true);
            if (MoveDirection.x < 0)
                spriteR.flipX = true;
            else
                spriteR.flipX = false;
        }
        else 
            anim.SetBool("WalkingSide", false);
    }
}
