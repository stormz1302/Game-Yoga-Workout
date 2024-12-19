using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectspawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject bot;
    [SerializeField] private GameObject Shadow;
    public float spawnInterval = 1f;
    public Transform spawnTransform;
    public Transform EndPoint;
    public float duration;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    private bool endGame = false;
    [SerializeField] private GameObject finishBG;
    PlayerController player;

    private void Start()
    {
        EndGame.Instance.finishBG = finishBG;
        finishBG.SetActive(false);
        player = FindObjectOfType<PlayerController>();
    }


    public void StartSpawning()
    {
        Shadow.SetActive(true);
        StartCoroutine(UpdateShadowPosition());
        StartCoroutine(SpawnObjects());
    }


    public void StopSpawning()
    {
        StopCoroutine(SpawnObjects());
        Shadow.SetActive(false);
    }
    private IEnumerator SpawnObjects()
    {
        endGame = GameManager.Instance.endGame;
        if (endGame) yield break;  // Stop spawning if the game is over
        SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
        GameObject spawnedObject = null;
        GameObject randomObject = null;

        while (!endGame) // This loop continues until the game ends
        {
            float spawnInterval = setObjectInLevel.SetSpawnRate();
            randomObject = setObjectInLevel.SpawnObjects();

            Vector3 newPosition = spawnTransform.position;

            if (randomObject != null)
            {
                if (randomObject.name == "Bot")
                {
                    spawnedObject = randomObject;
                    spawnedObject.SetActive(true);
                    newPosition = new Vector3(0.8f, newPosition.y, newPosition.z);
                }
                else
                {
                    spawnedObject = Instantiate(randomObject, newPosition, Quaternion.identity);
                }

                if (randomObject.CompareTag("Clean") || randomObject.CompareTag("notClean"))
                {
                    float randomX = Random.Range(-1f, 1f);
                    newPosition = new Vector3(randomX, newPosition.y, newPosition.z);
                }

                spawnedObject.transform.position = newPosition;
                spawnedObjects.Add(spawnedObject);
                StartCoroutine(MoveToPosition(newPosition, duration, spawnedObject));
            }

            yield return new WaitForSeconds(spawnInterval);  // Wait before the next spawn
        }
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
                Shadow.SetActive(false);
                Destroy(spawnedObject);
            }
            yield return null;
        }

        // Đặt vị trí cuối cùng, đảm bảo chỉ trục Z thay đổi
        Vector3 finalPosition = new Vector3(startPosition.x, startPosition.y, EndPoint.transform.position.z);
        spawnedObjects.Remove(spawnedObject);
        if (spawnedObjects.Count == 0 && !endGame) GameManager.Instance.LoadEndScreen();

        if (spawnedObject != null)
        {
            spawnedObject.transform.position = finalPosition;
            Shadow.SetActive(true);
            if (spawnedObject == bot)
            {
                spawnedObject.SetActive(false);
                SetObjectInLevel setObjectInLevel = FindObjectOfType<SetObjectInLevel>();
                setObjectInLevel.SpawnedBot();
            }
            else
                Destroy(spawnedObject);
        }
    }

    private IEnumerator UpdateShadowPosition()
    {
        while (true) 
        {
            if (spawnedObjects.Count > 0 && spawnedObjects[0] != null)
            {
                GameObject gameObject = spawnedObjects[0];
                Shadow.transform.position = new Vector3(
                    Shadow.transform.position.x,
                    Shadow.transform.position.y,
                    gameObject.transform.position.z
                );
            }

            yield return null; 
        }
    }

    public void DisableShadow()
    {
        Shadow.SetActive(false);
    }
}
