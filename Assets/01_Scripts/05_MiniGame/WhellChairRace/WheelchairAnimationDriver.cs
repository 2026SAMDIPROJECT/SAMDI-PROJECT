using UnityEngine;

public class WheelchairAnimationDriver : MonoBehaviour
{
    [SerializeField] private WheelchairController controller;
    [SerializeField] private Animator animator;

    private void Update()
    {
        animator.speed = controller.IsMoving ? 1f : 0f;
    }
}
