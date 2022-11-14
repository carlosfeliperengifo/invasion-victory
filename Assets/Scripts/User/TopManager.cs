using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TopManager : MonoBehaviour {
   [SerializeField] private GameObject Container;
   [SerializeField] private TopRow PanelTopRow;
   [SerializeField] private Sprite[] Medals;
   private struct TopUser {
      public string nick;
      public int score;
      public TopUser (string nick, int score) {
         this.nick = nick;
         this.score = score;
      }
   }
   private string nick;
   void Start () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "nick":
            nick = col[1];
            break;
         }
      }
      StartCoroutine(Top10DB());
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
               LoadTop();
            }
         }
      }
   }
   private void LoadTop () {
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
   }
}
