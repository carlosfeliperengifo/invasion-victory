using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BtnShoot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
   [SerializeField] private float holdTime = 1f;
   [SerializeField] private GameObject weapon;

   public UnityEvent onLongPress = new UnityEvent();

   private bool isLongPress = false;

   public void OnPointerDown (PointerEventData eventData) {
      Invoke("OnLongPress", holdTime);
   }
   public void OnPointerUp (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      if (isLongPress) {
         weapon.GetComponent<Weapon>().IsAutoShot = false;
         weapon.GetComponent<Weapon>().outFire();
      } else {
         weapon.GetComponent<Weapon>().ShootBurst();
      }
   }
   public void OnPointerExit (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      isLongPress = false;
   }
   private void OnLongPress () {
      isLongPress = true;
      onLongPress.Invoke();
      weapon.GetComponent<Weapon>().IsAutoShot = true;
   }
}
