using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Video;

public class Main : MonoBehaviour {
   [SerializeField] private GameObject LoginCanvas;
   [SerializeField] private GameObject RegisterCanvas;
   [SerializeField] private GameObject RecoverCanvas;
   [SerializeField] private GameObject ConsentCanvas;
   [SerializeField] private GameObject HistoryCanvas;

   private bool inHistory = false;

   public static Main States;
   private void Awake () {
      if (States != null && States != this) {
         Destroy(gameObject);
      } else {
         States = this;
      }
   }
   private void Update () {
      var frames = HistoryCanvas.GetComponentInChildren<VideoPlayer>().frame;

      if (inHistory && frames > 0 && !HistoryCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inHistory = false;
         GlobalManager.events.bt_skip1();
      }
   }
   public void Login () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   public void Register () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   public void Recover () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
   }
   public void Consent () {
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      TextAsset consent = Resources.Load<TextAsset>("InformedConsent");
      ConsentCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = consent.text;
   }
   public void History () {
      HistoryCanvas.SetActive(true);
      HistoryCanvas.GetComponentInChildren<VideoPlayer>().Play();
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      inHistory = true;
   }

   public void LoginDB () {
      LoginUserDB.instance.LoginUser();
   }
   public void RegisterDB () {
      RegisterUserDB.instance.RegisterUser();
   }
   public void UpdatePassDB () {
      RecoverDB.instance.EditPassword();
   }
}

