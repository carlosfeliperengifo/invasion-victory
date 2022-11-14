using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;
using System;
using UnityEngine.Networking;

public class MainManager : MonoBehaviour {
   [SerializeField] private GameObject LoginCanvas;
   [SerializeField] private GameObject RegisterCanvas;
   [SerializeField] private GameObject RecoverCanvas;
   [SerializeField] private GameObject ConsentCanvas;
   [SerializeField] private GameObject HistoryCanvas;
   [SerializeField] private GameObject UpdateCanvas;

   public Color[] backgrounds;
   public Sprite[] images;

   private int updateTime = 5;
   private float initialTime = 0;

   private bool inHistory = false;

   private void Awake () {
      inHistory = false;
      //HistoryCanvas.GetComponentInChildren<VideoPlayer>().Stop();
   }
   void Start () {
      StartCoroutine(updateApp());
      CanvasManager.instance.OnLoginMenu += ActivateLoginMenu;
      CanvasManager.instance.OnRegisterMenu += ActivateRegisterMenu;
      CanvasManager.instance.OnRecoverMenu += ActivateRecoverMenu;
      CanvasManager.instance.OnConsentMenu += ActivateConsent;
      CanvasManager.instance.OnHistoryMenu += ActivateHistory;

      CanvasManager.instance.LoginMenu();
   }
   private void Update () {
      if (Time.time >= initialTime) {
         int posBg = UnityEngine.Random.Range(0, backgrounds.Length);
         int posImage = UnityEngine.Random.Range(0, images.Length);
         transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = backgrounds[posBg];
         transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = images[posImage];
         initialTime = Time.time + updateTime;
      }

      if (HistoryCanvas.GetComponentInChildren<VideoPlayer>().isPlaying && !inHistory) {
         inHistory = true;
      }
      if (inHistory && !HistoryCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inHistory = false;
         GameObject.Find("ScenesControl").GetComponent<SceneCtrl>().LoadScene("01_User");
      }
   }
   private void ActivateLoginMenu () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      HistoryCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
   }
   private void ActivateRegisterMenu () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
   }
   private void ActivateRecoverMenu () {
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      GameObject.Find("txtMss1").GetComponent<Text>().text = "";
      GameObject.Find("txtMss2").GetComponent<Text>().text = "";
      GameObject.Find("txtMss3").GetComponent<Text>().text = "";
   }
   private void ActivateConsent () {
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      TextAsset consent = Resources.Load<TextAsset>("InformedConsent");
      ConsentCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = consent.text;
   }
   private void ActivateHistory () {
      HistoryCanvas.SetActive(true);
      //HistoryCanvas.GetComponentInChildren<VideoPlayer>().Stop();
      LoginCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RegisterCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      RecoverCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ConsentCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      HistoryCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      //HistoryCanvas.GetComponentInChildren<VideoPlayer>().Play();
   }
   IEnumerator updateApp () {
      WWWForm form = new WWWForm();
      //string url = "http://universalattack.000webhostapp.com/codes/update.php";
      string url = "https://semilleroarvrunicauca.com/invasion-victory/update.php";
      using (UnityWebRequest wr = UnityWebRequest.Post(url, form)) {
         yield return wr.SendWebRequest();
         if (wr.result != UnityWebRequest.Result.Success) {
            Debug.Log(wr.error);
         } else {
            if (wr.downloadHandler.text.Length <= 5) {
               if (wr.downloadHandler.text == "V1.0") {
                  UpdateCanvas.SetActive(false);
               } else {
                  UpdateCanvas.SetActive(true);
               }
            }
         }
      }
   }
}
