using UnityEngine;

public class GroundMove : MonoBehaviour
{
    private float width;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }

    void Update()
    {
        float speed = GameManager.instance.speed;


        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x <= -width)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector3 pos = transform.position;
        pos.x += width * 2f;
        transform.position = pos;
    }
}
