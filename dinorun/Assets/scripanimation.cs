using UnityEngine;

public class scripanimation : MonoBehaviour
{
    Animator anim;
    bool dead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Cactus") && !dead)
        {
            dead = true;
            anim.SetBool("isDead", true);
            Time.timeScale = 0f; // Dừng game
        }
    }
}
