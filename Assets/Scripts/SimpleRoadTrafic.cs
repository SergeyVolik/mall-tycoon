using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class TraficCarData
    {
        public Transform carInstance;
        public Transform destinationPoint;
        public float speed;
    }

    public class SimpleRoadTrafic : MonoBehaviour
    {
        public GameObject[] carPrefabs;

        public Transform rightSpawnPoint;
        public Transform rightSpawnPointEnd;


        public float carSpeedMPH = 30;
        public float spawnIntervalMin = 5;
        public float spawnIntervalMax = 5;
        private float m_NextSpawnInveral;

        private float m_SpawnT;
        public float distanceToDestoryCar = 1f;
        private List<TraficCarData> m_TraficCars = new List<TraficCarData>();
        private List<TraficCarData> m_TraficCarsToRemove = new List<TraficCarData>();

        public float GetNextSpawnIntarval()
        {
            return UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);
        }

        private void Awake()
        {
            m_NextSpawnInveral = GetNextSpawnIntarval();
        }
        private void Update()
        {
            UpdateSpawner();
            UpdateTrafic();
        }

        private void UpdateTrafic()
        {
            foreach (var item in m_TraficCars)
            {
                item.carInstance.position += item.carInstance.forward * item.speed * Time.deltaTime;

                if (Vector3.Distance(item.carInstance.position, item.destinationPoint.position) < distanceToDestoryCar)
                {
                    m_TraficCarsToRemove.Add(item);
                }
            }

            foreach (var item in m_TraficCarsToRemove)
            {
                m_TraficCars.Remove(item);
                GameObject.Destroy(item.carInstance.gameObject);
            }

            m_TraficCarsToRemove.Clear();
        }

        private void UpdateSpawner()
        {
            m_SpawnT += Time.deltaTime;

            if (m_SpawnT > m_NextSpawnInveral)
            {
                m_NextSpawnInveral = GetNextSpawnIntarval();
                m_SpawnT = 0;
                Transform start = rightSpawnPoint;
                Transform end = rightSpawnPointEnd;
                var carPrefab = carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)];
                var carInstance = GameManager.Instantiate(carPrefab, start.position, start.rotation);

                var carData = new TraficCarData
                {
                    carInstance = carInstance.transform,
                    destinationPoint = end,
                    speed = MathHelper.GetMPHSpeed(carSpeedMPH)
                };

                m_TraficCars.Add(carData);
            }
        }
    }
}