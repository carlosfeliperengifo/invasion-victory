using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateApp : MonoBehaviour {

   IEnumerator Start () {
      WWWForm form = new WWWForm();
      //string url = "http://universalattack.000webhostapp.com/codes/update.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/update.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            if (wr.downloadHandler.text.Length <= 5) {
               string version = "V" + Application.version;
               version = version.Replace(",", ".");
               Debug.Log(version);
               if (wr.downloadHandler.text == version) {
                  transform.GetChild(0).gameObject.SetActive(false);
               } else {
                  transform.GetChild(0).gameObject.SetActive(true);
               }
            }
         }
      }
   }
}
