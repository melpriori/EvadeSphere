using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _timeToMove, _timeToScale;

    [SerializeField]
    private Vector3 _startPosTop, _endPosTop, _startPosBottom, _endPosBottom, _startScale, _endScale;

    private void Start()
    {
        StartCoroutine(Move());
    }

    private void OnEnable()
    {
        GameManager.GameEnded += GameEnded;
    }

    private void OnDisable()
    {
        GameManager.GameEnded -= GameEnded;
    }

    private IEnumerator Move()
    {        
        Vector3 posStart = _startPosTop + Random.Range(0f, 1f) * (_endPosTop - _startPosTop);
        transform.position = posStart;
        Vector3 posEnd = _startPosBottom + Random.Range(0f, 1f) * (_endPosBottom - _startPosBottom);
        transform.rotation = Quaternion.identity;

        yield return Scale(transform, _startScale, _endScale, _timeToScale);

        float timeElapsed = 0f;
        float speed = 1 / _timeToMove;
        Vector3 offset = posEnd - posStart;
        Quaternion startRot = Quaternion.Euler(0, 0, 0);
        Quaternion endRot = Quaternion.Euler(0, 0, 
            (Random.Range(0,2) == 0 ? 1 : -1) * 180 );

        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.SetPositionAndRotation(
                posStart + timeElapsed * offset,
                Quaternion.Lerp(startRot,endRot,timeElapsed)
                );
            yield return new WaitForFixedUpdate();
        }


        yield return Scale(transform, _endScale, Vector3.zero, _timeToScale);
        Destroy(gameObject);
    }



    private void GameEnded()
    {
        StartCoroutine(Scale(transform, _endScale, Vector3.zero, _timeToScale));
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
