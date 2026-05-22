using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float baseMoveSpeed = 3f; // Tốc độ trôi mặc định
    public float deadZone = -10f;

    void Update()
    {
        // Tốc độ thực tế = tốc độ gốc x Hệ số độ khó
        float currentSpeed = baseMoveSpeed * GameManager.instance.globalSpeed;

        // Di chuyển cột ống sang trái
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;

        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}