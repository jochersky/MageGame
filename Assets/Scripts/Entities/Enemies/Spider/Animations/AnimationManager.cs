using UnityEngine;

// [RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    // Animation Hashes
    [HideInInspector] public readonly int RegularHash = Animator.StringToHash("Regular");
    [HideInInspector] public readonly int DeadHash = Animator.StringToHash("Dead");
    
    public delegate void StartDone();
    public event StartDone OnStartDone;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        OnStartDone?.Invoke();
    }

    public void Regular()
    {
        animator.CrossFade(RegularHash, 0, 0);
    }
    
    public void Dead()
    {
        animator.CrossFade(DeadHash, 0, 0);
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
