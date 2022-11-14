using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipsGenerator : MonoBehaviour {
   [SerializeField] private GameObject[] spaceships;
   [SerializeField] private GameObject portal;

   private int spaceshipsHorde;
   private float timeHordes;

   private int numberHordes = 5;
   private int hordesGenerated = 0;
   private bool completedHordes = false;
   [SerializeField] private Text txHordes;

   private float minSpawnDist = 10;
   private float maxSpawnDist = 12;

   private bool completedGame = false;
   private float timeGame = 0;
   private float maxTimeGame = 180;
   private float updateTime = 1;
   private float delayTime = 0;
   private bool completedTime = false;

   [SerializeField] private Text txtTime;
   private bool countTime = false;

   public int SpaceshipsHorde { set { spaceshipsHorde = value; } }
   public float TimeHordes { set { timeHordes = value; } }
   public int NumberHordes { get { return numberHordes; } }
   public float MaxSpawnDist { get { return maxSpawnDist; } }
   public float MinSpawnDist { get { return minSpawnDist; } }
   public bool CompletedGame { get { return completedGame; } set { completedGame = value; } }
   public float TimeGame { get { return timeGame; } }
   public float MaxTimeGame { get { return maxTimeGame; } }
   public bool CountTime { set { countTime = value; } }
   public bool CompletedTime { get { return completedTime; } }

   void Start () {
   }
   void Update () {
      if (countTime) {
         timeGame += Time.deltaTime;
         if (Time.time >= delayTime) {
            delayTime = Time.time + updateTime;
            var mn = (int)((maxTimeGame - timeGame) / 60);
            var sg = (int)((maxTimeGame - timeGame) - (mn * 60));

            string ps = mn.ToString();
            if (mn < 10) {
               ps = "0" + mn.ToString();
            }
            if (sg < 10) {
               ps += ":0" + sg.ToString();
            } else {
               ps += ":" + sg.ToString();
            }
            txtTime.text = ps;
         }
         if (timeGame >= maxTimeGame) {
            countTime = false;
            completedGame = false;
            timeGame = maxTimeGame;
            completedTime = true;
         }
      }
      if (completedHordes && GameObject.FindGameObjectsWithTag("EnemyPlane").Length == 0) {
         countTime = false;
         completedGame = true;
      }
   }
   public void InvokeHordes () {
      countTime = true;
      InvokeRepeating("GenerateHordes", 0, timeHordes);
   }

   private void GenerateHordes () {
      for (int i = 0; i < spaceshipsHorde; i++) {
         GenerateSpaceship();
      }
      hordesGenerated += 1;
      if (hordesGenerated == numberHordes) {
         completedHordes = true;
         CancelInvoke("GenerateHordes");
      }
      txHordes.text = hordesGenerated.ToString() + "/" + numberHordes.ToString();
   }
   private void GenerateSpaceship () {
      int pE = Random.Range(0, spaceships.Length);
      GameObject newEnemy = Instantiate(spaceships[pE], GeneratePosition(), spaceships[pE].transform.rotation);

      Vector3 offsetPos = new Vector3(0, 0.9f, 0);
      GameObject newPortal = Instantiate(portal, newEnemy.transform.position - offsetPos, transform.rotation);
      Destroy(newPortal, 1.5f);
   }
   private Vector3 GeneratePosition () {
      float angle = Random.Range(0, 72) * 5;
      float radius = Random.Range(minSpawnDist, maxSpawnDist);
      float genPosX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
      float genPosY = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
      float genPosZ = Random.Range(0.0f, 1.0f);
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      Vector3 randomPos = new Vector3(genPosX, genPosZ, genPosY) + player.transform.position;

      return randomPos;
   }
}
