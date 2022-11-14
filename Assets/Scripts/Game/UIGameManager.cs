using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIGameManager : MonoBehaviour {
   [SerializeField] private GameObject GameCanvas;
   [SerializeField] private GameObject PauseCanvas;
   [SerializeField] private GameObject Timer;

   private float temp = 3;
   private float updateTime = 1;
   private float countTime = 0;
   private bool finishCount = false;

   void Start () {
      CanvasManager.instance.OnGameMenu += ActivateGameMenu;
      CanvasManager.instance.OnPauseMenu += ActivatePauseMenu;

      CanvasManager.instance.GameMenu();

   }
   void Update () {
      temp -= Time.deltaTime;
      if (!finishCount && Time.time >= countTime) {
         countTime = Time.time + updateTime;
         if (temp <= 0.5f) {
            Timer.GetComponent<Text>().text = "";
            finishCount = true;
         } else {
            Timer.transform.DOScale(new Vector3(0, 0, 0), 0);
            Timer.GetComponent<Text>().text = temp.ToString("F0");
            Timer.transform.DOScale(new Vector3(1, 1, 1), 0.7f);
         }
      }
   }
   private void ActivateGameMenu () {
      GameCanvas.transform.GetChild(1).transform.DOScale(new Vector3(1, 1, 1), 0.18f);

      PauseCanvas.SetActive(false);
   }
   private void ActivatePauseMenu () {
      GameCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.18f);

      PauseCanvas.SetActive(true);
      Invoke("PauseGame", 0.2f);
   }
   public void PauseGame () {
      Time.timeScale = 0;
      AudioSource[] audios = FindObjectsOfType<AudioSource>();
      foreach (AudioSource a in audios) {
         a.Pause();
      }
   }
   public void ResumeGame () {
      Time.timeScale = 1;
      AudioSource[] audios = FindObjectsOfType<AudioSource>();
      foreach (AudioSource a in audios) {
         a.Play();
      }
   }
   public void LoadScene (string SceneName) {
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneName);
   }
}
