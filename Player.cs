using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Timer timer;

    public int counts;
    public GameObject gameOverPanel;
    

    public PlayerScriptRenderer smallRenderer;
    public PlayerScriptRenderer bigRenderer;
    public PlayerScriptRenderer activeRenderer;

    private DeathAnimation deathAnimation;
    private CapsuleCollider2D capsuleCollider;

    public bool big => bigRenderer.enabled;
    public bool small => smallRenderer.enabled;
    public bool dead => deathAnimation.enabled;
    public bool starpower { get; private set; }
    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        activeRenderer = smallRenderer;
    }
    public void Hit()
    {
        if (!dead && !starpower)
        {
            if (big)
            {
                Shrink();
                //StartCoroutine("UnBeatTime");
            }
            else
            {
                Death();
            }
        }
    }
    IEnumerator UnBeatTime(SpriteRenderer spriteRenderer)
    {
        int countTime = 0;
        while (countTime < 10)
        {
            if (countTime%2 == 0)
            {
                spriteRenderer.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                spriteRenderer.color = new Color32(255, 255, 255, 180);
            }

            yield return new WaitForSeconds(0.2f);

            countTime++;
        }

        spriteRenderer.color = new Color32(255, 255, 255, 255);

        yield return null;
    }
    private void Death()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;
        
        timer.StopTimer();

        ShowGameOver();

        counts++;
    }
    public void ShowGameOver()
    {
        Invoke("ActivateGameOverPanel", 2f);
    }

    public void ActivateGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void Grow()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = true;
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(ScaleAnimation());
    }

    private void Shrink()
    {
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(ScaleAnimation());
    }

    private IEnumerator ScaleAnimation()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                smallRenderer.enabled = !smallRenderer.enabled;
                bigRenderer.enabled = !smallRenderer.enabled;
            }

            yield return null;
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;

        activeRenderer.enabled = true;
    }

    public void Starpower(float duration = 10)
    {
        StartCoroutine(StarpowerAnimation(duration));
    }
    
    private IEnumerator StarpowerAnimation(float duration)
    {
        starpower = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 8 == 0)
            {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null; 
        }

        activeRenderer.spriteRenderer.color = Color.white;

        starpower = false;
    }
    public void GameEnd()
    {
        if (counts > 3)
        {
            Application.Quit();
        }
    }
}
