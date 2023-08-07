using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateApp : MonoBehaviour {
   private readonly string dataPath = "https://semilleroarvrunicauca.com/invasion-victory/IVAR_2";

   IEnumerator Start () {
      StartCoroutine(CheckVersion());
      yield return new WaitForSeconds(15);
   }
   // Check the game version
   private IEnumerator CheckVersion() {
      WWWForm form = new WWWForm();
      string url = dataPath + "/update.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            if (wr.downloadHandler.text.Length <= 5) {
               string version = "V" + Application.version;
               version = version.Replace(",", ".");
               Debug.Log(version);
               // Toggle lock screen based on the version
               if (wr.downloadHandler.text == version) {
                  transform.GetChild(0).gameObject.SetActive(false);
               } else {
                  transform.GetChild(0).gameObject.SetActive(true);
               }
            }
         }
         wr.Dispose();
      }
   }
   // Got to the Play Store
   public void GoUpdate () {
      Application.OpenURL("https://play.google.com/store/apps/details?id=com.unicauca.InvasionVictory");
   }
}
