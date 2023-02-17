using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour {
   [SerializeField] private Transform GameCanvas;
   [SerializeField] private GameObject PauseCanvas;
   [SerializeField] private Transform timer;
   [SerializeField] private GameObject infConfig;
   [SerializeField] private GameControl gameControl;

   private int temp = 4;

   public static GameUI States;
   private void Awake () {
      if (States != null && States != this) {
         Destroy(gameObject);
      } else {
         States = this;
      }
   }
   public void Countdown () {
      temp--;
      if (temp > 0) {
         timer.localScale = new Vector3(0, 0, 0);
         timer.GetComponent<Text>().text = temp.ToString();
         timer.DOScale(new Vector3(1, 1, 1), 0.7f);
      } else {
         timer.GetComponent<Text>().text = "";
         CancelInvoke("Countdown");
         GlobalManager.events.time3s();
      }
   }

   public void Game () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
      }
      GameCanvas.GetChild(1).localScale = new Vector3(0, 0, 0);
      PauseCanvas.SetActive(false);
      InvokeRepeating("Countdown", 0f, 1f);
   }
   public void Playing () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
         AudioSource[] audios = FindObjectsOfType<AudioSource>();
         foreach (AudioSource a in audios) {
            if (a.name != "MachineGun_00") {
               a.Play();
            }
         }
      }
      infConfig.SetActive(false);
      GameCanvas.GetChild(1).localScale = new Vector3(1, 1, 1);
      PauseCanvas.SetActive(false);
      gameControl.StartGame();
   }
   public void Pause () {
      infConfig.SetActive(true);
      GameCanvas.GetChild(1).localScale = new Vector3(0, 0, 0);
      PauseCanvas.SetActive(true);
      Invoke("PauseGame", 0.1f);
   }
   public void SavePerformanceTxt () {
      gameControl.SavePerformanceTxt();
   }
   private void PauseGame () {
      Time.timeScale = 0;
      AudioSource[] audios = FindObjectsOfType<AudioSource>();
      foreach (AudioSource a in audios) {
         a.Pause();
      }
   }
}
