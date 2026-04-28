using UnityEngine;

public class CactusSpawner : MonoBehaviour
{
    public GameObject cactusPrefab;
    public float spawnTime = 2f;
    public float yPos = -4.5f;

    void Start()
    {
        InvokeRepeating("Spawn", 1f, spawnTime);
    }

    void Spawn()
    {
        float xPos = 12f;

        Instantiate(cactusPrefab,
            new Vector3(xPos, yPos, 0),
            Quaternion.identity);
    }
}
