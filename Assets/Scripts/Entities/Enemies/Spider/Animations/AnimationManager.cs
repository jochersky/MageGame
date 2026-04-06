using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    // Animation Hashes
    [HideInInspector] public readonly int IdleHash = Animator.StringToHash("Idle");
    [HideInInspector] public readonly int MoveHash = Animator.StringToHash("Move");
    [HideInInspector] public readonly int AttackHash = Animator.StringToHash("Attack");
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        animator.CrossFade(IdleHash, 0, 0);
    }
    
    public void Move()
    {
        animator.CrossFade(MoveHash, 0, 0);
    }
    
    public void Attack()
    {
        animator.CrossFade(AttackHash, 0, 0);
    }

    public float GetAnimationDuration(int animHash)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == animHash)
            {
                return clip.length;
            }
        }

        return -1;
    }
}
