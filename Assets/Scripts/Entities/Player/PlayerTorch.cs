using UnityEngine;

public class PlayerTorch : Torch
{
    [Header("Experience Stats")]
    [SerializeField] private int torchExp;
    
    private Animator _animator;

    // Animation Hashes
    private readonly int _unlit = Animator.StringToHash("Unlit");
    private readonly int _lit = Animator.StringToHash("Lit");
    
    public delegate void ExpGained(int exp);
    public event ExpGained OnExpGained;
    
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
            litSound.Play();
        };
        OnLightDeactivated += () =>
        {
            _animator.CrossFade(_unlit, 0, 0);
        };
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Lightable>(out Lightable lightable))
        {
            if (IsLit && lightable.IsLit) return;
            
            if (IsLit && !lightable.IsLit)
            {
                lightable.ActivateLightSource();
                OnExpGained?.Invoke(torchExp);
            }
        }
    }
}
