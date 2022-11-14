using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class UIgameControl : MonoBehaviour {
   [SerializeField] private GameObject GameOverCanvas;
   [SerializeField] private GameObject GameComCanvas;

   private bool completedGame;
   private int Score;
   private float timeGame;

   public GameObject gameOverSound;
   public GameObject congratSound;

   void Start () {
      CanvasManager.instance.OnGameOverMenu += ActivateGameOver;
      CanvasManager.instance.OnCongratMenu += ActivateGameCongrat;
      GetPerformanceTxt();
      if (completedGame) {
         CanvasManager.instance.CongratMenu();
      } else {
         CanvasManager.instance.GameOverMenu();
      }
   }
   private void ActivateGameOver () {
      GameOverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

      GameComCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color(0.627f, 0.117f, 0.039f, 1);
      GameOverCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = Score.ToString();
      GameOverCanvas.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = GetTimeForm();
      Instantiate(gameOverSound);
   }
   private void ActivateGameCongrat () {
      GameOverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      GameComCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

      transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color(0.588f, 0.117f, 0.588f, 1);
      GameComCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = Score.ToString();
      GameComCanvas.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = GetTimeForm();
      Instantiate(congratSound);
   }
   private string GetTimeForm () {
      var mn = (int)(timeGame / 60);
      var sg = (int)(timeGame - mn * 60);

      string ps = mn.ToString();
      if (mn < 10) {
         ps = "0" + mn.ToString();
      }
      if (sg < 10) {
         ps += ":0" + sg.ToString();
      } else {
         ps += ":" + sg.ToString();
      }
      return ps;
   }
   private void GetPerformanceTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Performance.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      for (int i = 0; i < datos.Length; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
         switch (col[0]) {
         case "completedGame":
            completedGame = bool.Parse(col[1]);
            break;
         case "score":
            Score = int.Parse(col[1]);
            break;
         case "timePlayed":
            timeGame = float.Parse(col[1]);
            break;
         }
      }
   }
}
