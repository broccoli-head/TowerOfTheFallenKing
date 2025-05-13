using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrzybAnimation : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        //if (rb == null || anim == null)
          //  Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.x > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        if (rb.velocity.magnitude > 0)
        {
            anim.SetBool("Moving", true);
        }
        else
            anim.SetBool("Moving", false);
    }
}
