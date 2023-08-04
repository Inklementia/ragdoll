using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class RandomSpawner : MonoBehaviour
    {
        [SerializeField] private ObjectPooler pooler;
        [SerializeField] private Tag cubeTag;
        
        [SerializeField] private float maxWidth = 90;
        [SerializeField] private float maxHeight = 90;
        
        [SerializeField] private int number = 100;
        
        [SerializeField] private float spawnCollisionCheckRadius = 6;

        void Start()
        {
            
            float halfWidth = Mathf.Floor(maxWidth / 2);
            float halfHeight = Mathf.Floor(maxHeight / 2);
            int i = 0;
            
            while(i < number)
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(-halfWidth, halfWidth + 1), Random.Range(6, 18), Random.Range(-halfHeight, halfHeight + 1));
                if (!Physics.CheckSphere(randomSpawnPosition, spawnCollisionCheckRadius))
                {
                    pooler.SpawnFromPool(cubeTag, randomSpawnPosition, Quaternion.identity);
                    i++;
                }
            }
        }
    }
    

