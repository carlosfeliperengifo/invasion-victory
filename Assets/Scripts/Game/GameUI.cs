using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;

public class GameUI : MonoBehaviour {
   // Transform variables of the different screens
   [SerializeField] private Transform GameCanvas;
   [SerializeField] private GameObject PauseCanvas;
   [SerializeField] private Transform timer;
   [SerializeField] private GameObject infConfig;
   [SerializeField] private GameControl gameControl;

   private int currentShips = 3;
   private int temp = 4;
   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";
   // Countdown to start the game
   public void Countdown () {
      temp--;
      if (temp > 0) {
         timer.localScale = new Vector3(0, 0, 0);
         timer.GetComponent<Text>().text = temp.ToString();
         timer.DOScale(new Vector3(1, 1, 1), 0.7f);
      } else {
         timer.GetComponent<Text>().text = "";
         CancelInvoke(nameof(Countdown));
         GlobalManager.events.time3s();
      }
   }
   // Load game parameters and display the game screen
   public void Game () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
      }
      GameCanvas.GetChild(1).localScale = new Vector3(0, 0, 0);
      PauseCanvas.SetActive(false);
      if (Application.internetReachability == NetworkReachability.NotReachable) {
         GlobalManager.events.bt_home();
      } else {
         StartCoroutine(GetSpaceships());
      }
   }
   // Display player controls
   public void Playing () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
         AudioSource[] audios = FindObjectsOfType<AudioSource>();
         foreach (AudioSource a in audios) {
            if (a.time > 0) {
               a.Play();
            }
         }
      }
      infConfig.SetActive(false);
      GameCanvas.GetChild(1).localScale = new Vector3(1, 1, 1);
      PauseCanvas.SetActive(false);
      gameControl.StartGame();
   }
   // Enable pause screen with its menu
   public void Pause () {
      infConfig.SetActive(true);
      GameCanvas.GetChild(1).localScale = new Vector3(0, 0, 0);
      PauseCanvas.SetActive(true);
      Time.timeScale = 0;
      AudioSource[] audios = FindObjectsOfType<AudioSource>();
      foreach (AudioSource a in audios) {
         a.Pause();
      }
   }
   // Save Performance parameters and change of scene
   public void SavePerformanceTxt () {
      gameControl.SavePerformanceTxt();
      GlobalManager.events.performanceSaved();
   }
   // Load user id and assigned algorithm
   private int[] LoadUserTxt() {
      int[] data = new int[2];
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col[0] == "usid") {
            data[0] = int.Parse(col[1]);
         } else if (col[0] == "alid") {
            data[1] = int.Parse(col[1]);
         }
      }
      return data;
   }
   // Return the last used spaceshipsHorde and the increase or decrease of spaceships
   private IEnumerator GetSpaceships () {
      int[] dataUser = LoadUserTxt();
      WWWForm form = new WWWForm();
      form.AddField("usid", dataUser[0]);
      string url = dataPath + "/getSpaceships.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GlobalManager.events.bt_home();
         } else { // server ok
            Debug.Log(wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
            wr.Dispose();
            if (datos.Length == 3) {
               if (datos[0] == "1") {
                  currentShips = int.Parse(datos[1]);
                  LoadADD(dataUser[1], int.Parse(datos[2]));
               }
            } else {
               LoadADD(dataUser[1], 0);
            }
         }
         wr.Dispose();
      }
   }
   // Load the values from the table of the given algorithm
   private void LoadADD (int add, int n) {
      int index = 1;
      int newShips = 3, time = 20;
      float speed = 0.6f;
      Debug.Log("ADD: " + add.ToString());
      // load algorithm add
      TextAsset tableADD;
      switch (add) {
      case 1: // Random Forest
         tableADD = Resources.Load<TextAsset>("Tables_Algorithms/AlgorithmRF");
         break;
      case 2: // Support Vector Machine
         tableADD = Resources.Load<TextAsset>("Tables_Algorithms/AlgorithmSVM");
         break;
      case 3: // Neuronal Network
         tableADD = Resources.Load<TextAsset>("Tables_Algorithms/AlgorithmARN");
         break;
      default:
         GlobalManager.events.bt_home();
         return;
      }
      // Finding the current value of spaceships
      string[] datos = tableADD.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      string[] dat;
      for (int i=1; i<datos.Length; i++) {
         dat = datos[i].Split(new char[] { '\t' });
         if (currentShips == int.Parse(dat[3])) {
            index = i;
            break;
         }
      }
      // Update index to next value in table
      if (index + n <= 0) {
         index = 1;
      } else if (index + n >= datos.Length) {
         index = datos.Length - 1;
      } else {
         index += n;
      }
      // Load data from algorithm table
      dat = datos[index].Split(new char[] { '\t' });
      time = int.Parse(dat[1]);
      speed = float.Parse(dat[2]) / 10f;
      newShips = int.Parse(dat[3]);
      // Loading and saving Match data of the algorithm
      gameControl.TimeHorde = time;
      gameControl.SpeedSpaceships = speed;
      gameControl.SpaceshipsHorde = newShips;
      gameControl.LoadGameParameters();
      SaveMatchTxt(newShips, time, speed);
      InvokeRepeating(nameof(Countdown), 0f, 1f);
   }
   // Save Match parameters in a txt file
   private void SaveMatchTxt (int ships, int tm, float speed) {
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Match.txt", false);
      datos.WriteLine("spaceshipsHorde" + "\t" + ships.ToString("F0"));
      datos.WriteLine("timeHorde" + "\t" + tm.ToString("F0"));
      datos.WriteLine("speedSpaceships" + "\t" + speed.ToString("F1"));
      datos.Close();
   }
}
