/*
 * Copyright 2024 (c) Leia Inc.  All rights reserved.
 *
 * NOTICE:  All information contained herein is, and remains
 * the property of Leia Inc. and its suppliers, if any.  The
 * intellectual and technical concepts contained herein are
 * proprietary to Leia Inc. and its suppliers and may be covered
 * by U.S. and Foreign Patents, patents in process, and are
 * protected by trade secret or copyright law.  Dissemination of
 * this information or reproduction of this materials strictly
 * forbidden unless prior written permission is obtained from
 * Leia Inc.
 */
using System.Collections;
using UnityEngine;

namespace LeiaUnity.Examples.Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private Transform asteroidPrefab;
        [SerializeField] private Vector3 size;
        [SerializeField] private float spawnInterval = 1f;

        void Start()
        {
            StartSpawnTimer(spawnInterval);
        }

        void StartSpawnTimer(float waitTime)
        {
            IEnumerator timer = SpawnTimer(waitTime);
            StartCoroutine(timer);
        }
        IEnumerator SpawnTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            //Spawn Asteroid
            Instantiate(asteroidPrefab, transform.position + new Vector3(
                Random.value * size.x - size.x / 2f,
                Random.value * size.y - size.y / 2f,
                Random.value * size.z - size.z / 2f),
                Quaternion.identity
            );
            StartSpawnTimer(spawnInterval);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}