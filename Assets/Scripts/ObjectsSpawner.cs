using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectspawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private List<GameObject> foodObjects;
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
        Shadow.SetActive(false);
    }
    private void SpawnObjects()
    {
        Debug.Log("Spawn");
        List<GameObject> selectedList = (Random.Range(0, 2) == 0) ? foodObjects : objects;

        int randomIndex = Random.Range(0, selectedList.Count);
        GameObject randomObject = selectedList[randomIndex];
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && obj == randomObject) return;
        } 
        GameObject spawnedObject;
        Vector3 newPosition = spawnTransform.position;

        if (selectedList == foodObjects)
        {
            float randomX = Random.Range(-1f, 1f);
            newPosition = new Vector3(randomX, newPosition.y, newPosition.z);
            spawnedObject = Instantiate(randomObject);
        }
        else 
        {
            if (randomIndex == 0)
            {
                spawnedObject = objects[0];
                spawnedObject.SetActive(true);
                newPosition.x += 0.7f;
            }
            else spawnedObject = Instantiate(randomObject);
        }
        spawnedObject.transform.position = newPosition;
        spawnedObjects.Add(spawnedObject);

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
                Shadow.SetActive(false);
                Destroy(spawnedObject);
            }
            yield return null;
        }

        // Đặt vị trí cuối cùng, đảm bảo chỉ trục Z thay đổi
        Vector3 finalPosition = new Vector3(startPosition.x, startPosition.y, EndPoint.transform.position.z);

        spawnedObjects.Remove(spawnedObject);

        if (spawnedObject != null)
        {
            spawnedObject.transform.position = finalPosition;
            Shadow.SetActive(true);
            if (spawnedObject == bot)
            {
                spawnedObject.SetActive(false);
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
