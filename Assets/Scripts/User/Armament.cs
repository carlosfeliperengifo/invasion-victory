using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Armament : MonoBehaviour {
   [SerializeField] private Text nick;
   [SerializeField] private Image imLevel;
   [SerializeField] private Text txPoints;
   [SerializeField] private GameObject[] weapons;
   [SerializeField] private Transform pointer;
   [SerializeField] private Image[] frames;
   [SerializeField] private Button btAccept;
   [SerializeField] private Transform locked;

   private Transform currentWeapon;

   private int idLevel;
   private int idWeapon;
   private int idMat;

   public int IdLevel { set { idLevel = value; } }

   private Vector3 baseScale = 1300 * Vector3.one;
   private bool isFirst = true;
   float zRot = 0, yRot = 0;
   Vector3 startPos;
   Vector3 startRot;

   public void LoadConfiguration (string nick, Sprite im, string pt) {
      idWeapon = -1;
      ChangeWeapon(PlayerPrefs.GetInt("idWeapon", 0));
      idMat = PlayerPrefs.GetInt("idMat", 0);
      this.nick.text = nick;
      imLevel.sprite = im;
      txPoints.text = pt;
      new WaitForSeconds(0.2f);
      ChangeTexture(0);
   }
   public void SaveParameters () {
      CancelRotateGun();
      PlayerPrefs.SetInt("idWeapon", idWeapon);
      PlayerPrefs.SetInt("idMat", idMat);
   }

   private void ChangeWeapon (int wp) {
      CancelRotateGun();
      if (wp < 0) { wp = 0; }
      locked.localScale = Vector3.zero;
      if (wp != idWeapon) {
         if (pointer.childCount > 0) {
            Destroy(pointer.GetChild(0).gameObject);
         }
         idMat = 0;
         currentWeapon = Instantiate(weapons[wp], pointer).transform;
         ChangeTexture(0);
         HighlightFrame(wp);
         currentWeapon.eulerAngles = new Vector3(0, 90, 0);
         ScaleWeapon(false);
         idWeapon = wp;
         startRot = currentWeapon.GetChild(0).GetChild(0).eulerAngles;
         yRot = startRot.y;
         zRot = startRot.z;
      }
      if (idWeapon >= idLevel) {
         locked.DOScale(Vector3.one, 0.3f);
         btAccept.interactable = false;
      } else {
         locked.localScale = Vector3.zero;
         btAccept.interactable = true;
         InvokeRepeating(nameof(RotateGun), 0, 0.05f);
      }
   }
   private void ChangeTexture (int st) {
      currentWeapon.GetComponent<Weapon>().ChangeMaterial(idMat + st);
      idMat = currentWeapon.GetComponent<Weapon>().IdMat;
   }
   private void HighlightFrame (int f) {
      foreach (Image fm in frames) {
         fm.color = new Color(1, 1, 1, 0.6f);
      }
      frames[f].color = new Color(0, 0.8f, 0.2f, 0.6f);
   }
   public void CancelRotateGun () {
      CancelInvoke(nameof(RotateGun));
   }

   private void RotateGun () {
      if (Input.touchCount > 0) {
         Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
         if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.transform.tag == "Weapon") {
               if (isFirst) {
                  isFirst = false;
                  startPos = Input.GetTouch(0).position;
                  return;
               } else {
                  zRot += (Input.GetTouch(0).position.y - startPos.y) * 0.09f;
                  zRot = Mathf.Clamp(zRot, -55, 55);
                  yRot += (Input.GetTouch(0).position.x - startPos.x) * 0.25f;
                  //yRot = Mathf.Clamp(yRot, -75, 75);
                  startPos = Input.GetTouch(0).position;
               }
               ScaleWeapon(true);
            }
         }
      } else {
         isFirst = true;
         zRot = Mathf.Lerp(zRot, startRot.z, 0.08f);
         yRot = Mathf.Lerp(yRot, startRot.y, 0.08f);
         ScaleWeapon(false);
      }
      currentWeapon.GetChild(0).GetChild(0).eulerAngles = new Vector3(0, yRot, zRot);
   }
   private void ScaleWeapon (bool op) {
      float duration = 0.5f;
      if (op) {
         currentWeapon.DOScale(1700 * Vector3.one, duration);
      } else {
         currentWeapon.DOScale(baseScale, duration);
      }
   }
}
