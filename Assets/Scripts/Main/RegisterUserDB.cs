using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

public class RegisterUserDB : MonoBehaviour {
   [SerializeField] private InputField inNick;
   [SerializeField] private InputField inPass;
   [SerializeField] private InputField inAge;
   [SerializeField] private Dropdown inQuestions;
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

   public void GetQuestions () {
      StartCoroutine(Questions());
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
   }
   public void CleanAllDatas () {
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
      inNick.text = "";
      inPass.text = "";
      inAge.text = "";
      inQuestions.value = 0;
      inAnswer.text = "";
   }
   public void RegisterUser () {
      if (inNick.text.Length >= 3) {
         if (inPass.text.Length >= 6) {
            if (inAge.text.Length > 0) {
               if (inQuestions.value > 0) {
                  if (inAnswer.text.Length > 0) {
                     if (tgConsent.isOn) {
                        StartCoroutine(RegisterDB());
                        return;
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
            inPass.text = "";
         }
      } else {
         GameObject.Find("txtMss2").GetComponent<Text>().text = "Invalid Nick";
      }
      GlobalManager.events.failed2();
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
            GameObject.Find("txtMss2").GetComponent<Text>().text = "Connection error, try again";
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
         wr.Dispose();
      }
   }
   IEnumerator RegisterDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      form.AddField("age", inAge.text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.text.ToLower());

      //string url = "http://universalattack.000webhostapp.com/codes/register.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/register.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         GameObject.Find("txtMss2").GetComponent<Text>().text = "Loading . . .";
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            GameObject.Find("txtMss2").GetComponent<Text>().text = "Connection error, try again";
            GlobalManager.events.failed2();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "1": // Nick Existente
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Nick already exists";
               inPass.text = "";
               GlobalManager.events.failed2();
               break;
            case "2": // Error de registro
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Registration error, please try again later";
               inPass.text = "";
               GlobalManager.events.failed2();
               break;
            case "3": // Usuario registrado
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            default:
               GameObject.Find("txtMss2").GetComponent<Text>().text = "Connection error, try again";
               GlobalManager.events.failed2();
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
