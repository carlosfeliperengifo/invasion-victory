using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
   [SerializeField] private Image lifeBar;
   private int lifePoints = 100;
   private int maxLifePoints = 100;
   private float lifeRecovTime = 0;
   private float recoveryTime = 4;

   [SerializeField] private Image damage;

   public int LifePoints { get { return lifePoints; } }
   void Update () {
      if (Time.time > lifeRecovTime) {
         lifeRecovTime = Time.time + recoveryTime;
         lifePoints += 1;
         if (lifePoints > maxLifePoints) { lifePoints = maxLifePoints; }
         lifeBar.fillAmount = (float)(lifePoints) / (float)(maxLifePoints);
      }
   }
   private void OnTriggerEnter (Collider other) {
      switch (other.tag) {
      case "EnemyBullet":
         Handheld.Vibrate();
         lifePoints -= 2;
         damage.color = new Color(1f, 0f, 0f, 0.5f);
         Destroy(other.gameObject);
         Invoke("HideDamage", 0.12f);
         break;
      case "EnemyPlane":
         Handheld.Vibrate();
         lifePoints -= 10;
         damage.color = new Color(1f, 0f, 0f, 0.6f);
         Invoke("HideDamage", 0.25f);
         break;
      }
      if (lifePoints < 0) {
         lifePoints = 0;
      }
      lifeBar.fillAmount = (float)(lifePoints) / (float)(maxLifePoints);
   }
   private void HideDamage () {
      damage.color = new Color(1f, 0f, 0f, 0f);
   }
}
