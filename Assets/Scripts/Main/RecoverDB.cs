using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecoverDB : MonoBehaviour {
   // Text input variables for user data
   [SerializeField] private InputField inNick;
   [SerializeField] private Dropdown inQuestions;
   [SerializeField] private InputField inAnswer;
   [SerializeField] private InputField inPass;

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";
   // Clear all text inputs
   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inQuestions.value = 0;
      inAnswer.text = "";
      inPass.text = "";
   }
   // Check the length of the text fields
   public void EditPassword () {
      if (inNick.text.Length >= 3) {
         if (inQuestions.value > 0) {
            if (inAnswer.text.Length > 0 && inAnswer.text != " ") {
               if (inPass.text.Length >= 6) {
                  StartCoroutine(UpdatePassDB());
                  return;
               } else {
                  Message("Contraseña inválida");
                  inPass.text = "";
               }
            } else {
               Message("Respuesta inválida");
            }
         } else {
            Message("Selecciona una pregunta de seguridad");
         }
      } else {
         Message("Apodo inválido");
      }
      GlobalManager.events.failed3();
   }
   // Retrieve question options of the database
   public IEnumerator GetQuestions () {
      if (inQuestions.options.Count <= 1) {
         Message("");
         WWWForm form = new WWWForm();
         form.AddField("nick", inNick.text);
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
                     inQuestions.options.Add(new Dropdown.OptionData(dat[1]));
                  }
               }
            }
            wr.Dispose();
         }
      } else {
         yield return null;
      }
   }
   // Submit the form with the new user's data to the database
   IEnumerator UpdatePassDB () {
      WWWForm form = new WWWForm();
      form.AddField("nick", inNick.text);
      form.AddField("password", inPass.text);
      form.AddField("quid", inQuestions.value);
      form.AddField("answer", inAnswer.text.ToLower());
      string url = dataPath + "/recover.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         Message("Cargando . . .");
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
            Message("Error de conexión");
            GlobalManager.events.failed3();
         } else { // Server OK
            switch (wr.downloadHandler.text) {
            case "1": // Password updated
               Message("");
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            case "3": // Incorrect answer
               Message("Respuesta incorrecta");
               GlobalManager.events.failed3();
               break;
            case "4": // Incorrect question
               Message("Pregunta incorrecta");
               GlobalManager.events.failed3();
               break;
            case "5": // Nickname not found
               Message("Apodo no enontrado");
               GlobalManager.events.failed3();
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               Message("Error de conexión, vuelve a intentar");
               GlobalManager.events.failed3();
               break;
            }
         }
         wr.Dispose();
      }
   }
   // Display a message mss on the screen
   private void Message (string mss) {
      GameObject.Find("txtMss3").GetComponent<Text>().text = mss;
   }
   // Toggle the visibility of the password
   public void TogglePass () {
      if (inPass.contentType == InputField.ContentType.Password) {
         inPass.contentType = InputField.ContentType.Standard;
      } else {
         inPass.contentType = InputField.ContentType.Password;
      }
   }
}
