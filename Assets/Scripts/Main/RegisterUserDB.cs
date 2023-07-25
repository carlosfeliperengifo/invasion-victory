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

   [SerializeField] private Text message;

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inPass.text = "";
      inARQuestion.value = 0;
      inSecQuestions.value = 0;
      inAnswer.text = "";
      tgConsent.isOn = false;
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
                        Message("Acepta el consentimiento informado");
                     }
                  } else {
                     Message("Respuesta inválida");
                  }
               } else {
                  Message("Selecciona una pregunta de seguridad");
               }
            } else {
               Message("Selecciona Si o No");
            }
         } else {
            Message("Contraseña inválida");
            inPass.text = "";
         }
      } else {
         Message("Apodo inválido");
      }
      GlobalManager.events.failed2();
   }
   public IEnumerator GetQuestions () {
      if (inSecQuestions.options.Count <=1 ) {
         Message("");
         WWWForm form = new WWWForm();
         form.AddField("nick", "ivar");
         string url = dataPath + "/questions.php";
         using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
            yield return wr.SendWebRequest();
            if (wr.result != UnityWebRequest.Result.Success) {
               Debug.Log(wr.error);
               Message("Error de conexión, vuelve a intentar");
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
      } else {
         yield return null;
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
      string url = dataPath + "/register.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         Message("Cargando . . .");
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión, vuelve a intentar");
            GlobalManager.events.failed2();
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "1": // Nick Existente
               Message("El apodo ya existe");
               inPass.text = "";
               GlobalManager.events.failed2();
               break;
            case "2": // Error de registro
               Message("Error de registro, intenta más tarde");
               GlobalManager.events.failed2();
               break;
            case "3": // Usuario registrado
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            default:
               Message("Error de conexión, vuelve a intentar");
               GlobalManager.events.failed2();
               break;
            }
         }
         wr.Dispose();
      }
   }
   private void Message (string mss) {
      message.text = mss;
   }
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
}
