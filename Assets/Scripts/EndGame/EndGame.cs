using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class EndGame : MonoBehaviour {
   [SerializeField] private GameObject ConnectionCanvas;
   [SerializeField] private GameObject GameOverCanvas;
   [SerializeField] private GameObject CongratulationsCanvas;

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
   void Start () {
      sesionForm = new WWWForm();
      mathForm = new WWWForm();
      performanceForm = new WWWForm();
      GetSesionTxt();
      GetMatchTxt();
      GetPerformanceTxt();
   }

   private void GetSesionTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            sesionForm.AddField(col[0], col[1]);
         }
      }
   }
   private void GetMatchTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Match.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
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
   private void GetPerformanceTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Performance.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
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
   public IEnumerator Connection () {
      if (Application.internetReachability == NetworkReachability.NotReachable) {
         GameOverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
         CongratulationsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
         ConnectionCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
         GameObject.Find("BtRetry").transform.localScale = new Vector3(0, 0, 0);

         background.color = new Color(0.431f, 0.471f, 0.51f, 1f);

         isConnected = false;
      } else {
         GlobalManager.events.conn_ok();

         isConnected = true;
      }
      yield return new WaitForSeconds(10);
      if (!isConnected) {
         StartCoroutine(Connection());
      }
   }
   public IEnumerator InsertSessionDB () {
      //string url = "http://universalattack.000webhostapp.com/codes/sesion.php";
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
      //string url = "http://universalattack.000webhostapp.com/codes/match.php";
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
      //string url = "http://universalattack.000webhostapp.com/codes/performance.php";
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
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Performance.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      for (int i = 0; i < datos.Length; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
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
      ConnectionCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CongratulationsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      GameOverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

      background.color = new Color(0.627f, 0.117f, 0.039f, 1);
      GameOverCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = score.ToString();
      GameOverCanvas.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = timeGame;
      gameOverSound.Play();
   }
   public void Congratulations () {
      ConnectionCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      GameOverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CongratulationsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

      background.color = new Color(0.588f, 0.117f, 0.588f, 1);
      CongratulationsCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = score.ToString();
      CongratulationsCanvas.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = timeGame;
      congratSound.Play();
   }
}
