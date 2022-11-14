using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanvasManager : MonoBehaviour {
   public event Action OnLoginMenu;
   public event Action OnRegisterMenu;
   public event Action OnRecoverMenu;
   public event Action OnConsentMenu;
   public event Action OnHistoryMenu;
   public event Action OnPlayMenu;
   public event Action OnParamMenu;
   public event Action OnCreditsMenu;
   public event Action OnFeaturesMenu;
   public event Action OnMoveMenu;
   public event Action OnGameMenu;
   public event Action OnPauseMenu;
   public event Action OnGameOverMenu;
   public event Action OnCongratMenu;

   public static CanvasManager instance;

   private void Awake () {
      if (instance != null && instance != this) {
         Destroy(gameObject);
      } else {
         instance = this;
      }
   }
   void Start () {
   }
   public void LoginMenu () {
      OnLoginMenu?.Invoke();
   }
   public void RegisterMenu () {
      OnRegisterMenu?.Invoke();
   }
   public void RecoverMenu () {
      OnRecoverMenu?.Invoke();
   }
   public void ConsentMenu () {
      OnConsentMenu?.Invoke();
   }
   public void HistoryMenu () {
      OnHistoryMenu?.Invoke();
   }
   public void PlayMenu () {
      OnPlayMenu?.Invoke();
   }
   public void ParamMenu () {
      OnParamMenu?.Invoke();
   }
   public void CreditsMenu () {
      OnCreditsMenu?.Invoke();
   }
   public void FeaturesMenu () {
      OnFeaturesMenu?.Invoke();
   }
   public void MovementMenu () {
      OnMoveMenu?.Invoke();
   }
   public void GameMenu () {
      OnGameMenu?.Invoke();
   }
   public void PauseMenu () {
      OnPauseMenu?.Invoke();
   }
   public void GameOverMenu () {
      OnGameOverMenu?.Invoke();
   }
   public void CongratMenu () {
      OnCongratMenu?.Invoke();
   }
}
