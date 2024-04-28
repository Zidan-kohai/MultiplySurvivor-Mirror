using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private IMove movement;
    [SerializeField] private Animator animator;

    [SerializeField] private bool isFall;

    public void SetIMove(IMove movement)
    {
        this.movement = movement;
    }

    public void Fall()
    {
        isFall = true;
        animator.SetTrigger("Crawl");
    }

    public void Up()
    {
        isFall = false;
        animator.SetTrigger("Up");
    }

    public void Update()
    {
        float moveSpeed = movement.MoveSpeed();

        animator.SetFloat("MoveSpeed", moveSpeed);


        //bool jumped = movement.IsJump();

        //if (jumped)
        //{
        //    animator.SetBool("Jumping", true);
        //}
        //else
        //{
        //    animator.SetBool("Jumping", false);
        //}
    }
}