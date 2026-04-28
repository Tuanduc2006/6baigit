using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float moveSpeed = 2f;
    float destroyX;

    void Awake()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        destroyX = -camHalfWidth - 2f;

        Debug.Log(
            $"[PIPE SPAWN] x={transform.position.x}, destroyX={destroyX}, frame={Time.frameCount}"
        );
    }

    void Update()
    {
        // Pipe chỉ đứng yên khi không Playing
        if (GameManager.instance.state == GameState.Playing)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }

        // HỦY LUÔN ĐƯỢC – KHÔNG PHỤ THUỘC STATE
        if (transform.position.x < destroyX)
        {
            Debug.Log(
                $"[PIPE DESTROY] x={transform.position.x}, destroyX={destroyX}, frame={Time.frameCount}"
            );
            Destroy(gameObject);
        }
    }
}
