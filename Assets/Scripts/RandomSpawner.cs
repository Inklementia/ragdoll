using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class RandomSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float maxWidth = 90;
        [SerializeField] private float maxHeight = 90;
        [SerializeField] private int number = 60;

        [SerializeField] private float spawnCollisionCheckRadius = 6;

        void Awake()
        {
            float halfWidth = Mathf.Floor(maxWidth / 2);
            float halfHeight = Mathf.Floor(maxHeight / 2);
            
            for (int i = 0; i < number; i++)
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(-halfWidth, halfWidth + 1), Random.Range(6, 18), Random.Range(-halfHeight, halfHeight + 1));
                if (!Physics.CheckSphere(randomSpawnPosition, spawnCollisionCheckRadius))
                {
                    Instantiate(cubePrefab, randomSpawnPosition, Quaternion.identity, transform);
                }
            }
        }
    }
    

