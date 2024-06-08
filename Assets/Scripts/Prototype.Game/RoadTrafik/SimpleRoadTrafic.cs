using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace Prototype
{
    public class TraficCarData
    {
        public Transform carInstance;
        public SimpleRoadTraficCar traficCar;

        public Transform destinationPoint;
        public float maxSpeed;
        public bool noObstacles;
        public float currentSpeed;
        public float accelerationSpeed;
    }

    public class SimpleRoadTrafic : MonoBehaviour
    {
        public GameObject[] carPrefabs;

        public Transform rightSpawnPoint;
        public Transform rightSpawnPointEnd;

        public RoadCrosswalk crosswalk;

        public float carSpeedMPH = 30;
        public float acceleration = 10f;
        public float spawnIntervalMin = 5;
        public float spawnIntervalMax = 5;
        private float m_NextSpawnInveral;

        private float m_SpawnT;
        public float distanceToDestoryCar = 1f;
        private List<TraficCarData> m_TraficCars = new List<TraficCarData>();
        private List<TraficCarData> m_TraficCarsToRemove = new List<TraficCarData>();

        public LayerMask obstacleRaycastMask;
        private float m_AITick;
        const float aiTickInteval = 0.1f;
        public int maxCarsOnRoad = 3;
        public float obstacleRaycastDistance = 3f;

        public Vector3 castHalfSize = new Vector3(0.7f, 0.5f, 1f);
        public float offsetForward;

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
            UpdateTraficMove();
        }

        private void FixedUpdate()
        {
            UpdateTraficAI();
        }

        private void UpdateTraficMove()
        {
            foreach (var item in m_TraficCars)
            {
                var aceeleration = item.accelerationSpeed;
                if (!item.noObstacles)
                {
                    aceeleration *= -1;
                }

                item.currentSpeed += aceeleration * Time.deltaTime;

                item.currentSpeed = Mathf.Clamp(item.currentSpeed, 0, item.maxSpeed);
                item.carInstance.position += item.carInstance.forward * item.currentSpeed * Time.deltaTime;

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
        private void UpdateTraficAI()
        {
            //m_AITick += Time.deltaTime;

            //if (m_AITick < aiTickInteval)
            //    return;

            //m_AITick = 0;
            var halfSize = castHalfSize;
            float rayDistance = obstacleRaycastDistance;
            float forwardOffset = offsetForward;

            foreach (var item in m_TraficCars)
            {
                var origin = item.traficCar.front.position + new Vector3(0, 0.2f, 0) + item.traficCar.front.forward * forwardOffset;

               
                bool hasObstacle = Physics.BoxCast(origin, halfSize, item.traficCar.front.forward, out RaycastHit hit, item.traficCar.front.rotation, rayDistance, obstacleRaycastMask);

                ExtDebug.DrawBoxCastBox(origin, halfSize, item.traficCar.front.rotation, item.traficCar.front.forward, rayDistance, Color.red);

                if (hasObstacle)
                {
                    bool hasCustomer = hit.collider.GetComponent<CustomerAI>();
                    bool hasCar = hit.collider.GetComponent<SimpleRoadTraficCar>();
                    item.noObstacles = !hasCustomer && !hasCar;
                }
                else
                {
                    item.noObstacles = true;
                }
            }
        }

        private void UpdateSpawner()
        {
            m_SpawnT += Time.deltaTime;

            if (m_SpawnT > m_NextSpawnInveral)
            {
                m_NextSpawnInveral = GetNextSpawnIntarval();
                m_SpawnT = 0;

                if (m_TraficCars.Count > maxCarsOnRoad)
                    return;

                Transform start = rightSpawnPoint;
                Transform end = rightSpawnPointEnd;
                var carPrefab = carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)];
                var carInstance = GameManager.Instantiate(carPrefab, start.position, start.rotation).GetComponent<SimpleRoadTraficCar>();

                var carData = new TraficCarData
                {
                    carInstance = carInstance.transform,
                    destinationPoint = end,
                    maxSpeed = MathHelper.GetMPHSpeed(carSpeedMPH),
                    traficCar = carInstance,
                    accelerationSpeed = acceleration,
                };

                m_TraficCars.Add(carData);
            }
        }
    }
}