using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Torneo : MonoBehaviour {

   IEnumerator Start () {
      WWWForm form = new WWWForm();
      string url = "https://semilleroarvrunicauca.com/invasion-victory/torneo.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            if (wr.downloadHandler.text.Length <= 4) {
               if (wr.downloadHandler.text == "T:1") {
                  transform.GetChild(0).gameObject.SetActive(false);
               } else {
                  transform.GetChild(0).gameObject.SetActive(true);
               }
            }
         }
      }
   }
}
