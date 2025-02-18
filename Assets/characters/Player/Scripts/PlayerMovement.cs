using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour, Controller, ReciveSpeedChange
{

    [SerializeField] float MovementSpeed;
    [SerializeField] float DashForce;
    [SerializeField] float DashCooldownTime = 1f;
    public bool DashEnabled;

    float Horizontal;
    float Vertical;
    float speedTime;
    float speedMultiplier;

    bool IsInDash = false;
    bool canMove = true;
    bool speedChanged = false;

    Rigidbody2D rb;
    GameObject cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main.gameObject;
    }
    public void Disable(float time)
    {
        canMove = false;
        StartCoroutine(Enable(time));
    }

    void Update()
    {
        if (canMove)
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            Vector3 direction = ((Vector2.right * Horizontal) + (Vector2.up * Vertical));
            if(!IsInDash)
                rb.velocity = direction * MovementSpeed;
            if ( DashEnabled && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(direction * DashForce,ForceMode2D.Impulse);
                DashEnabled = false;
                IsInDash = true;
                Throw.CanThrow = false;
                StartCoroutine("DashCooldown");
            }
        }
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);

        if (speedTime > 0.0f)
        {
            speedTime -= Time.deltaTime;
        }

        if (speedTime < 0.0f && speedChanged)
        {
            MovementSpeed /= speedMultiplier;
            speedChanged = false;
            speedTime = 0.0f;
        }
    }

    public void ChangeSpeed(float speedMultiplier)
    {
        if (!speedChanged)
        {
            this.speedMultiplier = speedMultiplier;
            MovementSpeed *= speedMultiplier;
            speedChanged = true;
        }
    }

    public void ChangeSpeed(float speedMultiplier, float speedTime)
    {
        ChangeSpeed(speedMultiplier);
        this.speedTime = speedTime;
    }

    public void ChangeSpeedOnExit(float speedTime)
    {
        this.speedTime = speedTime;
    }


    public IEnumerator DashCooldown()
    {
        StartCoroutine("dashTime");
        yield return new WaitForSeconds(DashCooldownTime);
        DashEnabled = true;
        yield break;
    }
    public IEnumerator dashTime()
    {
        yield return new WaitForSeconds(0.2f);
        IsInDash = false;
        Throw.CanThrow = true;
        yield break;
    }
    public IEnumerator Enable(float time)
    {
        yield return new WaitForSeconds(time);
        canMove = true;
        yield break;
    }
}
