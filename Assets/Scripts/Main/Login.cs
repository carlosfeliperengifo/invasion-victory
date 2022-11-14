using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour {
   public GameObject inNick;
   public GameObject inPass;

   private void Start () {
      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
   }
   public void CleanAllDatas () {
      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
      inNick.GetComponent<InputField>().text = "";
      inPass.GetComponent<InputField>().text = "";
   }
   public void LoginUser () {
      if (inNick.GetComponent<InputField>().text.Length > 0) {
         if (inPass.GetComponent<InputField>().text.Length > 0) {
            StartCoroutine(LoginUserDB());
         } else {
            GameObject.Find("txtMss1").GetComponent<Text>().text = "Enter your Password";
         }
      } else {
         GameObject.Find("txtMss1").GetComponent<Text>().text = "Enter your Nick";
      }
   }

   IEnumerator LoginUserDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.GetComponent<InputField>().text);
      form.AddField("password", inPass.GetComponent<InputField>().text);
      //string url = "http://universalattack.000webhostapp.com/codes/login.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/login.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss1").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2": // Contraseña incorrecta
               GameObject.Find("txtMss1").GetComponent<Text>().text = "Incorrect Password";
               inPass.GetComponent<InputField>().text = "";
               break;
            case "3": // Nick no encontrado
               GameObject.Find("txtMss1").GetComponent<Text>().text = "Nick not found";
               inPass.GetComponent<InputField>().text = "";
               break;
            default: // Evaluar respuesta
               string[] datos = wr.downloadHandler.text.Split(new char[] { '♦', '►' }, StringSplitOptions.RemoveEmptyEntries);

               if (datos[0] == "1" && datos.Length == 5) {
                  TextWriter user = new StreamWriter(Application.persistentDataPath + "/User.txt", false);
                  user.WriteLine("usid" + "\t" + datos[1].ToString());
                  user.WriteLine("nick" + "\t" + inNick.GetComponent<InputField>().text);
                  user.WriteLine("pass" + "\t" + inPass.GetComponent<InputField>().text);
                  user.WriteLine("age" + "\t" + datos[2].ToString());
                  user.WriteLine("date" + "\t" + datos[3].ToString());
                  user.WriteLine("time" + "\t" + datos[4].ToString());
                  user.Close();
                  CleanAllDatas();
                  CanvasManager.instance.HistoryMenu();
               }
               break;
            }
         }
      }
   }
}
