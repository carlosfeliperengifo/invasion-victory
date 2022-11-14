using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SaveDatasDB : MonoBehaviour {
   private WWWForm sesionForm;
   private WWWForm mathForm;
   private WWWForm performanceForm;

   void Start () {
      sesionForm = new WWWForm();
      mathForm = new WWWForm();
      performanceForm = new WWWForm();
      GetSesionTxt();
      GetMatchTxt();
      GetPerformanceTxt();
      StartCoroutine(SesionDB());
   }
   private void GetSesionTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "usid":
            sesionForm.AddField(col[0], col[1]);
            break;
         case "date":
            sesionForm.AddField(col[0], col[1]);
            break;
         case "time":
            sesionForm.AddField(col[0], col[1]);
            break;
         }
      }
   }
   private void GetMatchTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Match.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      string serial = "";
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            col[1] = col[1].Replace(',', '.');
            mathForm.AddField(col[0], col[1]);
            serial += col[1] + ".";
         }
      }
      serial = serial.Remove(serial.Length - 1);
      mathForm.AddField("serial", serial);
   }
   private void GetPerformanceTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Performance.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         if (col.Length == 2) {
            if (col[0] == "completedGame") {
               if (bool.Parse(col[1])) {
                  col[1] = "1";
               } else { col[1] = "0"; }
            }
            col[1] = col[1].Replace(',', '.');
            performanceForm.AddField(col[0], col[1]);
         }
      }
   }
   IEnumerator SesionDB () {
      //string url = "http://universalattack.000webhostapp.com/codes/sesion.php";
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
      //string url = "http://universalattack.000webhostapp.com/codes/match.php";
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
      //string url = "http://universalattack.000webhostapp.com/codes/performance.php";
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
         }
      }
   }
}
