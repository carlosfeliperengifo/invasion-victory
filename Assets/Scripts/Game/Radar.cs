using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {
   private GameObject EnemyPoint;
   private float updateTime = 1;
   private float currentTime = 0;

   private float maxSpawnDist = 12;
   private float radiusScale;

   void Start () {
      EnemyPoint = transform.GetChild(1).gameObject;
      maxSpawnDist = GameObject.Find("Generator").GetComponent<SpaceshipsGenerator>().MaxSpawnDist;
      radiusScale = 155f / maxSpawnDist;
   }

   void Update () {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      transform.localRotation = new Quaternion(0, 0, player.transform.rotation.y, player.transform.rotation.w);
      if (Time.time >= currentTime) {
         currentTime = Time.time + updateTime;
         DeleteRadarPoints();
         GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyPlane");

         GameObject radarPoint;
         Vector3 posEnemy;
         for (int i = 0; i < enemies.Length; i++) {
            posEnemy = enemies[i].transform.position - player.transform.position;
            if (Vector3.Distance(player.transform.position, enemies[i].transform.position) <= maxSpawnDist) {
               radarPoint = Instantiate(EnemyPoint);
               radarPoint.transform.SetParent(transform, true);
               radarPoint.tag = "RadarPoint";
               radarPoint.transform.localPosition = new Vector3(posEnemy.x * radiusScale, posEnemy.z * radiusScale, 0);
               radarPoint.SetActive(true);
            }
         }
      }
   }

   private void DeleteRadarPoints () {
      GameObject[] enemyPoints = GameObject.FindGameObjectsWithTag("RadarPoint");
      foreach (GameObject enemy in enemyPoints) {
         Destroy(enemy);
      }
   }
}
