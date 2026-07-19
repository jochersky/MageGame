using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class Jellyfish : Entity
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D body;
    [SerializeField] Health health;
    [SerializeField] AnimationClip floatClip;
    [SerializeField] float floatAnimSpeed;
    [SerializeField] AnimationClip launchClip;
    [SerializeField] float launchAnimSpeed;
    [SerializeField] float windUpRatio = 0.25f;
    [SerializeField] float launchForce;
    [SerializeField] float aggroRange;
    private Player player;
    private State state = State.Floating;
    private float launchInterval;
    private float launchTimer = 0f;
    // Animation Hashes
    private readonly int floatAnim = Animator.StringToHash("Float");
    private float floatAnimLength;
    private readonly int launchAnim = Animator.StringToHash("Launch");
    private float launchAnimLength;
    private Collision2D lastCollision;
    private float defaultMinX = -1;
    private float defaultMaxX = 1;
    private float defaultMinY = -1;
    private float defaultMaxY = 1;
    private enum State
    {
        Floating,
        Launching
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        health.OnDeath += () =>
        {
            Destroy(gameObject);
        };

        floatAnimLength = floatClip.length / floatAnimSpeed;
        launchAnimLength = launchClip.length / launchAnimSpeed;
        launchInterval = floatAnimLength * 2;
        launchTimer = launchInterval;
        animator.CrossFade(floatAnim, 0, 0);

    }

    void Update()
    {
        if (state == State.Floating)
        {
            if (launchTimer <= 0f)
            {
                state = State.Launching;
                launchTimer = launchInterval;
                Vector2 direction = ChooseRandomDirection();
                StartCoroutine(Launch(direction));
            }
            else
            {
                launchTimer -= Time.deltaTime;
            }
        }
    }
    // this feels dangerous, should windUp and Launch be different states?
    IEnumerator Launch(Vector2 direction)
    {
        lastCollision = null;
        transform.up = direction;
        animator.CrossFade(launchAnim, 0, 0);
        yield return new WaitForSeconds(launchAnimLength * windUpRatio);
        body.AddForce(launchForce * direction);
        yield return new WaitForSeconds(launchAnimLength * (1 - windUpRatio));
        state = State.Floating;
        animator.CrossFade(floatAnim, 0, 0);
    }

    private Vector2 ChooseRandomDirection()
    {
        float xMin = defaultMinX;
        float xMax = defaultMaxX;
        float yMin = defaultMinY;
        float yMax = defaultMaxY;
        float x_comp = -000;
        float y_comp = -000;
        // if player is in range, launch towards them
        if (Vector2.Distance(transform.position, player.transform.position) <= aggroRange)
        {
            x_comp = player.transform.position.x - transform.position.x;
            y_comp = player.transform.position.y - transform.position.y;
        }
         // Jellyfish won't launch towards a surface they just collided with
        else if (lastCollision != null)
        {
            Vector2 normal = lastCollision.GetContact(0).normal;
            normal.Normalize();
            // extremely dubious math to adjust range according to normal
            xMin = Math.Clamp(normal.x - Math.Abs(normal.y), -1, 0);
            xMax = Math.Clamp(normal.x + Math.Abs(normal.y), 0, 1);
            yMin = Math.Clamp(-Math.Abs(normal.x) + normal.y, -1, 0);
            yMax = Math.Clamp(Math.Abs(normal.x) + normal.y, 0, 1);
            x_comp = UnityEngine.Random.Range(xMin, xMax);
            y_comp = UnityEngine.Random.Range(yMin, yMax);
        } else
        {
            x_comp = UnityEngine.Random.Range(xMin, xMax);
            y_comp = UnityEngine.Random.Range(yMin, yMax);
        }
        
        Vector2 direction = new Vector2(x_comp, y_comp);
        direction.Normalize();
        return direction;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Environment"))
        {
            lastCollision = collision;
        }
    }
    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Environment")) {
    //         lastCollision = collision;
    //     }
    // }
}
