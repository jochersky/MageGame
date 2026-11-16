using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStyle : MonoBehaviour
{
    [SerializeField] private float eyeLifetimeMin;
    [SerializeField] private float eyeLifetimeMax;
    [SerializeField] private float revealTime;
    [SerializeField] private float hideTime;
    [SerializeField] private Image[] eyes;
    [SerializeField] private Sprite[] eyeSprites;
    private int _pos = 0;
    private int _midPos = 1;
    private CountdownTimer _timer;
    
    private void Start()
    {
        _midPos = eyes.Length / 2;
        
        foreach (var eye in eyes)
        {
            eye.sprite = eyeSprites[Random.Range(0, eyeSprites.Length)];
        }
        
        _timer = new CountdownTimer(Random.Range(eyeLifetimeMin, eyeLifetimeMax));
        _timer.OnTimerStop += TimerLapped;
        _timer.Start();
    }

    private void Update()
    {
        _timer.Tick(Time.deltaTime);
    }

    private void TimerLapped()
    {
        _timer.Time = Random.Range(eyeLifetimeMin, eyeLifetimeMax);
        _timer.Start();
        
        _pos = (_pos + 1) % eyes.Length;
        _midPos = (_midPos + 1) % eyes.Length;

        StartCoroutine(RevealEye(eyes[_pos]));
        StartCoroutine(HideEye(eyes[_midPos]));
    }

    private IEnumerator RevealEye(Image eye)
    {
        eye.sprite = eyeSprites[Random.Range(0, eyeSprites.Length)];
        eye.enabled = true;
        
        yield return new WaitForSeconds(0.1f);
        
        float timer = 0;
        while (timer < revealTime)
        {
            timer += Time.deltaTime;
            float progress = timer / revealTime;
            eye.color = new Color(eye.color.r, eye.color.g, eye.color.b, progress);
            yield return null;
        }
    }

    private IEnumerator HideEye(Image eye)
    {
        float timer = 0;
        while (timer < revealTime)
        {
            timer += Time.deltaTime;
            float progress = 1 - (timer / revealTime);
            eye.color = new Color(eye.color.r, eye.color.g, eye.color.b, progress);
            yield return null;
        }
        
        eyes[_midPos].enabled = false;
    }
}
