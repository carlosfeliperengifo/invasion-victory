using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour {
   // Controller scripts for each scene
   [SerializeField] private Main mainScript;
   [SerializeField] private User userScript;
   [SerializeField] private GameUI gameScript;
   [SerializeField] private EndGame endGameScript;
   // States of finite state machines
   private enum State {
      Login, Register, Recover, Consent, History,
      LoginDB, RegisterDB, UpdatePassDB,
      Ranking, Parameters, Credits, Manual, Movement,
      SaveParameters,
      Game, Playing, Pause,
      SavePerformanceTxt, Connection,
      InsertSessionDB, InsertMatchDB, InsertPerformanceDB,
      GetDataGame, GameOver, Congratulations, CloseApp
   };

   private State currentState;

   public static GlobalManager events;

   private void Awake () {
      if (events != null && events != this) {
         Destroy(gameObject);
      } else {
         events = this;
      }
   }
   // Load initial methods of each finite state machine
   private void Start () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
      }
      Debug.Log("Scene: " + SceneManager.GetActiveScene().name);
      switch (SceneManager.GetActiveScene().name) {
      case "00_Main":
         userScript = null;
         gameScript = null;
         endGameScript = null;
         currentState = State.Login;
         break;
      case "01_User":
         mainScript = null;
         gameScript = null;
         endGameScript = null;
         currentState = State.Ranking;
         break;
      case "02_Game":
         mainScript = null;
         userScript = null;
         endGameScript = null;
         currentState = State.Game;
         break;
      case "03_EndGame":
         mainScript = null;
         userScript = null;
         gameScript = null;
         currentState = State.Connection;
         break;
      }
      StateMachine();
   }

   /**********STATE MACHINE*********/
   private void StateMachine () {
      Debug.Log(currentState.ToString() + " activated");
      switch (currentState) {
      case State.CloseApp:
         Application.Quit();
         break;
      case State.Login:
         mainScript.Login();
         break;
      case State.Register:
         mainScript.Register();
         break;
      case State.Recover:
         mainScript.Recover();
         break;
      case State.Consent:
         mainScript.Consent();
         break;
      case State.LoginDB:
         mainScript.LoginDB();
         break;
      case State.RegisterDB:
         mainScript.RegisterDB();
         break;
      case State.UpdatePassDB:
         mainScript.UpdatePassDB();
         break;
      case State.History:
         mainScript.History();
         break;
      case State.Ranking:
         userScript.Ranking();
         break;
      case State.Parameters:
         userScript.Parameters();
         break;
      case State.SaveParameters:
         userScript.SaveParameters();
         break;
      case State.Credits:
         userScript.Credits();
         break;
      case State.Manual:
         userScript.Manual();
         break;
      case State.Movement:
         userScript.Movement();
         break;
      case State.Game:
         gameScript.Game();
         break;
      case State.Playing:
         gameScript.Playing();
         break;
      case State.Pause:
         gameScript.Pause();
         break;
      case State.SavePerformanceTxt:
         gameScript.SavePerformanceTxt();
         break;
      case State.Connection:
         StartCoroutine(endGameScript.Connection());
         break;
      case State.InsertSessionDB:
         StartCoroutine(endGameScript.InsertSessionDB());
         break;
      case State.InsertMatchDB:
         StartCoroutine(endGameScript.InsertMatchDB());
         break;
      case State.InsertPerformanceDB:
         StartCoroutine(endGameScript.InsertPerformanceDB());
         break;
      case State.GetDataGame:
         endGameScript.GetDataGame();
         break;
      case State.GameOver:
         endGameScript.GameOver();
         break;
      case State.Congratulations:
         endGameScript.Congratulations();
         break;
      }
   }

   /**********TRANSITIONS**********/
   public void bt_exit () {
      currentState = State.CloseApp;
      StateMachine();
   }
   public void bt_login2 () {
      currentState = State.Login;
      StateMachine();
   }
   public void bt_cancel1 () {
      currentState = State.Login;
      StateMachine();
   }
   public void success1 () {
      currentState = State.Login;
      StateMachine();
   }
   public void failed1 () {
      currentState = State.Login;
      StateMachine();
   }
   public void bt_Logout () {
      currentState = State.Login;
      SceneManager.LoadScene("00_Main");
      //StateMachine();
   }
   public void bt_registerNow () {
      currentState = State.Register;
      StateMachine();
   }
   public void bt_close1 () {
      currentState = State.Register;
      StateMachine();
   }
   public void failed2 () {
      currentState = State.Register;
      StateMachine();
   }
   public void bt_recover () {
      currentState = State.Recover;
      StateMachine();
   }
   public void failed3 () {
      currentState = State.Recover;
      StateMachine();
   }
   public void bt_consent () {
      currentState = State.Consent;
      StateMachine();
   }

   public void bt_login1 () {
      currentState = State.LoginDB;
      StateMachine();
   }
   public void bt_register () {
      currentState = State.RegisterDB;
      StateMachine();
   }
   public void bt_accept1 () {
      currentState = State.UpdatePassDB;
      StateMachine();
   }
   public void success2 () {
      currentState = State.History;
      StateMachine();
   }

   public void bt_skip1 () {
      currentState = State.Ranking;
      SceneManager.LoadScene("01_User");
      //StateMachine();
   }
   public void bt_cancel2 () {
      currentState = State.Ranking;
      StateMachine();
   }
   public void bt_close2 () {
      currentState = State.Ranking;
      StateMachine();
   }
   public void bt_home () {
      currentState = State.Ranking;
      SceneManager.LoadScene("01_User");
      //StateMachine();
   }
   public void bt_options () {
      currentState = State.Parameters;
      StateMachine();
   }
   public void bt_accept2 () {
      currentState = State.SaveParameters;
      StateMachine();
   }
   public void bt_credits () {
      currentState = State.Credits;
      StateMachine();
   }
   public void bt_manual () {
      currentState = State.Manual;
      StateMachine();
   }
   public void bt_play () {
      currentState = State.Movement;
      StateMachine();
   }

   public void bt_skip2 () {
      currentState = State.Game;
      SceneManager.LoadScene("02_Game");
      //StateMachine();
   }
   public void bt_retry () {
      currentState = State.Game;
      SceneManager.LoadScene("02_Game");
      //StateMachine();
   }
   public void time3s () {
      currentState = State.Playing;
      StateMachine();
   }
   public void bt_resume () {
      currentState = State.Playing;
      StateMachine();
   }
   public void bt_pause () {
      currentState = State.Pause;
      StateMachine();
   }
   public void finishgame () {
      currentState = State.SavePerformanceTxt;
      StateMachine();
   }
   public void performanceSaved () {
      currentState = State.Connection;
      SceneManager.LoadScene("03_EndGame");
      //StateMachine();
   }
   public void conn_ok () {
      currentState = State.InsertSessionDB;
      StateMachine();
   }
   public void failed4 () {
      currentState = State.Connection;
      StateMachine();
   }
   public void success3 () {
      currentState = State.InsertMatchDB;
      StateMachine();
   }
   public void success4 () {
      currentState = State.InsertPerformanceDB;
      StateMachine();
   }
   public void success5 () {
      currentState = State.GetDataGame;
      StateMachine();
   }
   public void completedGame (bool state) {
      if (state) {
         currentState = State.Congratulations;
      } else {
         currentState = State.GameOver;
      }
      StateMachine();
   }
}
