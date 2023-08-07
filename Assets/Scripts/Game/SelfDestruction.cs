using System.Collections;
using UnityEngine;

public class SelfDestruction : MonoBehaviour {
   [SerializeField] private float lifeTime;
   [SerializeField] private float speed = 1;
   // Destroy the object after lifeTime seconds
   private IEnumerator Start () {
      // Add speed to the object
      GetComponent<Rigidbody>().velocity = transform.forward * speed;
      yield return new WaitForSeconds(lifeTime);
      Destroy(gameObject);
   }
}
