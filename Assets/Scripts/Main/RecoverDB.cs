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

   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   public void CleanAllDatas () {
      Message("");
      inNick.text = "";
      inQuestions.value = 0;
      inAnswer.text = "";
      inPass.text = "";
   }
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
         } else { // Servidor OK
            switch (wr.downloadHandler.text) {
            case "1":
               Message("");
               CleanAllDatas();
               GlobalManager.events.success1();
               break;
            case "3":
               Message("Respuesta incorrecta");
               GlobalManager.events.failed3();
               break;
            case "4":
               Message("Pregunta incorrecta");
               GlobalManager.events.failed3();
               break;
            case "5":
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
