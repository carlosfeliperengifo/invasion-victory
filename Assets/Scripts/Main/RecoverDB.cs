using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecoverDB : MonoBehaviour {
   [SerializeField] private InputField inNick;
   [SerializeField] private Dropdown inQuestions;
   [SerializeField] private InputField inAnswer;
   [SerializeField] private InputField inPass;

   public static RecoverDB instance;
   private void Awake () {
      if (instance != null && instance != this) {
         Destroy(gameObject);
      } else {
         instance = this;
      }
   }

   public void GetQuestions () {
      StartCoroutine(Questions());
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
   }
   public void CleanAllDatas () {
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
      inNick.text = "";
      inQuestions.value = 0;
      inAnswer.text = "";
      inPass.text = "";
   }
   IEnumerator Questions () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);

      //string url = "http://universalattack.000webhostapp.com/codes/questions.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/questions.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GameObject.Find("txtMss3").GetComponent<Text>().text = "Connection error, try again";
         } else {
            string[] datos = wr.downloadHandler.text.Split(new char[] { '%', '%' }, StringSplitOptions.RemoveEmptyEntries);
            if (datos.Length > 0) {
               foreach (string dato in datos) {
                  string[] dat = dato.Split(new char[] { '&', '&' }, StringSplitOptions.RemoveEmptyEntries);
                  inQuestions.options.Add(new Dropdown.OptionData(dat[1]));
               }
            }
         }
         wr.Dispose();
      }
   }
   public void EditPassword () {
      if (inNick.text.Length >= 3) {
         if (inQuestions.value > 0) {
            if (inAnswer.text.Length > 0 && inAnswer.text != " ") {
               if (inPass.text.Length >= 6) {
                  StartCoroutine(UpdatePassDB());
                  return;
               } else {
                  GameObject.Find("txtMss3").GetComponent<Text>().text = "Invalid Password";
                  inPass.text = "";
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
      GlobalManager.events.failed3();
   }
   IEnumerator UpdatePassDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.text.ToLower());

      //string url = "http://universalattack.000webhostapp.com/codes/questions.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/editPass.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss3").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GameObject.Find("txtMss3").GetComponent<Text>().text = "Connection error";
            GlobalManager.events.failed3();
         } else { // Servidor OK
            switch (wr.downloadHandler.text) {
            case "1":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "";
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            case "3":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Incorrect answer";
               GlobalManager.events.failed3();
               break;
            case "4":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Incorrect question";
               GlobalManager.events.failed3();
               break;
            case "5":
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Nick not found";
               GlobalManager.events.failed3();
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               GameObject.Find("txtMss3").GetComponent<Text>().text = "Connection error";
               GlobalManager.events.failed3();
               break;
            }
         }
         wr.Dispose();
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
