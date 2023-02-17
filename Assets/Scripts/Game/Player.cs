using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
   [SerializeField] private Image lifeBar;
   [SerializeField] private Image damage;
   [SerializeField] private GameObject bullet;
   [SerializeField] private Transform SpawnPoint;
   [SerializeField] private Animator animator;
   [SerializeField] private ParticleSystem fire;
   [SerializeField] private AudioSource shotSound;
   [SerializeField] private Text txMun;

   private const int maxLifePoints = 100;
   private int lifePoints = maxLifePoints;
   private const float recoveryTime = 4;

   private const float shotDelay = 0.18f;
   private int burstShots;
   private const int maxMunition = 99;
   private int munition = maxMunition;

   private enum State {
      Available, Recharging, ShootingBurst, Shooting
   }
   public enum Transition {
      wait, recharge, shootBurst, autoShot
   }

   private State weapon = State.Available;

   public int LifePoints { get { return lifePoints; } }

   private IEnumerator Start () {
      yield return new WaitForSeconds(recoveryTime);
      if (lifePoints < maxLifePoints) {
         lifePoints++;
         lifeBar.fillAmount = lifePoints / (float)(maxLifePoints);
      }
      StartCoroutine(Start());
   }

   private void PlayerStates () {
      switch (weapon) {
      case State.Available:
         fire.Stop();
         break;
      case State.Recharging:
         fire.Stop();
         animator.SetTrigger("DoReload");
         Invoke("OutReload", 4f);
         break;
      case State.ShootingBurst:
         burstShots = 3;
         StartCoroutine(GenerateBullet());
         break;
      case State.Shooting:
         StartCoroutine(GenerateBullet());
         break;
      }
   }
   public void PlayerTransitions (Transition t) {
      if (weapon == State.Available) {
         switch (t) {
         case Transition.recharge:
            if (munition < maxMunition) {
               weapon = State.Recharging;
            }
            break;
         case Transition.shootBurst:
            weapon = State.ShootingBurst;
            break;
         case Transition.autoShot:
            weapon = State.Shooting;
            break;
         default:
            return;
         }
         PlayerStates();
      }
      if (t == Transition.wait) {
         weapon = State.Available;
         PlayerStates();
      }
   }
   public void PlayerTransitionsRecharge () => PlayerTransitions(Transition.recharge);
   private IEnumerator GenerateBullet () {
      if (munition > 0) {
         fire.Play();
         animator.SetTrigger("Shoot");
         shotSound.Play();
         Instantiate(bullet, SpawnPoint.position, SpawnPoint.rotation);
         munition--;
         txMun.text = munition.ToString();
      } else {
         animator.SetTrigger("DoOpen");
         StopCoroutine(GenerateBullet());
         PlayerTransitions(Transition.wait);
      }
      yield return new WaitForSeconds(shotDelay);
      shotSound.Stop();
      switch (weapon) {
      case State.ShootingBurst:
         burstShots--;
         if (burstShots > 0) {
            StartCoroutine(GenerateBullet());
         } else {
            PlayerTransitions(Transition.wait);
         }
         break;
      case State.Shooting:
         StartCoroutine(GenerateBullet());
         break;
      default:
         StopCoroutine(GenerateBullet());
         break;
      }
   }
   private void OutReload () {
      munition = maxMunition;
      txMun.text = munition.ToString();
      PlayerTransitions(Transition.wait);
   }

   private void OnTriggerEnter (Collider other) {
      switch (other.tag) {
      case "Projectile":
         Handheld.Vibrate();
         Destroy(other.gameObject);
         lifePoints -= 2;
         StartCoroutine(Damage(0.5f, 0.12f, true));
         break;
      case "SpaceShip":
         Handheld.Vibrate();
         lifePoints -= 10;
         StartCoroutine(Damage(0.6f, 0.22f, true));
         break;
      default:
         return;
      }
      if (lifePoints < 0) { lifePoints = 0; }
      lifeBar.fillAmount = lifePoints / (float)(maxLifePoints);
   }
   private IEnumerator Damage (float alpha, float duration, bool st) {
      damage.color = new Color(1f, 0f, 0f, alpha);
      if (st) {
         yield return new WaitForSeconds(duration);
         StartCoroutine(Damage(0f, 0f, false));
      }
   }
}
