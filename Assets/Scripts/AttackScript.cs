using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Timeline.Actions;
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
        //isAttacking = false;
        //canAttackAgain = true;

        // Eğer saldırı sırasında yeni bir girdi alındıysa doubleAttack coroutine'ini başlat

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


        yield return new WaitForSeconds(0.3f);
        if (gameObject.GetComponent<CharacterMovementScript>().isRunning)
        {
            yield return new WaitForSeconds(0.2f);
        }

        charAnim.SetLayerWeight(3, 0f);
        charAnim.SetBool("isRapidAttack", false);
        doubleAttacking = false;

    }

}
