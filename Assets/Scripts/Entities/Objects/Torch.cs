using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Torch : Lightable
{
    private Animator _animator;

    // Animation Hashes
    private readonly int _unlit = Animator.StringToHash("Unlit");
    private readonly int _lit = Animator.StringToHash("Lit");

    new void Start()
    {
        base.Start();
        
        _animator = GetComponent<Animator>();

        if (IsLit)
        {
            _animator.CrossFade(_lit, 0 , 0);
        }

        OnLightActivated += () =>
        {
            _animator.CrossFade(_lit, 0 , 0);
        };
        OnLightDeactivated += () =>
        {
            _animator.CrossFade(_unlit, 0, 0);
        };
    }
}
