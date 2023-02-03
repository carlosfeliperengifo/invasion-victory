using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {
   [SerializeField] private Player player;
   [SerializeField] private GameObject[] spaceships;
   [SerializeField] private Radar radar;
   [SerializeField] private Text txHordes;
   [SerializeField] private Text txTime;

   /******Match*****/
   private int spaceshipsHorde;
   private int timeHordes;
   private float speedSpaceships;

   /*****Performance*****/
   private bool completedGame;
   private float score;
   private int spaceshipsDest;
   private float destroyed7Total;
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

   public float MaxSpawnDist { get { return maxSpawnDist; } }

   void Start () {
      GetMatchTxt();
      InvokeRepeating("GenerateHorde", 3.5f, timeHordes);
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
            CancelInvoke("GenerateHorde");
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
   private void GetMatchTxt () {
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
      foreach (GameObject ss in spaceships) {
         ss.GetComponent<SpaceShip>().Speed = speedSpaceships;
      }
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
         CancelInvoke("GenerateHorde");
      }
      txHordes.text = hordesGenerated.ToString() + "/" + hordes.ToString();
   }
   private Vector3 GeneratePosition () {
      float angle = Random.Range(0, 72) * 5;
      float radius = Random.Range(minSpawnDist, maxSpawnDist);
      float genPosX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
      float genPosY = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
      float genPosZ = Random.Range(0.0f, 1.0f);
      Transform player = GameObject.FindGameObjectWithTag("Player").transform;
      Vector3 randomPos = new Vector3(genPosX, genPosZ, genPosY) + player.position;

      return randomPos;
   }

   private void GetGamePerformance () {
      string dest = GameObject.Find("Destroyed").GetComponentInChildren<Text>().text;
      spaceshipsDest = int.Parse(dest);
      var H = hordes;
      destroyed7Total = ((float)(spaceshipsDest) / (float)(spaceshipsHorde * H)) * 100;
      lifeBar = player.LifePoints;

      var SS = (spaceshipsHorde * H / 50f) * (spaceshipsDest / 50f) * 10000;

      var r = minSpawnDist;
      var T = maxTimeGame;
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

      val = destroyed7Total.ToString("F2");
      datos.WriteLine("destroyed7Total" + "\t" + val);

      val = lifeBar.ToString();
      datos.WriteLine("lifeBar" + "\t" + val);

      val = timePlayed.ToString("F2");
      datos.WriteLine("timePlayed" + "\t" + val);
      datos.Close();

      GlobalManager.events.performanceSaved();
   }

}
