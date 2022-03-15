using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private GameObject afterImagePrefab;

    [Header("Variable")]
    [SerializeField] private Queue<GameObject> availableObject = new Queue<GameObject>();
    [SerializeField] private float maxPool;

    public static AfterImagePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    public void GrowPool()
    {
        for(int i = 0; i < maxPool; i++)
        {
            GameObject instantObject = Instantiate(afterImagePrefab);
            instantObject.transform.SetParent(transform);
            AddToPool(instantObject);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObject.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(availableObject.Count == 0)
        {
            GrowPool();
        }

        GameObject instance = availableObject.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
