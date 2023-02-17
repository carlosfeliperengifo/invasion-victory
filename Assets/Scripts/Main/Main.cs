using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Main : MonoBehaviour {
   [SerializeField] private Transform LoginCanvas;
   [SerializeField] private Transform RegisterCanvas;
   [SerializeField] private Transform RecoverCanvas;
   [SerializeField] private Transform ConsentCanvas;
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
      ShowCanvas("Login");
   }
   public void Register () {
      ShowCanvas("Register");
   }
   public void Recover () {
      ShowCanvas("Recover");
   }
   public void Consent () {
      TextAsset consent = Resources.Load<TextAsset>("InformedConsent");
      ConsentCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = consent.text;
      ShowCanvas("Consent");
   }
   public void History () {
      ShowCanvas("History");
      HistoryCanvas.GetComponentInChildren<VideoPlayer>().Play();
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
   private void ShowCanvas (string canvas) {
      float duration = 0.3f;
      switch (canvas) {
      case "Login":
         LoginCanvas.GetChild(0).DOScale(new Vector3(1, 1, 1), duration);
         RegisterCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         RecoverCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         break;
      case "Register":
         LoginCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         RegisterCanvas.GetChild(0).DOScale(new Vector3(1, 1, 1), duration);
         ConsentCanvas.GetChild(0).DOScale(new Vector3(0, 0, 0), duration);
         break;
      case "Recover":
         LoginCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         RecoverCanvas.GetChild(0).DOScale(new Vector3(1, 1, 1), duration);
         break;
      case "Consent":
         ConsentCanvas.GetChild(0).DOScale(new Vector3(1, 1, 1), duration);
         break;
      case "History":
         LoginCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         RegisterCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         RecoverCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
         HistoryCanvas.SetActive(true);
         break;
      default:
         break;
      }
   }
}
