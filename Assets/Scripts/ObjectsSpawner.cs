using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectspawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> objects;
    public GameObject bot;
    public float spawnInterval = 1f;
    public Transform spawnTransform;
    public Transform EndPoint;
    public float duration;
    float _randomIndex = -1;
    private bool endGame = false;
    [SerializeField] private GameObject finishBG;

    private void Start()
    {
        EndGame.Instance.finishBG = finishBG;
        finishBG.SetActive(false);
    }
    public void StartSpawning()
    {
        objects = GameManager.Instance.missions;
        if (objects[0] == null || objects[0].name != "Bot")
        {
            objects.RemoveAll(item => item == null);
            objects.Insert(0, bot);
        }
        InvokeRepeating(nameof(SpawnObjects), 0f, spawnInterval);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnObjects));
    }
    private void SpawnObjects()
    {
        Debug.Log("Spawn");
        int randomIndex = Random.Range(0, objects.Count);
        if (randomIndex == _randomIndex) return;
        GameObject randomObject = objects[randomIndex];
        GameObject spawnedObject;
        Vector3 newPosition = spawnTransform.position;
        if (randomIndex == 0)
        {
            spawnedObject = objects[0];
            spawnedObject.SetActive(true);
            _randomIndex = 0;
            newPosition.x += 0.7f;
        }
        else spawnedObject = Instantiate(randomObject);

        spawnedObject.transform.position = newPosition;

        StartCoroutine(MoveToPosition(newPosition, duration, spawnedObject));
    }

    IEnumerator MoveToPosition(Vector3 startPosition, float duration, GameObject spawnedObject)
    {
        float elapsedTime = 0f;
        Vector3 destination = EndPoint.transform.position;

        while (elapsedTime < duration && spawnedObject != null)
        {
            // Lerp chỉ cho trục Z
            float newZ = Mathf.Lerp(startPosition.z, destination.z, elapsedTime / duration);

            // Giữ nguyên giá trị X và Y từ startPosition
            Vector3 newPosition = new Vector3(startPosition.x, startPosition.y, newZ);

            spawnedObject.transform.position = newPosition;
            elapsedTime += Time.deltaTime;
            endGame = GameManager.Instance.endGame;
            if (endGame)
            {
                Destroy(spawnedObject);
            }
            yield return null;
        }

        // Đặt vị trí cuối cùng, đảm bảo chỉ trục Z thay đổi
        Vector3 finalPosition = new Vector3(startPosition.x, startPosition.y, EndPoint.transform.position.z);
        
        if (spawnedObject != null)
        {
            spawnedObject.transform.position = finalPosition;

            if (_randomIndex == 0)
            {
                spawnedObject.SetActive(false);
                _randomIndex = -1;
            }
            else
                Destroy(spawnedObject);
        }
    }
}
