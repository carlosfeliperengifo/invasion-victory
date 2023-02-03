using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginUserDB : MonoBehaviour {
   [SerializeField] private InputField inNick;
   [SerializeField] private InputField inPass;

   public static LoginUserDB instance;

   private void Awake () {
      if (instance != null && instance != this) {
         Destroy(gameObject);
      } else {
         instance = this;
      }
   }

   public void CleanAllDatas () {
      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
      inNick.text = "";
      inPass.text = "";
   }
   public void LoginUser () {
      if (inNick.text.Length > 0) {
         if (inPass.text.Length > 0) {
            StartCoroutine(LoginDB());
            return;
         } else {
            GameObject.Find("txtMss1").GetComponent<Text>().text = "Enter your Password";
         }
      } else {
         GameObject.Find("txtMss1").GetComponent<Text>().text = "Enter your Nick";
      }
      GlobalManager.events.failed1();
   }

   IEnumerator LoginDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      //string url = "http://universalattack.000webhostapp.com/codes/login.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/login.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss1").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GameObject.Find("txtMss1").GetComponent<Text>().text = "Connection error, try again";
            GlobalManager.events.failed1();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2": // Contraseña incorrecta
               GameObject.Find("txtMss1").GetComponent<Text>().text = "Incorrect Password";
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            case "3": // Nick no encontrado
               GameObject.Find("txtMss1").GetComponent<Text>().text = "Nick not found";
               inPass.text = "";
               GlobalManager.events.failed1();
               break;
            default: // Evaluar respuesta
               string[] datos = wr.downloadHandler.text.Split(new char[] { '♦', '►' }, StringSplitOptions.RemoveEmptyEntries);
               wr.Dispose();
               if (datos[0] == "1" && datos.Length == 5) {
                  TextWriter user = new StreamWriter(Application.persistentDataPath + "/User.txt", false);
                  user.WriteLine("usid" + "\t" + datos[1].ToString());
                  user.WriteLine("nick" + "\t" + inNick.text);
                  user.WriteLine("pass" + "\t" + inPass.text);
                  user.WriteLine("age" + "\t" + datos[2].ToString());
                  user.WriteLine("date" + "\t" + datos[3].ToString());
                  user.WriteLine("time" + "\t" + datos[4].ToString());
                  user.Close();
                  CleanAllDatas();
                  GlobalManager.events.success2();
               } else {
                  GameObject.Find("txtMss1").GetComponent<Text>().text = "Connection error, try again";
                  GlobalManager.events.failed1();
               }
               break;
            }
         }
      }
   }
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
}
