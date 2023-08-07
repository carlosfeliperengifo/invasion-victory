using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class User : MonoBehaviour {
   // Transform variables of the different screens
   [SerializeField] private Transform RankingPanel;
   [SerializeField] private Transform ConfigPanel;
   [SerializeField] private Transform CreditsPanel;
   [SerializeField] private Transform ManualPanel;
   [SerializeField] private GameObject MoveCanvas;
   [SerializeField] private Armament armamentScript;
   private bool inMovement = false;
   // Variables for displaying users in the Ranking
   [SerializeField] private Sprite[] Medals;
   [SerializeField] private Transform Container;
   [SerializeField] private TopRow PanelTopRow;
   // Variables for displaying the image of the user's current level.
   [SerializeField] private Sprite[] Level;
   [SerializeField] private Image[] ContainersLevel;
   // Survey button
   [SerializeField] private Transform btSurvey;
   // Variables for displaying the manual's images
   [SerializeField] private Sprite[] ManualImages;
   [SerializeField] private Image ContainerManual;
   [SerializeField] private Transform[] ManualBts;
   private int indexManual;

   // Structure for displaying a user in the Ranking.
   private struct TopUser {
      public string nick;
      public int score;
      public TopUser (string nick, int score) {
         this.nick = nick;
         this.score = score;
      }
   }
   private int idUser;
   private string nick;
   private int idLevel = 0;
   private string points = "";
   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   private void Update () {
      var frames = MoveCanvas.GetComponentInChildren<VideoPlayer>().frame;
      if (inMovement && frames > 0 && !MoveCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inMovement = false;
         GlobalManager.events.bt_skip2();
      }
   }
   // Displays the Ranking screen
   public void Ranking () {
      Message("Cargando . . .");
      LoadUserData();
      HideAllPanels();
      ShowPanel(RankingPanel);
      StartCoroutine(GetLevel());
   }
   // Displays the Parameters screen
   public void Parameters () {
      armamentScript.LoadConfiguration(nick, Level[idLevel - 1], points);
      HideAllPanels();
      ShowPanel(ConfigPanel);
   }
   // Save user parameters to a txt file
   public void SaveParameters () {
      armamentScript.SaveParameters();
      StartCoroutine(UpdateWeapon());
   }
   // Displays the Credits screen
   public void Credits () {
      HideAllPanels();
      TextAsset credits = Resources.Load<TextAsset>("CreditsEs");
      CreditsPanel.GetChild(1).GetComponentInChildren<Text>().text = credits.text;
      ShowPanel(CreditsPanel);
   }
   // Displays the Manual screen
   public void Manual () {
      HideAllPanels();
      ShowPanel(ManualPanel);
      indexManual = 0;
      ChangeManualImage(indexManual);
   }
   // Displays the Movement video
   public void Movement () {
      Message("Cargando . . .");
      MoveCanvas.SetActive(true);
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Play();
      HideAllPanels();
      inMovement = true;
   }
   // Hide all panels
   private void HideAllPanels () {
      RankingPanel.localScale = Vector3.zero;
      ConfigPanel.localScale = Vector3.zero;
      CreditsPanel.localScale = Vector3.zero;
      ManualPanel.localScale = Vector3.zero;
   }
   // Dynamic display of the panel
   private void ShowPanel (Transform panel) {
      float duration = 0.3f;
      panel.DOScale(Vector3.one, duration);
   }
   // Retrieve the user data and display their nickname
   private void LoadUserData () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "usid":
            idUser = int.Parse(col[1]);
            break;
         case "nick":
            nick = col[1];
            GameObject.Find("txtNick").GetComponent<Text>().text = col[1];
            break;
         }
      }
   }
   // Displays the image of the user's level.
   private void ShowLevelImage () {
      if (idLevel > 1) {
         ContainersLevel[0].sprite = Level[idLevel - 2];
      } else {
         ContainersLevel[0].color = new Color(1, 1, 1, 0);
      }
      ContainersLevel[1].sprite = Level[idLevel - 1];
      if (idLevel < Level.Length) {
         ContainersLevel[2].sprite = Level[idLevel];
      } else {
         ContainersLevel[2].color = new Color(1, 1, 1, 0);
      }
   }
   // Display a message mss on the screen.
   private void Message (string mss) {
      GameObject.Find("txMss").GetComponent<Text>().text = mss;
   }
   // Retrieve user parameters from the database
   private IEnumerator GetLevel () {
      WWWForm form = new WWWForm();
      form.AddField("usid", idUser);
      form.AddField("option", "return");
      string url = dataPath + "/ranking.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión, vuelve a intentar");
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
            wr.Dispose();
            if (datos.Length == 5) {
               if (datos[0] == "1") {
                  idLevel = int.Parse(datos[1]);
                  points = datos[2];
                  armamentScript.IdLevel = idLevel;
                  PlayerPrefs.SetInt("idLevel", idLevel);
                  PlayerPrefs.SetInt("idWeapon", int.Parse(datos[3]));
                  PlayerPrefs.SetInt("idMat", int.Parse(datos[4]));
                  ShowLevelImage();
                  StartCoroutine(GetTopListDB());
               }
            } else {
               Message("Error de conexión, vuelve a intentar");
            }
         }
         wr.Dispose();
      }
   }
   // Retrieve the top players based on the user's level
   private IEnumerator GetTopListDB () {
      WWWForm form = new WWWForm();
      form.AddField("raid", idLevel);
      string url = dataPath + "/topUser.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión, vuelve a intentar");
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
            if (datos.Length > 0) {
               TextWriter topFile = new StreamWriter(Application.persistentDataPath + "/TopUser.txt", false);
               foreach (string dato in datos) {
                  string[] dat = dato.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
                  topFile.WriteLine(dat[0] + "\t" + dat[1]);
               }
               topFile.Close();
               LoadTop10();
            } else {
               Message("Juega y posicionate primero en el Ranking");
               ShowBtSurvey();
            }
         }
         wr.Dispose();
      }
   }
   // Display the top 10 players on the Ranking screen
   private void LoadTop10 () {
      Message("Cargando . . .");
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
      ShowBtSurvey();
   }
   // Update player's weapon in the database
   public IEnumerator UpdateWeapon () {
      string ids = PlayerPrefs.GetInt("idWeapon", 0).ToString();
      ids += "&&";
      ids += PlayerPrefs.GetInt("idMat", 0).ToString();

      WWWForm form = new WWWForm();
      form.AddField("usid", idUser);
      form.AddField("weapon", ids);
      string url = dataPath + "/updateWeapon.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión, vuelve a intentar");
         } else {
            if (wr.downloadHandler.text == "1") {
               GlobalManager.events.bt_close2();
            } else {
               Message("Error de conexión, vuelve a intentar");
            }
         }
         wr.Dispose();
      }
   }
   // Display the survey button.
   private void ShowBtSurvey () {
      if (points == "0" && PlayerPrefs.GetInt("isLogin", 0) == 1) {
         PlayerPrefs.SetInt("isLogin", 0);
         GlobalManager.events.bt_manual();
         RankingPanel.localScale = Vector3.zero;
      } else {
         string str = PlayerPrefs.GetString("survey", "");
         if (str == "") {
            btSurvey.localScale = Vector3.zero;
         } else {
            btSurvey.localScale = Vector3.one;
            if (PlayerPrefs.GetInt("showSurvey", 0) == 1) {
               PlayerPrefs.SetInt("showSurvey", 0);
               OpenCloseSurveyPlane(true);
            } else {
               OpenCloseSurveyPlane(false);
            }
         }
      }
   }
   // Toggle the visibility of the survey window
   public void OpenCloseSurveyPlane (bool op) {
      if (op) {
         btSurvey.GetChild(0).localScale = Vector3.one;
      } else {
         btSurvey.GetChild(0).localScale = Vector3.zero;
      }
   }
   // Redirects the user to the form
   public void GoToSurvey () {
      string str = PlayerPrefs.GetString("survey", "");
      Application.OpenURL(str);
   }
   // Change the manual's image.
   public void ChangeManualImage (int id) {
      indexManual += id;
      if (indexManual >= ManualImages.Length - 1) {
         indexManual = ManualImages.Length - 1;
         ManualBts[1].localScale = Vector3.zero;
      } else if (indexManual <= 0) {
         indexManual = 0;
         ManualBts[0].localScale = Vector3.zero;
         ManualBts[1].localScale = Vector3.one;
      } else {
         ManualBts[0].localScale = Vector3.one;
         ManualBts[1].localScale = Vector3.one;
      }
      ContainerManual.sprite = ManualImages[indexManual];
   }
}
