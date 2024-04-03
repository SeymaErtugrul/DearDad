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
    public bool isAttacking = false;
    Rigidbody2D rb;
    private void Awake()
    {
      rb=GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        AttackInput = playerInput.actions["Attack"].ReadValue<Vector2>();
        if (AttackInput!=Vector2.zero && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine("Attack");
        }
    }

    IEnumerator Attack()
    {
        if (isAttacking && gameObject.GetComponent<CharacterMovementScript>().groundedRight)
        {
            charAnim.SetBool("isAttackingRight", true);
            charAnim.SetLayerWeight(1, 1f);
        }

        else if (isAttacking && gameObject.GetComponent<CharacterMovementScript>().groundedLeft)
        {
            charAnim.SetBool("isAttackingLeft", true);
            charAnim.SetLayerWeight(1, 1f);
        }
        //charAnim.SetLayerWeight(1, 1f);
        //charAnim.SetBool("isAttacking", true);
        //float pushForce = 5f; // Bu değeri ayarlayarak karakterin ne kadar itileceğini kontrol edebilirsiniz
        //Vector2 pushDirection = (gameObject.GetComponent<CharacterMovementScript>().isFacingRight ? Vector2.right : Vector2.left) * pushForce;

        //// Rigidbody'e anlık bir ivme uygula
        //rb.AddForce(pushDirection, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f);
        charAnim.SetBool("isAttackingLeft", false);
        charAnim.SetBool("isAttackingRight", false);
        charAnim.SetLayerWeight(1, 0f);
        charAnim.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (isAttacking && gameObject.GetComponent<CharacterMovementScript>().groundedRight)
        {

        }
    }
}
