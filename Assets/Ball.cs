using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.position = Random.insideUnitSphere * (10 + Random.Range(0.2f, 0.3f));
            ScoreBoard.Instance.Increment();
        }
    }
}
