using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
   [SerializeField] private Image lifeBar;
   [SerializeField] private Image damage;
   [SerializeField] private GameObject bullet;
   [SerializeField] private Transform spawnPoint;
   [SerializeField] private Transform contWeapon;
   [SerializeField] private GameObject[] weapons;
   [SerializeField] private Text txMun;

   private Weapon weaponScript;

   private const int maxLifePoints = 100;
   private int lifePoints = maxLifePoints;
   private const float recoveryTime = 4;

   private const float shotDelay = 0.18f;
   private int burstShots;
   private const int maxMunition = 99;
   private int munition = maxMunition;
   // Player states
   private enum State {
      Available, Recharging, ShootingBurst, Shooting
   }
   // Transitions for the weapon
   public enum Transition {
      wait, recharge, shootBurst, autoShot
   }

   private State weapon = State.Available;

   public int LifePoints { get { return lifePoints; } }

   // Recovers a life point every recoveryTime
   private IEnumerator Start () {
      yield return new WaitForSeconds(recoveryTime);
      if (lifePoints < maxLifePoints) {
         lifePoints++;
         lifeBar.fillAmount = lifePoints / (float)(maxLifePoints);
      }
      StartCoroutine(Start());
   }
   // Instantiate the weapon selected by the user
   public void LoadWeapon () {
      int idWp = PlayerPrefs.GetInt("idWeapon", 0);
      int idMat = PlayerPrefs.GetInt("idMat", 0);
      GameObject wp = Instantiate(weapons[idWp], contWeapon);
      weaponScript = wp.GetComponent<Weapon>();
      weaponScript.ChangeMaterial(idMat);
   }
   // Management of player states
   private void PlayerStates () {
      switch (weapon) {
      case State.Available:
         weaponScript.Repose();
         break;
      case State.Recharging:
         weaponScript.Reload();
         Invoke("OutReload", 3f);
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
   // Management of the events
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
   // Generates a bullet to be fired
   private IEnumerator GenerateBullet () {
      if (munition > 0) {
         weaponScript.Shoot();
         Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
         munition--;
         txMun.text = munition.ToString();
      } else {
         weaponScript.Open();
         StopCoroutine(GenerateBullet());
         PlayerTransitions(Transition.wait);
      }
      yield return new WaitForSeconds(shotDelay);
      weaponScript.Repose();
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
   // Ends the reload state
   private void OutReload () {
      munition = maxMunition;
      txMun.text = munition.ToString();
      PlayerTransitions(Transition.wait);
   }
   // Player collision handling
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
   // Changes the color of the screen when the player taking damage
   private IEnumerator Damage (float alpha, float duration, bool st) {
      damage.color = new Color(1f, 0f, 0f, alpha);
      if (st) {
         yield return new WaitForSeconds(duration);
         StartCoroutine(Damage(0f, 0f, false));
      }
   }
}
