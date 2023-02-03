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
   [SerializeField] private GameObject RankingCanvas;
   [SerializeField] private GameObject ParamCanvas;
   [SerializeField] private GameObject CreditsCanvas;
   [SerializeField] private GameObject ManualCanvas;
   [SerializeField] private GameObject MoveCanvas;

   private bool inMovement = false;

   [SerializeField] private Sprite[] Medals;
   [SerializeField] private GameObject Container;
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
      SliderValues.instance.GetMatchTxt();
      GameObject.Find("txMss").GetComponent<Text>().text = "";

      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "nick":
            nick = col[1];
            GameObject.Find("txtNick").GetComponent<Text>().text = col[1];
            break;
         }
      }
      StartCoroutine(Top10DB());

      RankingCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      RankingCanvas.transform.GetChild(1).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ManualCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   public void Parameters () {
      RankingCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RankingCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
   }
   public void SaveMatchTxt () {
      SliderValues.instance.SaveMatchTxt();
   }
   public void Credits () {
      RankingCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RankingCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

      TextAsset credits = Resources.Load<TextAsset>("Credits");
      CreditsCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = credits.text;
   }
   public void Manual () {
      RankingCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RankingCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ManualCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
   }
   public void Movement () {
      GameObject.Find("txMss").GetComponent<Text>().text = "Loading . . .";
      MoveCanvas.SetActive(true);
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Play();
      RankingCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RankingCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      inMovement = true;
   }

   IEnumerator Top10DB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", nick);
      //string url = "http://universalattack.000webhostapp.com/codes/topUser.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/topUser.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GameObject.Find("txMss").GetComponent<Text>().text = "Connection error, try again";
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
               GameObject.Find("txMss").GetComponent<Text>().text = "Connection error, try again";
            }
         }
         wr.Dispose();
      }
   }
   private void LoadTop10 () {
      GameObject.Find("txMss").GetComponent<Text>().text = "Loading . . .";
      GameObject[] topAnt = GameObject.FindGameObjectsWithTag("Top10");
      foreach (GameObject obj in topAnt) {
         Destroy(obj);
      }
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/TopUser.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var n = 10;
      if (datos.Length < 10) { n = datos.Length; }

      for (int i = 0; i < n; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
         TopRow row;
         row = Instantiate(PanelTopRow, Container.transform);
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
            row = Instantiate(PanelTopRow, Container.transform);
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
      GameObject.Find("txMss").GetComponent<Text>().text = "";
   }
}
