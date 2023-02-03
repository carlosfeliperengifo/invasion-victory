using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RegDatosA : MonoBehaviour {
   private WWWForm sesionForm;
   private WWWForm mathForm;
   private WWWForm performanceForm;

   private bool enab = false;
   private int k;
   private string[] datos;

   void Start () {
      TextAsset DatosTxt = Resources.Load<TextAsset>("DatosAnteriores");
      datos = DatosTxt.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Debug.Log(datos.Length);
      k = 0;
      enab = true;
   }
   void Update () {
      if (enab && k < datos.Length) {
         string[] col = datos[k].Split(new char[] { '\t' });

         Debug.Log("col: " + col.Length.ToString());
         sesionForm = new WWWForm();
         mathForm = new WWWForm();
         performanceForm = new WWWForm();

         sesionForm.AddField("usid", col[0]);
         sesionForm.AddField("date", col[5]);
         sesionForm.AddField("time", col[6]);

         mathForm.AddField("serial", col[14]);
         mathForm.AddField("spaceshipsHorde", col[15]);
         mathForm.AddField("timeHorde", col[16]);
         mathForm.AddField("speedSpaceships", col[17].Replace(',', '.'));

         performanceForm.AddField("completedGame", col[8]);
         performanceForm.AddField("score", col[26]);
         performanceForm.AddField("destroyedSpaceships", col[9]);
         performanceForm.AddField("destroyed7Total", col[10].Replace(',', '.'));
         performanceForm.AddField("lifeBar", col[11]);
         performanceForm.AddField("timePlayed", col[12].Replace(',', '.'));

         enab = false;
         StartCoroutine(SesionDB());
      }
   }

   IEnumerator SesionDB () {
      string url = "https://semilleroarvrunicauca.com/invasion-victory/sesion.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, sesionForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor ok
            Debug.Log("Se: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("seid", datos[1].ToString());
               StartCoroutine(MatchDB());
            }
         }
      }
   }
   IEnumerator MatchDB () {
      string url = "https://semilleroarvrunicauca.com/invasion-victory/match.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, mathForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor ok
            Debug.Log("Ma: " + wr.downloadHandler.text);
            string[] datos = wr.downloadHandler.text.Split(new char[] { ':' });
            if (datos[0] == "1" && datos.Length == 2) {
               performanceForm.AddField("maid", datos[1].ToString());
               StartCoroutine(PerformanceDB());
            }
         }
      }
   }
   IEnumerator PerformanceDB () {
      string url = "https://semilleroarvrunicauca.com/invasion-victory/performance.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, performanceForm)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else { // Servidor ok
            switch (wr.downloadHandler.text) {
            case "2":
               Debug.Log("Performance registrado");
               break;
            default:
               Debug.Log(wr.downloadHandler.text);
               break;
            }
            Debug.Log(k);
            k++;
            enab = true;
         }
      }
   }
   IEnumerator updateApp () {
      WWWForm form = new WWWForm();
      string url = "https://semilleroarvrunicauca.com/invasion-victory/update.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            if (wr.downloadHandler.text.Length <= 5) {
               if (wr.downloadHandler.text == "V1.0") {
                  Debug.Log("k: " + k.ToString());
                  k++;
                  enab = true;
               }
            }
         }
      }
   }
}
