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
using UnityEngine;

namespace LeiaUnity.Examples.Asteroids
{
    public class Shoot : MonoBehaviour
    {
        [SerializeField] private Transform projectilePrefab;
        [SerializeField] private Transform[] spawnPoint;
        int currentSpawnPoint;
        [SerializeField] private float interval = .3f;
        float timer;

        public void Fire()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Instantiate(projectilePrefab, spawnPoint[currentSpawnPoint].position, Quaternion.Euler(spawnPoint[currentSpawnPoint].forward));
                timer = interval;
                currentSpawnPoint++;
                if (currentSpawnPoint >= spawnPoint.Length)
                {
                    currentSpawnPoint = 0;
                }
            }
        }
    }
}