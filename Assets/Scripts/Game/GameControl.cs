using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameControl : MonoBehaviour {
   [SerializeField] private Player player;
   [SerializeField] private GameObject[] spaceships;
   [SerializeField] private Radar radar;
   [SerializeField] private Text txHordes;
   [SerializeField] private Text txTime;
   [SerializeField] private Text txPhrase;

   /******Match*****/
   private int spaceshipsHorde;
   private int timeHordes;
   private float speedSpaceships;

   /*****Performance*****/
   private bool completedGame;
   private float score;
   private int spaceshipsDest;
   private int lifeBar;
   private float timePlayed;

   private bool enableGame = false;

   private const int hordes = 5;
   private int hordesGenerated = 0;
   private bool completedHordes = false;

   private const float minSpawnDist = 10;
   private const float maxSpawnDist = 12;

   private const float maxTimeGame = 180;
   private const float updateTime = 1;
   private float delayTime = 0;

   public int SpaceshipsHorde { set { spaceshipsHorde = value; } }
   public int TimeHorde { set { timeHordes = value; } }
   public float SpeedSpaceships { set { speedSpaceships = value; } }

   public float MaxSpawnDist { get { return maxSpawnDist; } }

   public void LoadGameParameters () {
      ShowPhrase();
      player.LoadWeapon();
      foreach (GameObject ss in spaceships) {
         ss.GetComponent<SpaceShip>().Speed = speedSpaceships;
      }
      InvokeRepeating(nameof(GenerateHorde), 3.5f, timeHordes);
   }
   void Update () {
      if (enableGame) {
         timePlayed += Time.deltaTime;
         if (Time.time >= delayTime) {
            delayTime = Time.time + updateTime;
            var mn = Mathf.FloorToInt((maxTimeGame - timePlayed) / 60);
            var sg = Mathf.FloorToInt((maxTimeGame - timePlayed) % 60);
            txTime.text = string.Format("{0:00}:{1:00}", mn, sg);
         }

         if (player.LifePoints == 0) {
            CancelInvoke(nameof(GenerateHorde));
            enableGame = false;
            completedGame = false;
            GlobalManager.events.finishgame();
         } else if (completedHordes && GameObject.FindGameObjectsWithTag("SpaceShip").Length == 0) {
            enableGame = false;
            completedGame = true;
            GlobalManager.events.finishgame();
         } else if (timePlayed >= maxTimeGame) {
            enableGame = false;
            completedGame = false;
            timePlayed = maxTimeGame;
            GlobalManager.events.finishgame();
         }
      }
   }
   private void ShowPhrase () {
      TextAsset phrase = Resources.Load<TextAsset>("MotivationalQuotes");
      string[] datos = phrase.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      int index = Random.Range(0, datos.Length);
      txPhrase.text = datos[index];
   }
   public void StartGame () {
      GetComponent<AudioSource>().Play();
      enableGame = true;
   }

   private void GenerateHorde () {
      for (int i = 0; i < spaceshipsHorde; i++) {
         int pE = Random.Range(0, spaceships.Length);
         Instantiate(spaceships[pE], GeneratePosition(), new Quaternion(0, Random.Range(-Mathf.PI, Mathf.PI), 0, 1));
      }
      hordesGenerated++;
      if (hordesGenerated == hordes) {
         completedHordes = true;
         CancelInvoke(nameof(GenerateHorde));
      }
      txHordes.text = hordesGenerated.ToString() + "/" + hordes.ToString();
   }
   private Vector3 GeneratePosition () {
      float angle = Random.Range(0, 72) * 5;
      float radius = Random.Range(minSpawnDist, maxSpawnDist);
      float genPosX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
      float genPosY = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
      float genPosZ = Random.Range(0.0f, 2f);
      Transform player = GameObject.FindGameObjectWithTag("Player").transform;
      Vector3 randomPos = new Vector3(genPosX, genPosZ, genPosY) + player.position;

      return randomPos;
   }

   private void GetGamePerformance () {
      string dest = GameObject.Find("Destroyed").GetComponentInChildren<Text>().text;
      spaceshipsDest = int.Parse(dest);
      var H = hordes;
      lifeBar = player.LifePoints;

      var SS = (spaceshipsHorde * H / 50f) * (spaceshipsDest / 50f) * 10000;

      var r = minSpawnDist;
      var T = maxTimeGame;
      var M = timeHordes * (H - 1f) + r / speedSpaceships;
      float tp;
      if (timePlayed <= M) {
         tp = Mathf.Sin(2f * Mathf.PI * timePlayed / (4f * M));
      } else {
         tp = Mathf.Sin(2f * Mathf.PI * (timePlayed - M) / (4f * (T - M)) + Mathf.PI / 2f);
      }
      //var tm = (T - M) / (T - (6f * (H - 1f) + (r / 1.2f)));
      var TS = tp * 10000;
      var cg = 0;
      if (completedGame) { cg = 1; }

      score = SS + TS + (cg * 1000) + lifeBar * 10;
   }
   public void SavePerformanceTxt () {
      GetGamePerformance();
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Performance.txt", false);
      string val = completedGame.ToString();
      datos.WriteLine("completedGame" + "\t" + val);

      val = Mathf.Round(score).ToString("F0");
      datos.WriteLine("score" + "\t" + val);

      val = spaceshipsDest.ToString();
      datos.WriteLine("destroyedSpaceships" + "\t" + val);

      val = lifeBar.ToString();
      datos.WriteLine("lifeBar" + "\t" + val);

      val = timePlayed.ToString("F2");
      datos.WriteLine("timePlayed" + "\t" + val);
      datos.Close();
   }

}
