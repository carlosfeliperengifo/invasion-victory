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

   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inQuestions.value = 0;
      inAnswer.text = "";
      inPass.text = "";
   }
   public IEnumerator GetQuestions () {
      Message("");
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      string url = "https://semilleroarvrunicauca.com/invasion-victory/questions.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Connection error, try again");
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
                  Message("Invalid Password");
                  inPass.text = "";
               }
            } else {
               Message("Invalid answer");
            }
         } else {
            Message("Select a recovery question");
         }
      } else {
         Message("Invalid Nick");
      }
      GlobalManager.events.failed3();
   }
   IEnumerator UpdatePassDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.text.ToLower());
      string url = "https://semilleroarvrunicauca.com/invasion-victory/editPass.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         Message("Loading . . .");
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Connection error");
            GlobalManager.events.failed3();
         } else { // Servidor OK
            switch (wr.downloadHandler.text) {
            case "1":
               Message("");
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            case "3":
               Message("Incorrect answer");
               GlobalManager.events.failed3();
               break;
            case "4":
               Message("Incorrect question");
               GlobalManager.events.failed3();
               break;
            case "5":
               Message("Nick not found");
               GlobalManager.events.failed3();
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               Message("Connection error");
               GlobalManager.events.failed3();
               break;
            }
         }
         wr.Dispose();
      }
   }
   private void Message (string mss) {
      GameObject.Find("txtMss3").GetComponent<Text>().text = mss;
   }
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
}
