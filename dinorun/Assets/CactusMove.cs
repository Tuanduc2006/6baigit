using UnityEngine;

public class CactusMove : MonoBehaviour
{
    void Update()
    {
        // Lấy speed từ GameManager
        float speed = GameManager.instance.speed;


        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
