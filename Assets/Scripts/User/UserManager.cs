using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class UserManager : MonoBehaviour {
   [SerializeField] private GameObject PlayCanvas;
   [SerializeField] private GameObject ParamCanvas;
   [SerializeField] private GameObject CreditsCanvas;
   [SerializeField] private GameObject FeaturesCanvas;
   [SerializeField] private GameObject MoveCanvas;

   public Color[] backgrounds;
   public Sprite[] images;

   private int updateTime = 5;
   private float initialTime = 0;

   private bool inVideo = false;
   void Start () {
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Stop();
      GameObject.Find("txLoading").transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CanvasManager.instance.OnPlayMenu += ActivatePlayMenu;
      CanvasManager.instance.OnParamMenu += ActivateParamMenu;
      CanvasManager.instance.OnCreditsMenu += ActivateCredits;
      CanvasManager.instance.OnFeaturesMenu += ActivateFeatures;
      CanvasManager.instance.OnMoveMenu += ActivateMove;

      CanvasManager.instance.PlayMenu();
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/User.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' });
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "nick":
            GameObject.Find("txtNick").GetComponent<Text>().text = col[1];
            break;
         }
      }
   }
   private void Update () {
      if (Time.time >= initialTime) {
         int posBg = Random.Range(0, backgrounds.Length);
         int posImage = Random.Range(0, images.Length);
         transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = backgrounds[posBg];
         transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = images[posImage];
         initialTime = Time.time + updateTime;
      }

      if (MoveCanvas.GetComponentInChildren<VideoPlayer>().isPlaying && !inVideo) {
         inVideo = true;
      }
      if (inVideo && !MoveCanvas.GetComponentInChildren<VideoPlayer>().isPlaying) {
         inVideo = false;
         GameObject.Find("ScenesControl").GetComponent<SceneCtrl>().LoadScene("02_Game");
      }
   }
   public void Loading (bool st) {
      if (st) {
         GameObject.Find("txLoading").transform.DOScale(new Vector3(1, 1, 1), 0.2f);
      } else {
         GameObject.Find("txLoading").transform.DOScale(new Vector3(0, 0, 0), 0.2f);
      }
   }
   private void ActivatePlayMenu () {
      Loading(false);
      PlayCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      PlayCanvas.transform.GetChild(1).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      FeaturesCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      MoveCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   private void ActivateParamMenu () {
      Loading(false);
      PlayCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      PlayCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      FeaturesCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      MoveCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   private void ActivateCredits () {
      Loading(false);
      PlayCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      PlayCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      FeaturesCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      MoveCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

      TextAsset credits = Resources.Load<TextAsset>("Credits");
      CreditsCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = credits.text;
   }
   private void ActivateFeatures () {
      Loading(false);
      PlayCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      PlayCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      FeaturesCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      MoveCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
   }
   private void ActivateMove () {
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Stop();
      PlayCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      PlayCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      ParamCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      CreditsCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      FeaturesCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
      MoveCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
      MoveCanvas.GetComponentInChildren<VideoPlayer>().Play();
   }
}
