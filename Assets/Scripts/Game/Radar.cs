using System.Collections;
using UnityEngine;

public class Radar : MonoBehaviour {
   [SerializeField] private Transform player;
   [SerializeField] private GameObject EnemyPoint;

   private const float updateTime = 0.1f;
   private int count;

   private float maxSpawnDist;
   private float radiusScale;

   void Start () {
      try {
         maxSpawnDist = GameObject.Find("GameControl").GetComponent<GameControl>().MaxSpawnDist;
      } catch {
         maxSpawnDist = 12;
      }
      radiusScale = 155f / maxSpawnDist;
      count = 10;
      StartCoroutine(UpdatePoints());
   }

   private IEnumerator UpdatePoints () {
      transform.localRotation = new Quaternion(0, 0, player.transform.rotation.y, player.transform.rotation.w);
      if (count >= 10) {
         DeleteRadarPoints();
         GameObject[] enemies = GameObject.FindGameObjectsWithTag("SpaceShip");

         foreach (GameObject enemy in enemies) {
            var posEnemy = enemy.transform.position - player.transform.position;
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <= maxSpawnDist) {
               GameObject radarPoint = Instantiate(EnemyPoint, transform);
               radarPoint.transform.localPosition = new Vector3(posEnemy.x * radiusScale, posEnemy.z * radiusScale, 0);
            }
         }
         count = 0;
      } else {
         count++;
      }

      yield return new WaitForSeconds(updateTime);
      StartCoroutine(UpdatePoints());
   }

   private void DeleteRadarPoints () {
      GameObject[] enemyPoints = GameObject.FindGameObjectsWithTag("RadarPoint");
      foreach (GameObject enemy in enemyPoints) {
         Destroy(enemy);
      }
   }
}
