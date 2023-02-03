using System.Collections;
using UnityEngine;

public class SelfDestruction : MonoBehaviour {
   [SerializeField] private float lifeTime;
   [SerializeField] private float speed = 1;
   
   private IEnumerator Start () {
      GetComponent<Rigidbody>().velocity = transform.forward * speed;
      yield return new WaitForSeconds(lifeTime);
      Destroy(gameObject);
   }
}
