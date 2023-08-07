using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Main : MonoBehaviour {
   // Transform variables of the different screens
   [SerializeField] private Transform LoginPanel;
   [SerializeField] private Transform RegisterPanel;
   [SerializeField] private Transform RecoverPanel;
   [SerializeField] private Transform ConsentPanel;
   [SerializeField] private GameObject HistoryCanvas;

   private bool inHistory = false;
   // Scripts for server connection
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
   // Displays the Login screen
   public void Login () {
      ShowCanvas("Login");
      registerScript.CleanAllDatas();
      recoverScript.CleanAllDatas();
   }
   // Displays the Register screen
   public void Register () {
      StartCoroutine(registerScript.GetQuestions());
      ShowCanvas("Register");
      loginScript.CleanAllDatas();
   }
   // Displays the Recover screen
   public void Recover () {
      StartCoroutine(recoverScript.GetQuestions());
      ShowCanvas("Recover");
      loginScript.CleanAllDatas();
   }
   // Displays the Consent screen
   public void Consent () {
      TextAsset consent = Resources.Load<TextAsset>("InformedConsentEs");
      ConsentPanel.GetChild(1).GetComponentInChildren<Text>().text = consent.text;
      ShowCanvas("Consent");
   }
   // Displays the History video
   public void History () {
      ShowCanvas("History");
      HistoryCanvas.GetComponentInChildren<VideoPlayer>().Play();
      inHistory = true;
   }
   // Call the LoginUser method to login
   public void LoginDB () {
      loginScript.LoginUser();
   }
   // Call the RegisterDB method to register
   public void RegisterDB () {
      registerScript.RegisterUser();
   }
   // Call the UpdatePassDB method to update the password
   public void UpdatePassDB () {
      recoverScript.EditPassword();
   }
   // Dynamic display of the panel.
   private void ShowCanvas (string panel) {
      float duration = 0.3f;
      switch (panel) {
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
