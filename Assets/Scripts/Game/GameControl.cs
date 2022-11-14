using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {
   [SerializeField] private GameObject Generator;
   [SerializeField] private GameObject Player;
   [SerializeField] private GameObject Radar;
   [SerializeField] private GameObject[] Enemies;

   private int spaceshipsHorde;
   private int timeHordes;
   private float speedSpaceships;

   private float initialTimeGame = 0;
   private bool enableGame = false;
   private bool isgameOver = false;

   private bool completedgame;
   private float score;
   private int spaceshipsDest;
   private float destroyed7Total;
   private int lifeBar;
   private float timePlayed;

   private void Awake () {
      Generator.gameObject.SetActive(false);
      Radar.GetComponent<Radar>().enabled = false;
      Player.SetActive(false);
   }
   void Start () {
      GetMatchTxt();
      SetGameMatch();
   }
   void Update () {
      initialTimeGame += Time.deltaTime;
      if (initialTimeGame >= 4 && !enableGame) {
         Player.SetActive(true);
         Generator.gameObject.SetActive(true);
         Generator.GetComponent<SpaceshipsGenerator>().InvokeHordes();
         Radar.GetComponent<Radar>().enabled = true;
         enableGame = true;
      }
      if (enableGame) {
         if ((Player.GetComponent<Player>().LifePoints == 0 || Generator.GetComponent<SpaceshipsGenerator>().CompletedTime) && !isgameOver) {
            Generator.GetComponent<SpaceshipsGenerator>().CancelInvoke();
            Generator.GetComponent<SpaceshipsGenerator>().CountTime = false;
            Generator.GetComponent<SpaceshipsGenerator>().CompletedGame = false;
            isgameOver = true;
            SavePerformanceTxt();
         }
         if (Generator.GetComponent<SpaceshipsGenerator>().CompletedGame) {
            SavePerformanceTxt();
         }
      }

   }
   private void GetMatchTxt () {
      //TextAsset Datostxt = Resources.Load<TextAsset>("DataDB");
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Match.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });

      for (int i = 0; i < datos.Length; i++) {
         string[] col = datos[i].Split(new char[] { '\t' });
         switch (col[0]) {
         case "spaceshipsHorde":
            spaceshipsHorde = int.Parse(col[1]);
            break;
         case "timeHorde":
            timeHordes = int.Parse(col[1]);
            break;
         case "speedSpaceships":
            speedSpaceships = float.Parse(col[1]);
            break;
         }
      }
   }
   private void SetGameMatch () {
      Generator.GetComponent<SpaceshipsGenerator>().SpaceshipsHorde = spaceshipsHorde;
      Generator.GetComponent<SpaceshipsGenerator>().TimeHordes = timeHordes;

      foreach (GameObject enemy in Enemies) {
         enemy.GetComponent<SpaceShip>().Speed = speedSpaceships;
      }
   }
   private void GetGamePerformance () {
      string dest = GameObject.Find("Destroyed").GetComponentInChildren<Text>().text;
      spaceshipsDest = int.Parse(dest);
      var H = Generator.GetComponent<SpaceshipsGenerator>().NumberHordes;
      destroyed7Total = ((float)(spaceshipsDest) / (float)(spaceshipsHorde * H)) * 100;
      lifeBar = Player.GetComponent<Player>().LifePoints;
      timePlayed = Generator.GetComponent<SpaceshipsGenerator>().TimeGame;
      completedgame = Generator.GetComponent<SpaceshipsGenerator>().CompletedGame;

      var SS = (spaceshipsHorde * H / 50f) * (spaceshipsDest / 50f) * 10000;

      var r = Generator.GetComponent<SpaceshipsGenerator>().MinSpawnDist;
      var T = Generator.GetComponent<SpaceshipsGenerator>().MaxTimeGame;
      var M = timeHordes * (H - 1f) + r / speedSpaceships;
      var tp = 0f;
      if (timePlayed <= M) {
         tp = Mathf.Sin(2f * Mathf.PI * timePlayed / (4f * M));
      } else {
         tp = Mathf.Sin(2f * Mathf.PI * (timePlayed - M) / (4f * (T - M)) + Mathf.PI / 2f);
      }
      var tm = (T - M) / (T - (6f * (H - 1f) + (r / 1.2f)));
      var TS = tp * tm * 10000;
      var cg = 0;
      if (completedgame) { cg = 1; }

      score = SS + TS + (cg * 1000) + lifeBar * 10;
   }
   private void SavePerformanceTxt () {
      GetGamePerformance();
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Performance.txt", false);
      string val = completedgame.ToString();
      datos.WriteLine("completedGame" + "\t" + val);

      val = Mathf.Round(score).ToString("F0");
      datos.WriteLine("score" + "\t" + val);

      val = spaceshipsDest.ToString();
      datos.WriteLine("destroyedSpaceships" + "\t" + val);

      val = destroyed7Total.ToString("F2");
      datos.WriteLine("destroyed7Total" + "\t" + val);

      val = lifeBar.ToString();
      datos.WriteLine("lifeBar" + "\t" + val);

      val = timePlayed.ToString("F2");
      datos.WriteLine("timePlayed" + "\t" + val);
      datos.Close();

      SceneManager.LoadScene("03_EndGame");
   }
}
