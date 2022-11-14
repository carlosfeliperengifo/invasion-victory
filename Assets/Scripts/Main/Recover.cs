using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Recover : MonoBehaviour {
   [SerializeField] private GameObject inNick;
   [SerializeField] private GameObject inPass;
   [SerializeField] private Dropdown inQuestions;
   [SerializeField] private GameObject inAnswer;

   void Start () {
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
      StartCoroutine(Questions());
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

               foreach (string dato in datos) {
                  string[] dat = dato.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
                  inQuestions.options.Add(new Dropdown.OptionData(dat[1]));
               }
            }
         }
      }
   }
   public void EditPassword () {
      if (inNick.GetComponent<InputField>().text.Length >= 3) {
         if (inQuestions.value > 0) {
            if (inAnswer.GetComponent<InputField>().text.Length > 0) {
               if (inPass.GetComponent<InputField>().text.Length >= 6) {
                  StartCoroutine(EditPassDB());
               } else {
                  GameObject.Find("txtMss3").GetComponent<Text>().text = "Invalid Password";
                  inPass.GetComponent<InputField>().text = "";
               }
            } else {
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Invalid answer";
            }
         } else {
            GameObject.Find("txtMss3").GetComponent<Text>().text = "Select a recovery question";
         }
      } else {
         GameObject.Find("txtMss3").GetComponent<Text>().text = "Invalid Nick";
      }
   }
   IEnumerator EditPassDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.GetComponent<InputField>().text);
      form.AddField("password", inPass.GetComponent<InputField>().text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.GetComponent<InputField>().text.ToLower());

      //string url = "http://universalattack.000webhostapp.com/codes/questions.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/editPass.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss3").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor OK
            switch (wr.downloadHandler.text) {
            case "1":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "";
               CanvasManager.instance.LoginMenu();
               break;
            case "3":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Incorrect answer";
               break;
            case "4":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Incorrect question";
               break;
            case "5":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Nick not found";
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               break;
            }
         }
      }
   }
}
