using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] GameObject staff;
    [SerializeField] private int frames = 30;
    [SerializeField] AudioClip[] swingSFX;
    readonly float swingVolume = 0.25f;
    int animationFrames = 30;
    float animationDuration = 0.15f;
    bool isAttacking = false;
    int totalRotation = -120;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Swing()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            if (AudioManager.instance != null)
                AudioManager.instance.PlayRandomClipFromAt(swingSFX, transform, swingVolume);
            StartCoroutine(SwingAnimation()); 
        }
    }

    IEnumerator SwingAnimation()
    {
        Collider2D hitbox = staff.GetComponentInChildren<Collider2D>();
        hitbox.enabled = true;
        float rotationAmt = totalRotation / frames;
        float frameDuration = animationDuration / frames;
        for (int frame = 0; frame < frames; frame++)
        {
            staff.transform.Rotate(new Vector3(0, 0, rotationAmt));
            yield return new WaitForSeconds(frameDuration);
        }
        isAttacking = false;
        staff.transform.rotation = Quaternion.identity;
        hitbox.enabled = false;
    }
}
