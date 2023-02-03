using UnityEngine;

public class Projectile : MonoBehaviour {
   [SerializeField] private GameObject explosion;
   private void OnTriggerEnter (Collider other) {
      if (other.tag == "PlayerBullet") {
         Destroy(other.gameObject);
         Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.3f);
         Destroy(gameObject);
      }
   }
}
