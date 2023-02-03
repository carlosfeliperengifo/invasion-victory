using UnityEngine;
using UnityEngine.EventSystems;

public class BtnShoot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
   [SerializeField] private float holdTime = 0.4f;
   [SerializeField] private Player player;


   private bool isLongPress = false;

   public void OnPointerDown (PointerEventData eventData) {
      Invoke("OnLongPress", holdTime);
   }
   public void OnPointerUp (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      if (isLongPress) {
         isLongPress = false;
         player.PlayerTransitions(Player.Transition.wait);
      } else {
         player.PlayerTransitions(Player.Transition.shootBurst);
      }
   }
   public void OnPointerExit (PointerEventData eventData) {
      CancelInvoke("OnLongPress");
      isLongPress = false;
   }
   private void OnLongPress () {
      isLongPress = true;
      player.PlayerTransitions(Player.Transition.autoShot);
   }
}
