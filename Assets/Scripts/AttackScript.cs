using Spine;
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
    public bool isRapidAttack = false; // Rapid attack tracking
    private float lastAttackTime = 0.0f; // Zamanı saklamak için değişken
    private float attackThreshold = 0.6f; // İzin verilen maksimum zaman aralığı

    private Vector2 previousAttackInput = Vector2.zero;

    private void Awake()
    {
      rb=GetComponent<Rigidbody2D>();
      playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        AttackInput = playerInput.actions["Attack"].ReadValue<Vector2>();

        if (AttackInput != Vector2.zero && previousAttackInput == Vector2.zero)
        {
            if (Time.time - lastAttackTime <= attackThreshold && lastAttackTime != 0)
            {
                isRapidAttack = true; // Saldırılar arasındaki süre yeterince kısa ise, hızlı saldırı modunu aktif et
                charAnim.SetBool("isRapidAttack", true);
            }

            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine("Attack");
            }
            lastAttackTime = Time.time; // Son saldırı zamanını güncelle
        }

        previousAttackInput = AttackInput; // Saldırı girişini güncelle

    }



    IEnumerator Attack()
    {
        if (isAttacking)
        {
            charAnim.SetBool("isAttackingRight", true);

            charAnim.SetLayerWeight(1, 1f);
            gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 5f;
            yield return new WaitForSeconds(0.2f);
            gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 0f;
  

        }

        yield return new WaitForSeconds(0.2f);
        charAnim.SetBool("isAttackingLeft", false);
        charAnim.SetBool("isAttackingRight", false);
        gameObject.GetComponent<CharacterMovementScript>().moveSpeed = 19f;
        charAnim.SetLayerWeight(1, 0f);
        charAnim.SetBool("isAttacking", false);
        isAttacking = false;
        isRapidAttack = false;
        charAnim.SetBool("isRapidAttack", false);
        yield return new WaitForSeconds(0.3f);

    }

}
