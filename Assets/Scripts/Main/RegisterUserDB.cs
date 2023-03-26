using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RegisterUserDB : MonoBehaviour {
   [SerializeField] private InputField inNick;
   [SerializeField] private InputField inPass;
   [SerializeField] private Dropdown inARQuestion;
   [SerializeField] private Dropdown inSecQuestions;
   [SerializeField] private InputField inAnswer;
   [SerializeField] private Toggle tgConsent;

   public static RegisterUserDB instance;
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
      inPass.text = "";
      inARQuestion.value = 0;
      inSecQuestions.value = 0;
      inAnswer.text = "";
   }
   public void RegisterUser () {
      if (inNick.text.Length >= 3) {
         if (inPass.text.Length >= 6) {
            if (inARQuestion.value > 0) {
               if (inSecQuestions.value > 0) {
                  if (inAnswer.text.Length > 0) {
                     if (tgConsent.isOn) {
                        StartCoroutine(RegisterDB());
                        return;
                     } else {
                        Message("Accept informed consent");
                     }
                  } else {
                     Message("Invalid answer");
                  }
               } else {
                  Message("Select a recovery question");
               }
            } else {
               Message("Select Yes or No");
            }
         } else {
            Message("Invalid Password");
            inPass.text = "";
         }
      } else {
         Message("Invalid Nick");
      }
      GlobalManager.events.failed2();
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
                  inSecQuestions.options.Add(new Dropdown.OptionData(dat[1]));
               }
            }
         }
         wr.Dispose();
      }
   }
   private IEnumerator RegisterDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      if (inARQuestion.value == 1) {
         form.AddField("arqu", "YES");
      } else {
         form.AddField("arqu", "NO");
      }
      form.AddField("quid", inSecQuestions.value);
      form.AddField("answer", inAnswer.text.ToLower());
      string url = "https://semilleroarvrunicauca.com/invasion-victory/register.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         Message("Loading . . .");
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Connection error, try again");
            GlobalManager.events.failed2();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "1": // Nick Existente
               Message("Nick already exists");
               inPass.text = "";
               GlobalManager.events.failed2();
               break;
            case "2": // Error de registro
               Message("Registration error, please try again later");
               inPass.text = "";
               GlobalManager.events.failed2();
               break;
            case "3": // Usuario registrado
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            default:
               Message("Connection error, try again");
               GlobalManager.events.failed2();
               break;
            }
         }
         wr.Dispose();
      }
   }
   private void Message(string mss) {
      GameObject.Find("txtMss2").GetComponent<Text>().text = mss;
   }
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
}
