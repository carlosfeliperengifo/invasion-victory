using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using System;

public class EndGame : MonoBehaviour {
   // Transform variables across different panels
   [SerializeField] private Transform ConnectionPanel;
   [SerializeField] private Transform GameOverPanel;
   [SerializeField] private Transform CongratulationsPanel;
   [SerializeField] private Transform PerformancePanel;
   // Audios for each panel
   [SerializeField] private AudioSource gameOverSound;
   [SerializeField] private AudioSource congratSound;
   [SerializeField] private RawImage background;
   // Images of the game levels
   [SerializeField] private Sprite[] Level;
   // Texts for performance display
   [SerializeField] private Text txScore;
   [SerializeField] private Text txTime;
   [SerializeField] private Text txPoints;
   [SerializeField] private Image imLevel;
   [SerializeField] private Transform txUnlocked;
   // Forms for the connection with the database
   private WWWForm sesionForm;
   private WWWForm mathForm;
   private WWWForm performanceForm;

   private bool completedGame;
   private int idUser;

   private bool isConnected = false;

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";
   // Save sesionForm with user data
   private void LoadSesion () {
      sesionForm = new WWWForm();
      string[] datos = GetDataTxt("User");
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col[0] == "usid") {
            idUser = int.Parse(col[1]);
         }
         sesionForm.AddField(col[0], col[1]);
      }
   }
   // Save mathForm with user data
   private void LoadMatch () {
      mathForm = new WWWForm();
      string[] datos = GetDataTxt("Match");
      string serial = "";
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            col[1] = col[1].Replace(',', '.');
            if (col[0] == "speedSpaceships" && col[1] == "1") {
               col[1] = "1.0";
            }
            mathForm.AddField(col[0], col[1]);
            serial += col[1] + ".";
         }
      }
      serial = serial.Remove(serial.Length - 1);
      mathForm.AddField("serial", serial);
   }
   // Save performanceForm with user data
   private void LoadPerformance () {
      performanceForm = new WWWForm();
      string[] datos = GetDataTxt("Performance");
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            if (col[0] == "completedGame") {
               if (bool.Parse(col[1])) {
                  col[1] = "1";
               } else { col[1] = "0"; }
            }
            col[1] = col[1].Replace(',', '.');
            performanceForm.AddField(col[0], col[1]);
         }
      }
   }
   // Retrieve data from a txt file
   private string[] GetDataTxt (string name) {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/" + name + ".txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      return datos;
   }
   // Check internet connection
   public IEnumerator Connection () {
      if (Application.internetReachability == NetworkReachability.NotReachable) {
         HideAllPanels();
         ShowCanvas(ConnectionPanel);
         background.color = new Color(0.431f, 0.471f, 0.51f, 1f);
         isConnected = false;
      } else {
         isConnected = true;
         GlobalManager.events.conn_ok();
      }
      yield return new WaitForSeconds(10);
      if (!isConnected) {
         StartCoroutine(Connection());
      }
   }
   // Add user session data to the database
   public IEnumerator InsertSessionDB () {
      LoadSesion();
      LoadPerformance();
      string url = dataPath + "/sesion.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, sesionForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            wr.Dispose();
            GlobalManager.events.failed4();
         } else { // Server ok
            Debug.Log("Se: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("seid", datos[1].ToString());
               wr.Dispose();
               GlobalManager.events.success3();
            } else {
               wr.Dispose();
               GlobalManager.events.failed4();
            }
         }
      }
   }
   // Add user match data to the database
   public IEnumerator InsertMatchDB () {
      LoadMatch();
      string url = dataPath + "/match.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, mathForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            wr.Dispose();
            GlobalManager.events.failed4();
         } else { // Servidor ok
            Debug.Log("Ma: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("maid", datos[1].ToString());
               wr.Dispose();
               GlobalManager.events.success4();
            } else {
               wr.Dispose();
               GlobalManager.events.failed4();
            }
         }
      }
   }
   // Add user performance data to the database
   public IEnumerator InsertPerformanceDB () {
      string url = dataPath + "/performance.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, performanceForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            wr.Dispose();
            GlobalManager.events.failed4();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2":
               Debug.Log("Performance registrado");
               StartCoroutine(UpdateLevel());
               //GlobalManager.events.success5();
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               wr.Dispose();
               GlobalManager.events.failed4();
               break;
            }
         }
      }
   }
   // Update player level in the database
   private IEnumerator UpdateLevel () {
      WWWForm form = new WWWForm();
      form.AddField("usid", idUser);
      form.AddField("option", "update");
      string url = dataPath + "/ranking.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            wr.Dispose();
            GlobalManager.events.success5();
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
            wr.Dispose();
            if (datos.Length == 3) {
               if (datos[0] == "1") {
                  int rankId = int.Parse(datos[1]);
                  imLevel.sprite = Level[rankId - 1];
                  txPoints.text = datos[2];
                  if (rankId > PlayerPrefs.GetInt("idLevel", 10)) {
                     txUnlocked.localScale = Vector3.one;
                  } else {
                     txUnlocked.localScale = Vector3.zero;
                  }
               }
            }
            wr.Dispose();
            GlobalManager.events.success5();
         }
      }
   }
   // Load data from the last game
   public void GetDataGame () {
      HideAllPanels();
      string[] datos = GetDataTxt("Performance");
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "completedGame":
            completedGame = bool.Parse(col[1]);
            break;
         case "score":
            txScore.text = col[1];
            break;
         case "timePlayed":
            var t = float.Parse(col[1]);
            var mn = Mathf.FloorToInt(t / 60);
            var sg = Mathf.FloorToInt(t % 60);
            txTime.text = string.Format("{0:00}:{1:00}", mn, sg);
            break;
         }
      }
      ShowCanvas(PerformancePanel);
      GlobalManager.events.completedGame(completedGame);
   }

   // Displays the GameOver screen
   public void GameOver () {
      ShowCanvas(GameOverPanel);
      background.color = new Color(0.627f, 0.117f, 0.039f, 1);
      gameOverSound.Play();
   }
   // Displays the Congratulations screen
   public void Congratulations () {
      ShowCanvas(CongratulationsPanel);
      background.color = new Color(0.588f, 0.117f, 0.588f, 1);
      congratSound.Play();
   }
   // Hide all panels
   private void HideAllPanels () {
      ConnectionPanel.localScale = Vector3.zero;
      GameOverPanel.localScale = Vector3.zero;
      CongratulationsPanel.localScale = Vector3.zero;
      PerformancePanel.localScale = Vector3.zero;
   }
   // Dynamic display of the panel
   private void ShowCanvas (Transform panel) {
      float duration = 0.3f;
      panel.DOScale(Vector3.one, duration);
   }
}
