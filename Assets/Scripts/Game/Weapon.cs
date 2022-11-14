using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {
   [SerializeField] private GameObject bullet;
   [SerializeField] private Transform SpawnPoint;

   private float shotForce = 1000;
   private float shotDelay = 0.4f;
   private float delayTime = 0;

   [SerializeField] private GameObject shotSound;
   private Animator animator;
   private ParticleSystem fire;

   private static int maxMunition = 99;
   private int munition;

   private bool isReload;
   private bool isAutoShot = false;

   public bool IsAutoShot { set { isAutoShot = value; } }

   void Start () {
      isReload = false;
      animator = transform.GetComponent<Animator>();
      fire = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
      munition = maxMunition;
      GameObject.Find("txMun").GetComponent<Text>().text = munition.ToString();
   }
   void Update () {
      if (munition <= 0 && !isReload) {
         Recharge();
      }
      if (isAutoShot && !isReload) {
         if (Time.time > delayTime && !isReload && munition > 0) {
            delayTime = Time.time + shotDelay;
            animator.SetTrigger("Shoot");
            Destroy(Instantiate(shotSound), 0.18f);
            GenBullet();
            munition -= 1;
            GameObject.Find("txMun").GetComponent<Text>().text = munition.ToString();
         }
      }
   }
   public void Recharge () {
      if (!isReload) {
         isReload = true;
         CancelInvoke();
         fire.Stop();
         animator.SetTrigger("DoReload");
         Invoke("outReload", 4f);
      }
   }
   public void ShootBurst () {
      shotDelay = 0.4f;
      if (Time.time > delayTime && !isReload && munition > 0) {
         var n = 3;
         animator.SetTrigger("Shoot");
         fire.Play();
         Invoke("outFire", 0.34f);
         Destroy(Instantiate(shotSound), 0.3f);
         GenBullet();
         for (int i = 1; i < n; i++) {
            if (munition > 0) {
               Invoke("GenBullet", i / 5.5f);
               munition -= 1;
               GameObject.Find("txMun").GetComponent<Text>().text = munition.ToString();
            }
         }
         delayTime = Time.time + shotDelay;
      }
   }
   public void AutoShot (bool st) {
      if (st) {
         shotDelay = 0.18f;
         fire.Play();
      }
   }
   private void GenBullet () {
      GameObject newBullet;
      newBullet = Instantiate(bullet, SpawnPoint.position, SpawnPoint.rotation);
      newBullet.GetComponent<Rigidbody>().AddForce(SpawnPoint.forward * shotForce);
      Destroy(newBullet, 1.5f);
   }
   public void outFire () {
      fire.Stop();
   }
   private void outReload () {
      fire.Stop();
      isReload = false;
      munition = maxMunition;
      GameObject.Find("txMun").GetComponent<Text>().text = munition.ToString();
   }
}
