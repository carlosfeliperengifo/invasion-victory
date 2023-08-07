using UnityEngine;
using UnityEngine.EventSystems;

public class BtnShoot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
   [SerializeField] private float holdTime = 0.4f;
   [SerializeField] private Player player;


   private bool isLongPress = false;

   // Invoke the OnLongPress when the button is held down holdTime seconds
   public void OnPointerDown (PointerEventData eventData) {
      Invoke("OnLongPress", holdTime);
   }
   // When the button is pressed once
   public void OnPointerUp (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      if (isLongPress) {
         isLongPress = false;
         player.PlayerTransitions(Player.Transition.wait);
      } else {
         player.PlayerTransitions(Player.Transition.shootBurst);
      }
   }
   // When the button has been released
   public void OnPointerExit (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      isLongPress = false;
   }
   // If the button
   private void OnLongPress () {
      isLongPress = true;
      player.PlayerTransitions(Player.Transition.autoShot);
   }
}
