using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackScript : MonoBehaviour
{
    public Animator charAnim;
    private Vector2 AttackInput;
    private PlayerInput playerInput;
    Rigidbody2D rb;

    public bool isAttacking = false;

    public bool canAttackAgain = true;

    public bool canMove = true;


    public bool comboAttack = false;

    public bool doubleAttacking = false;

    public bool isCanceled;
    //private void Awake()
    //{
    //    rb = GetComponent<Rigidbody2D>();
    //    playerInput = GetComponent<PlayerInput>();
    //}
    //private void Update()
    //{
    //    AttackInput = playerInput.actions["Attack"].ReadValue<Vector2>();
    //    playerInput = GetComponent<PlayerInput>();
    //    var attackAction = playerInput.actions["Attack"];

    //    attackAction.performed += OnAttackPerformed; // Tuşa basıldığında tetiklenir.
    //    attackAction.canceled += OnAttackCanceled; // Tuş bırakıldığında tetiklenir.
    //    if (AttackInput != Vector2.zero)
    //    {

    //         if (!isAttacking && canAttackAgain && !doubleAttacking)
    //        {
    //            canAttackAgain = false;
    //            isAttacking = true;
    //            StartCoroutine("Attack");
    //        }

    //        if (isAttacking && comboAttack && isCanceled)
    //        {
    //            StartCoroutine("doubleAttack");
    //        }

    //    }

    //}
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        var attackAction = playerInput.actions["Attack"];
        attackAction.performed += OnAttackPerformed; // Tuşa basıldığında tetiklenir.
        attackAction.canceled += OnAttackCanceled; // Tuş bırakıldığında tetiklenir.
    }

    private void OnDestroy()
    {
        var attackAction = playerInput.actions["Attack"];
        attackAction.performed -= OnAttackPerformed;
        attackAction.canceled -= OnAttackCanceled;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!isAttacking && canAttackAgain && !doubleAttacking)
        {
            canAttackAgain = false;
            isAttacking = true;
            StartCoroutine("Attack");
        }

        if (isAttacking && comboAttack && isCanceled)
        {
            StartCoroutine("doubleAttack");
        }

        isCanceled = false; // Her başarılı atakta, iptal durumunu sıfırla
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isCanceled = true;
    }
  
    IEnumerator Attack()
    {
        if (!gameObject.GetComponent<CharacterMovementScript>().isRunning)
        {
            canMove = false;

        }
        if (isAttacking)
        {
            charAnim.SetBool("isAttackingRight", true);
            charAnim.SetLayerWeight(1, 1f);
            gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 5f;
            yield return new WaitForSeconds(0.267f);
            charAnim.SetBool("canAttackEnd", true);
            gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 0f;
            comboAttack = true;
        }

        if (!gameObject.GetComponent<CharacterMovementScript>().isRunning)
        {
            yield return new WaitForSeconds(0.4f);
            canMove = true;

        }

        yield return new WaitForSeconds(0.1f);

        charAnim.SetBool("isAttackingLeft", false);
        charAnim.SetBool("isAttackingRight", false);

        charAnim.SetBool("isAttacking", false);
        gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 19f;
        charAnim.SetLayerWeight(1, 0f);

        yield return new WaitForSeconds(0.2f);
        comboAttack = false;


        isAttacking = false;
        canAttackAgain = true;
        canMove = true;


    }


    IEnumerator doubleAttack()
    {
        doubleAttacking = true;
        charAnim.SetBool("isRapidAttack", true);
        charAnim.SetLayerWeight(3, 1f);
        yield return new WaitForSeconds(0.267f);

        if (!gameObject.GetComponent<CharacterMovementScript>().isRunning)
        {
            yield return new WaitForSeconds(0.2f);
        }

        charAnim.SetLayerWeight(3, 0f);
        charAnim.SetBool("isRapidAttack", false);
        doubleAttacking = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.name=="wolf"&& isAttacking || collision.gameObject.name == "wolf" && doubleAttacking)
        //{
        //    Debug.Log("rffşl1");

        //    collision.gameObject.GetComponent<WolfMovementScript>().wolfHealth = collision.gameObject.GetComponent<WolfMovementScript>().wolfHealth - 5;
        //    collision.gameObject.GetComponent<WolfMovementScript>().healthSlider.value = collision.gameObject.GetComponent<WolfMovementScript>().wolfHealth;
        //}

        if ((collision.gameObject.name == "wolf" && isAttacking) || (collision.gameObject.name == "wolf" && doubleAttacking))
        {
            Debug.Log("Hit wolf");

            // Wolf sağlık azaltma
            var wolfMovement = collision.gameObject.GetComponent<WolfMovementScript>();
            wolfMovement.wolfHealth -= 5;
            wolfMovement.healthSlider.value = wolfMovement.wolfHealth;

            // Vuruş kuvveti uygulama
            Rigidbody2D wolfRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (wolfRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                float knockbackForce = 50f;  // Kuvvet miktarını azalttık
                wolfRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                StartCoroutine(ResetWolfVelocity(wolfRb, 0.2f));  // 0.2 saniye sonra hızı sıfırla
            }
        }
    }
    IEnumerator ResetWolfVelocity(Rigidbody2D wolfRb, float delay)
    {
        yield return new WaitForSeconds(delay);
        wolfRb.velocity = Vector2.zero;  // Hızı sıfırla
    }


}
