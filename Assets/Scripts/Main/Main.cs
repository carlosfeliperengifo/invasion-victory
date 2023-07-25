using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Main : MonoBehaviour {
   [SerializeField] private Transform LoginPanel;
   [SerializeField] private Transform RegisterPanel;
   [SerializeField] private Transform RecoverPanel;
   [SerializeField] private Transform ConsentPanel;
   [SerializeField] private GameObject HistoryCanvas;

   private bool inHistory = false;

   [SerializeField] private LoginUserDB loginScript;
   [SerializeField] private RegisterUserDB registerScript;
   [SerializeField] private RecoverDB recoverScript;

   private void Update () {
      var frames = HistoryCanvas.GetComponentInChildren<VideoPlayer>().frame;
      if (inHistory && frames > 0 && !HistoryCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inHistory = false;
         GlobalManager.events.bt_skip1();
      }
   }
   public void Login () {
      ShowCanvas("Login");
      registerScript.CleanAllDatas();
      recoverScript.CleanAllDatas();
   }
   public void Register () {
      StartCoroutine(registerScript.GetQuestions());
      ShowCanvas("Register");
      loginScript.CleanAllDatas();
   }
   public void Recover () {
      StartCoroutine(recoverScript.GetQuestions());
      ShowCanvas("Recover");
      loginScript.CleanAllDatas();
   }
   public void Consent () {
      TextAsset consent = Resources.Load<TextAsset>("InformedConsentEs");
      ConsentPanel.GetChild(1).GetComponentInChildren<Text>().text = consent.text;
      ShowCanvas("Consent");
   }
   public void History () {
      ShowCanvas("History");
      HistoryCanvas.GetComponentInChildren<VideoPlayer>().Play();
      inHistory = true;
   }

   public void LoginDB () {
      loginScript.LoginUser();
   }
   public void RegisterDB () {
      registerScript.RegisterUser();
   }
   public void UpdatePassDB () {
      recoverScript.EditPassword();
   }
   private void ShowCanvas (string canvas) {
      float duration = 0.3f;
      switch (canvas) {
      case "Login":
         RegisterPanel.localScale = Vector3.zero;
         RecoverPanel.localScale = Vector3.zero;
         ConsentPanel.localScale = Vector3.zero;
         LoginPanel.DOScale(Vector3.one, duration);
         break;
      case "Register":
         LoginPanel.localScale = Vector3.zero;
         RegisterPanel.DOScale(Vector3.one, duration);
         ConsentPanel.DOScale(Vector3.zero, duration);
         break;
      case "Recover":
         LoginPanel.localScale = Vector3.zero;
         RecoverPanel.DOScale(Vector3.one, duration);
         break;
      case "Consent":
         ConsentPanel.DOScale(Vector3.one, duration);
         break;
      case "History":
         LoginPanel.localScale = Vector3.zero;
         RegisterPanel.localScale = Vector3.zero;
         RecoverPanel.localScale = Vector3.zero;
         HistoryCanvas.SetActive(true);
         break;
      default:
         break;
      }
   }
}
