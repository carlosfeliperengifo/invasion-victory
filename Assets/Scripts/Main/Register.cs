using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class Register : MonoBehaviour {
   [SerializeField] private GameObject inNick;
   [SerializeField] private GameObject inPass;
   [SerializeField] private GameObject inAge;
   [SerializeField] private GameObject tgConsent;
   [SerializeField] private Dropdown inQuestions;
   [SerializeField] private GameObject inAnswer;

   private void Start () {
      StartCoroutine(Questions());
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
   }
   public void CleanAllDatas () {
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
      inNick.GetComponent<InputField>().text = "";
      inPass.GetComponent<InputField>().text = "";
      inAge.GetComponent<InputField>().text = "";
   }
   public void RegisterUser () {
      if (inNick.GetComponent<InputField>().text.Length >= 3) {
         if (inPass.GetComponent<InputField>().text.Length >= 6) {
            if (inAge.GetComponent<InputField>().text.Length > 0) {
               if (inQuestions.value > 0) {
                  if (inAnswer.GetComponent<InputField>().text.Length > 0) {
                     if (tgConsent.GetComponent<Toggle>().isOn) {
                        StartCoroutine(RegisterUserDB());
                     } else {
                        GameObject.Find("txtMss2").GetComponent<Text>().text = "Accept informed consent";
                     }
                  } else {
                     GameObject.Find("txtMss2").GetComponent<Text>().text = "Invalid answer";
                  }
               } else {
                  GameObject.Find("txtMss2").GetComponent<Text>().text = "Select a recovery question";
               }
            } else {
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Required Age";
            }
         } else {
            GameObject.Find("txtMss2").GetComponent<Text>().text = "Invalid Password";
            inPass.GetComponent<InputField>().text = "";
         }
      } else {
         GameObject.Find("txtMss2").GetComponent<Text>().text = "Invalid Nick";
      }
   }
   IEnumerator Questions () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.GetComponent<InputField>().text);

      //string url = "http://universalattack.000webhostapp.com/codes/questions.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/questions.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
            if (datos.Length > 0) {
               TextWriter topFile = new StreamWriter(Application.persistentDataPath + "/TopUser.txt", false);

               foreach (string dato in datos) {
                  string[] dat = dato.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
                  inQuestions.options.Add(new Dropdown.OptionData(dat[1]));
               }
            }
         }
      }
   }
   IEnumerator RegisterUserDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.GetComponent<InputField>().text);
      form.AddField("password", inPass.GetComponent<InputField>().text);
      form.AddField("age", inAge.GetComponent<InputField>().text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.GetComponent<InputField>().text.ToLower());

      //string url = "http://universalattack.000webhostapp.com/codes/register.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/register.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss2").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "1": // Nick Existente
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Nick already exists";
               inPass.GetComponent<InputField>().text = "";
               break;
            case "2": // Error de registro
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Registration error, please try again later";
               inPass.GetComponent<InputField>().text = "";
               break;
            case "3": // Usuario registrado
               CleanAllDatas();
               CanvasManager.instance.LoginMenu();
               break;
            }
         }
      }
   }
}
