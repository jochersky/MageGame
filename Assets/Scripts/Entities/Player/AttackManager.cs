using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] GameObject staff;
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
            StartCoroutine(SwingAnimation()); 
        }
    }

    IEnumerator SwingAnimation()
    {
        Collider2D hitbox = staff.GetComponentInChildren<Collider2D>();
        hitbox.enabled = true;
        float rotationAmt = totalRotation / animationFrames;
        float frameDuration = animationDuration / animationFrames;
        for (int frames = 0; frames < animationFrames; frames++)
        {
            staff.transform.Rotate(new Vector3(0, 0, rotationAmt));
            yield return new WaitForSeconds(frameDuration);
        }
        isAttacking = false;
        staff.transform.rotation = Quaternion.identity;
        hitbox.enabled = false;
    }
}
