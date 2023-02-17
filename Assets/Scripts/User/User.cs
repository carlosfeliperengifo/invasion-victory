using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class User : MonoBehaviour {
   [SerializeField] private Transform RankingCanvas;
   [SerializeField] private Transform ParamCanvas;
   [SerializeField] private Transform CreditsCanvas;
   [SerializeField] private Transform ManualCanvas;
   [SerializeField] private GameObject MoveCanvas;

   private bool inMovement = false;

   [SerializeField] private Sprite[] Medals;
   [SerializeField] private Transform Container;
   [SerializeField] private TopRow PanelTopRow;
   private struct TopUser {
      public string nick;
      public int score;
      public TopUser (string nick, int score) {
         this.nick = nick;
         this.score = score;
      }
   }
   private string nick;

   public static User States;
   private void Awake () {
      if (States != null && States != this) {
         Destroy(gameObject);
      } else {
         States = this;
      }
   }
   private void Update () {
      var frames = MoveCanvas.GetComponentInChildren<VideoPlayer>().frame;
      if (inMovement && frames > 0 && !MoveCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inMovement = false;
         GlobalManager.events.bt_skip2();
      }
   }
   public void Ranking () {
      Message("");
      ShowNick();
      StartCoroutine(GetTopListDB());

      HideAllCanvas();
      ShowCanvas(RankingCanvas);
   }
   public void Parameters () {
      SliderValues.instance.LoadConfigurationTxt();
      SliderValues.instance.LoadMatchTxt();
      HideAllCanvas();
      ShowCanvas(ParamCanvas);
   }
   public void SaveMatchTxt () {
      SliderValues.instance.SaveMatchTxt();
      GlobalManager.events.bt_close2();
   }
   public void Credits () {
      HideAllCanvas();
      TextAsset credits = Resources.Load<TextAsset>("Credits");
      CreditsCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = credits.text;
      ShowCanvas(CreditsCanvas);
   }
   public void Manual () {
      HideAllCanvas();
      ShowCanvas(ManualCanvas);
   }
   public void Movement () {
      Message("Loading . . .");
      MoveCanvas.SetActive(true);
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Play();
      HideAllCanvas();
      inMovement = true;
   }

   private void HideAllCanvas () {
      RankingCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
      RankingCanvas.GetChild(1).localScale = new Vector3(0, 0, 0);
      ParamCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
      CreditsCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
      ManualCanvas.GetChild(0).localScale = new Vector3(0, 0, 0);
   }
   private void ShowCanvas (Transform canvas) {
      float duration = 0.3f;
      int n = canvas.childCount;
      for (int i = 0; i < n; i++) {
         canvas.GetChild(i).DOScale(new Vector3(1, 1, 1), duration);
      }
   }
   private void ShowNick () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col[0] == "nick") {
            nick = col[1];
            GameObject.Find("txtNick").GetComponent<Text>().text = col[1];
         }
      }
   }
   private void Message (string mss) {
      GameObject.Find("txMss").GetComponent<Text>().text = mss;
   }
   private IEnumerator GetTopListDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", nick);
      //string url = "http://universalattack.000webhostapp.com/codes/topUser.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/topUser.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Connection error, try again");
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
            if (datos.Length > 0) {
               List<TopUser> top = new List<TopUser>();
               TextWriter topFile = new StreamWriter(Application.persistentDataPath + "/TopUser.txt", false);

               for (int i = 0; i < datos.Length; i++) {
                  string[] dat = datos[i].Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
                  top.Add(new TopUser(dat[0], int.Parse(dat[1])));
               }
               IEnumerable<TopUser> topOrder = top.OrderByDescending(ts => ts.score);
               foreach (TopUser topUser in topOrder) {
                  topFile.WriteLine(topUser.nick + "\t" + topUser.score.ToString());
               }
               topFile.Close();
               LoadTop10();
            } else {
               Message("Connection error, try again");
            }
         }
         wr.Dispose();
      }
   }
   private void LoadTop10 () {
      Message("Loading . . .");
      GameObject[] topAnt = GameObject.FindGameObjectsWithTag("Top10");
      foreach (GameObject obj in topAnt) {
         Destroy(obj);
      }
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/TopUser.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      var n = 10;
      if (datos.Length < 10) { n = datos.Length; }

      for (int i = 0; i < n; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
         TopRow row;
         row = Instantiate(PanelTopRow, Container);
         if (i > 3) {
            row.Medal = Medals[3];
         } else {
            row.Medal = Medals[i];
         }
         row.Top = (i + 1).ToString();
         row.Nick = col[0];
         row.Score = col[1];
         row.Background = new Color(1f, 1f, 1, 1);
      }

      for (int i = 0; i < datos.Length; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
         if (nick == col[0]) {
            TopRow row;
            row = Instantiate(PanelTopRow, Container);
            if (i > 3) {
               row.Medal = Medals[3];
            } else {
               row.Medal = Medals[i];
            }
            row.Top = (i + 1).ToString();
            row.Nick = col[0];
            row.Score = col[1];
            row.Background = new Color(0.9f, 0.7f, 0, 1);
         }
      }
      Message("");
   }
}
