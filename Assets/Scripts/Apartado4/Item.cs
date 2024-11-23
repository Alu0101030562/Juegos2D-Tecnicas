using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;

    private int _timesCollected;

    private Spawner _spawner;

    private void Start()
    {
        _spawner = GetComponentInParent<Spawner>();
    }


    private void Update()
    {
        transform.Translate(Vector3.left * (_moveSpeed * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _timesCollected++;

            if (_timesCollected == 3)
            {
                _spawner.UnsubscribeItem(this);
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}