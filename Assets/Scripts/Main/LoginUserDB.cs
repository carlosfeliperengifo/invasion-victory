using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginUserDB : MonoBehaviour {
   [SerializeField] private InputField inNick;
   [SerializeField] private InputField inPass;

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inPass.text = "";
   }
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
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2": // Contraseña incorrecta
               Message("Contraseña incorrecta");
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            case "3": // Nick no encontrado
               Message("Apodo no encontrado");
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            default: // Evaluar respuesta
               string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
               wr.Dispose();
               if (datos.Length == 5) {
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
   private void Message (string mss) {
      GameObject.Find("txtMss1").GetComponent<Text>().text = mss;
   }
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }

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
