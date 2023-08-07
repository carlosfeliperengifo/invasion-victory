using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginUserDB : MonoBehaviour {
   // Text input variables for user data
   [SerializeField] private InputField inNick;
   [SerializeField] private InputField inPass;

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   // Clear all text inputs
   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inPass.text = "";
   }
   // Check the length of the text fields
   public void LoginUser () {
      if (inNick.text.Length > 0) {
         if (inPass.text.Length > 0) {
            StartCoroutine(CheckSurvey());
            return;
         } else {
            Message("ingresa tu contraseña");
         }
      } else {
         Message("Ingresa tu apodo");
      }
      GlobalManager.events.failed1();
   }
   // Submit the form with the new user's data to the database
   IEnumerator LoginDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      string url = dataPath + "/login.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         Message("Cargando . . .");
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión, vuelve a intentar");
            GlobalManager.events.failed1();
         } else { // Server ok
            switch (wr.downloadHandler.text) {
            case "2": // Incorrect password
               Message("Contraseña incorrecta");
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            case "3": // Nickname not found
               Message("Apodo no encontrado");
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            default: // Evaluate response
               string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
               wr.Dispose();
               if (datos.Length == 5) {
                  // Save user data to a txt file
                  if (datos[0] == "1") {
                     TextWriter user = new StreamWriter(Application.persistentDataPath + "/User.txt", false);
                     user.WriteLine("usid" + "\t" + datos[1]);
                     user.WriteLine("nick" + "\t" + inNick.text);
                     user.WriteLine("pass" + "\t" + inPass.text);
                     user.WriteLine("alid" + "\t" + datos[2]);
                     user.WriteLine("date" + "\t" + datos[3]);
                     user.WriteLine("time" + "\t" + datos[4]);
                     user.Close();
                     CleanAllDatas();
                     PlayerPrefs.SetInt("isLogin", 1);
                     GlobalManager.events.success2();
                  } else {
                     Message("Error de conexión, vuelve a intentar");
                     GlobalManager.events.failed1();
                  }
               } else {
                  Message("Error de conexión, vuelve a intentar");
                  GlobalManager.events.failed1();
               }
               break;
            }
         }
      }
   }
   // Display a message mss on the screen
   private void Message (string mss) {
      GameObject.Find("txtMss1").GetComponent<Text>().text = mss;
   }
   // Toggle the visibility of the password
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
   // Retrieve the survey link from the server
   private IEnumerator CheckSurvey () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      string url = dataPath + "/usability.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            PlayerPrefs.SetInt("showSurvey", 0);
            PlayerPrefs.SetString("survey", "");
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
            if (datos.Length == 2) {
               if (datos[0] == "1") {
                  PlayerPrefs.SetInt("showSurvey", 1);
                  PlayerPrefs.SetString("survey", datos[1]);
               } else {
                  PlayerPrefs.SetInt("showSurvey", 0);
                  PlayerPrefs.SetString("survey", "");
               }
            } else {
               PlayerPrefs.SetInt("showSurvey", 0);
               PlayerPrefs.SetString("survey", "");
            }
         }
         wr.Dispose();
         StartCoroutine(LoginDB());
      }
   }
}
