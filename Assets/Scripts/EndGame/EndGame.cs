using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class EndGame : MonoBehaviour {
   [SerializeField] private Transform ConnectionCanvas;
   [SerializeField] private Transform GameOverCanvas;
   [SerializeField] private Transform CongratulationsCanvas;

   [SerializeField] private AudioSource gameOverSound;
   [SerializeField] private AudioSource congratSound;
   [SerializeField] private RawImage background;

   private WWWForm sesionForm;
   private WWWForm mathForm;
   private WWWForm performanceForm;

   private bool completedGame;
   private int score;
   private string timeGame;

   private bool isConnected = false;

   public static EndGame States;
   private void Awake () {
      if (States != null && States != this) {
         Destroy(gameObject);
      } else {
         States = this;
      }
   }
   private void LoadSesion () {
      sesionForm = new WWWForm();
      string[] datos = GetDataTxt("User");
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            sesionForm.AddField(col[0], col[1]);
         }
      }
   }
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
   private string[] GetDataTxt (string name) {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/" + name + ".txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      Datostxt.Close();
      return datos;
   }
   public IEnumerator Connection () {
      if (Application.internetReachability == NetworkReachability.NotReachable) {
         ShowCanvas(ConnectionCanvas);
         GameObject.Find("BtRetry").transform.localScale = new Vector3(0, 0, 0);
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
   public IEnumerator InsertSessionDB () {
      LoadPerformance();
      LoadSesion();
      string url = "https://semilleroarvrunicauca.com/invasion-victory/sesion.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, sesionForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GlobalManager.events.failed4();
         } else { // Servidor ok
            Debug.Log("Se: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("seid", datos[1].ToString());
               GlobalManager.events.success3();
            } else {
               GlobalManager.events.failed4();
            }
         }
      }
   }
   public IEnumerator InsertMatchDB () {
      LoadMatch();
      string url = "https://semilleroarvrunicauca.com/invasion-victory/match.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, mathForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GlobalManager.events.failed4();
         } else { // Servidor ok
            Debug.Log("Ma: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("maid", datos[1].ToString());
               GlobalManager.events.success4();
            } else {
               GlobalManager.events.failed4();
            }
         }
      }
   }
   public IEnumerator InsertPerformanceDB () {
      string url = "https://semilleroarvrunicauca.com/invasion-victory/performance.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, performanceForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GlobalManager.events.failed4();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2":
               Debug.Log("Performance registrado");
               GlobalManager.events.success5();
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               GlobalManager.events.failed4();
               break;
            }
         }
      }
   }
   public void GetDataGame () {
      string[] datos = GetDataTxt("Peformance");
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "completedGame":
            completedGame = bool.Parse(col[1]);
            break;
         case "score":
            score = int.Parse(col[1]);
            break;
         case "timePlayed":
            var t = float.Parse(col[1]);
            var mn = Mathf.FloorToInt(t / 60);
            var sg = Mathf.FloorToInt(t % 60);
            timeGame = string.Format("{0:00}:{1:00}", mn, sg);
            break;
         }
      }
      GameObject.Find("BtRetry").transform.localScale = new Vector3(1, 1, 1);
      GlobalManager.events.completedGame(completedGame);
   }
   public void GameOver () {
      HideAllCanvas();
      ShowCanvas(GameOverCanvas);
      background.color = new Color(0.627f, 0.117f, 0.039f, 1);
      GameOverCanvas.GetChild(0).GetChild(2).GetComponent<Text>().text = score.ToString();
      GameOverCanvas.GetChild(0).GetChild(4).GetComponent<Text>().text = timeGame;
      gameOverSound.Play();
   }
   public void Congratulations () {
      HideAllCanvas();
      ShowCanvas(CongratulationsCanvas);
      background.color = new Color(0.588f, 0.117f, 0.588f, 1);
      CongratulationsCanvas.GetChild(0).GetChild(2).GetComponent<Text>().text = score.ToString();
      CongratulationsCanvas.GetChild(0).GetChild(4).GetComponent<Text>().text = timeGame;
      congratSound.Play();
   }
   private void HideAllCanvas () {
      ConnectionCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
      GameOverCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
      CongratulationsCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
   }
   private void ShowCanvas (Transform canvas) {
      float duration = 0.3f;
      for (int i = 0; i < canvas.childCount; i++) {
         canvas.GetChild(i).DOScale(new Vector3(1, 1, 1), duration);
      }
   }
}
