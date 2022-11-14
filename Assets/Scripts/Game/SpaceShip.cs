using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShip : MonoBehaviour {
   [SerializeField] private float speed = 1;
   private Rigidbody rbEnemy;

   [SerializeField] private GameObject projectile;
   private float ShotForce = 300;

   private int lifePoints = 5;

   [SerializeField] private GameObject explosion;
   [SerializeField] private GameObject soundimpact;

   private Vector3 evasionVector;
   private int stateEva = 0;

   public float Speed { get { return speed; } set { speed = value; } }

   void Start () {
      rbEnemy = GetComponent<Rigidbody>();
      StartCoroutine(ShootProjectile());
      stateEva = 0;
   }

   void Update () {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      GenerateEvasionVector(player);
      Vector3 vectorObjetive = (player.transform.position - transform.position - evasionVector).normalized;
      var rotacion = Quaternion.LookRotation(vectorObjetive);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacion, 5);
      if (Time.timeScale > 0) {
         rbEnemy.velocity = vectorObjetive * speed;
      }
   }

   IEnumerator ShootProjectile () {
      float shootRate = Random.Range(5f, 8f);
      yield return new WaitForSeconds(shootRate);
      GameObject newProjectile = Instantiate(projectile, transform.position, transform.rotation);
      newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * ShotForce);
      Destroy(newProjectile, 3f);

      StartCoroutine(ShootProjectile());
   }
   private void OnTriggerEnter (Collider other) {
      switch (other.tag) {
      case "PlayerBullet":
         lifePoints -= 1;
         if (lifePoints == 0) {
            Text spaceshipsDest = GameObject.Find("Destroyed").GetComponentInChildren<Text>();
            spaceshipsDest.text = (int.Parse(spaceshipsDest.text) + 1).ToString();
            Destroy(Instantiate(explosion, transform.position, transform.rotation), 2.5f);
            Destroy(gameObject);
         } else {
            StartCoroutine(Damage(true));
            Destroy(Instantiate(soundimpact), 0.3f);
         }
         Destroy(other.gameObject);
         break;
      case "Player":
         Destroy(gameObject);
         break;
      }
   }
   private void GenerateEvasionVector (GameObject player) {
      float evax;
      float evay;
      float evaz;
      float distance = Vector3.Distance(player.transform.position, transform.position);
      if ((distance >= 8.0 && distance <= 10.0) && stateEva != 3) {
         evax = Random.Range(-3f, 3f);
         evay = Random.Range(-1f, 1f);
         evaz = Random.Range(-3f, 3f);
         stateEva = 3;
      } else if ((distance >= 5.0 && distance < 7.0) && stateEva != 2) {
         evax = Random.Range(-1.4f, 1.4f);
         evay = Random.Range(-0.5f, 0.5f);
         evaz = Random.Range(-1.4f, 1.4f);
         stateEva = 2;
      } else if ((distance < 5.0 || distance > 10.0 || (distance >= 7.0 && distance < 8.0)) && stateEva != 1) {
         evax = 0;
         evay = 0;
         evaz = 0;
         stateEva = 1;
      } else {
         evax = evasionVector.x;
         evay = evasionVector.y;
         evaz = evasionVector.z;
      }

      evasionVector = new Vector3(evax, evay, evaz);
   }
   IEnumerator Damage (bool st) {
      if (st) {
         transform.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 1f);
         yield return new WaitForSeconds(0.17f);
         StartCoroutine(Damage(false));
      } else {
         transform.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
      }
   }
}
