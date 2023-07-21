using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    
    private GameObject[] balls = new GameObject[100];

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            balls[i] = Instantiate(ballPrefab, Random.onUnitSphere * (10 + Random.Range(0.2f, 0.3f)), Quaternion.identity, transform);
        }
    }
}
