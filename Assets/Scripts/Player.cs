using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private Vector3 _startPos, _endPos, _startScale, _endScale;

    [SerializeField]
    private float _startSpeed, _timeToScale;

    private float speed;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _loseClip;

    private void Awake()
    {
        speed = _startSpeed;
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        GameManager.GameEnded += GameEnded;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        GameManager.GameEnded -= GameEnded;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            AudioManager.Instance.PlaySound(_moveClip);
            speed *= -1f;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.right);
        if(transform.position.x > _endPos.x || transform.position.x < _startPos.x)
        {
            speed *= -1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            AudioManager.Instance.PlaySound(_loseClip);
            GameManager.Instance.EndGame();
            GetComponent<Collider2D>().enabled = false;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            return;
        }
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            AudioManager.Instance.PlaySound(_pointClip);
            GameManager.Instance.UpdateScore();
            Destroy(collision.gameObject);
            return;
        }
    }

    private void GameStarted()
    {
        StartCoroutine(Scale(transform,_startScale,_endScale,_timeToScale));
    }

    private void GameEnded()
    {
        StartCoroutine(Scale(transform, _endScale,Vector3.zero, _timeToScale));
        Destroy(gameObject, _timeToScale);
    }



    public IEnumerator Scale(Transform target, Vector3 startScale, Vector3 endScale, float timeToFinish)
    {
        target.localScale = startScale;

        float timeElapsed = 0f;
        float speed = 1 / timeToFinish;
        Vector3 offset = endScale - startScale;

        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            target.localScale = startScale + timeElapsed * offset;
            yield return null;
        }

        target.localScale = endScale;
    }
}