using UnityEngine;

public class Projectile : MonoBehaviour {
   // Animation of explotion
   [SerializeField] private GameObject explosion;
   // If it collides with a PlayerBullet, the Projectile is destroyed
   private void OnTriggerEnter (Collider other) {
      if (other.tag == "PlayerBullet") {
         Destroy(other.gameObject);
         Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.3f);
         Destroy(gameObject);
      }
   }
}
