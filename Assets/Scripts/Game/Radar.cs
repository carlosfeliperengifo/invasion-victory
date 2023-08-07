using System.Collections;
using UnityEngine;

public class Radar : MonoBehaviour {
   [SerializeField] private Transform player;
   [SerializeField] private GameObject EnemyPoint;
   [SerializeField] private GameControl gameControl;

   private const float updateTime = 0.1f;
   private int count;

   private float maxSpawnDist;
   private float radiusScale;

   void Start () {
      maxSpawnDist = gameControl.MaxSpawnDist;
      radiusScale = 155f / maxSpawnDist;
      count = 10;
      StartCoroutine(UpdatePoints());
   }
   // Subroutine to update the radar every updateTime
   private IEnumerator UpdatePoints () {
      // Update radar rotation
      transform.localRotation = new Quaternion(0, 0, player.transform.rotation.y, player.transform.rotation.w);
      if (count >= 10) {
         RemoveRadarPoints();
         AddRadarPoints();
         count = 1;
      } else {
         count++;
      }
      yield return new WaitForSeconds(updateTime);
      StartCoroutine(UpdatePoints());
   }
   // Add one point per enemy on the radar
   private void AddRadarPoints () {
      GameObject[] enemies = GameObject.FindGameObjectsWithTag("SpaceShip");
      foreach (GameObject enemy in enemies) {
         var posEnemy = enemy.transform.position - player.transform.position;
         if (Vector3.Distance(player.transform.position, enemy.transform.position) <= maxSpawnDist) {
            GameObject radarPoint = Instantiate(EnemyPoint, transform);
            radarPoint.transform.localPosition = new Vector3(posEnemy.x * radiusScale, posEnemy.z * radiusScale, 0);
         }
      }
   }
   // Remove enemy points on the radar
   private void RemoveRadarPoints () {
      GameObject[] enemyPoints = GameObject.FindGameObjectsWithTag("RadarPoint");
      foreach (GameObject enemy in enemyPoints) {
         Destroy(enemy);
      }
   }
}
