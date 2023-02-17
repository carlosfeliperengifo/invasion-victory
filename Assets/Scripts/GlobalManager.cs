using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour {
   private enum State {
      Login, Register, Recover, Consent, History,
      LoginDB, RegisterDB, UpdatePassDB,
      Ranking, Parameters, Credits, Manual, Movement,
      SaveMatchTxt,
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

   private void Start () {
      if (Time.timeScale == 0) {
         Time.timeScale = 1;
      }
      Debug.Log("Scene: " + SceneManager.GetActiveScene().name);
      switch (SceneManager.GetActiveScene().name) {
      case "00_Main":
         currentState = State.Login;
         break;
      case "01_User":
         currentState = State.Ranking;
         break;
      case "02_Game":
         currentState = State.Game;
         break;
      case "03_EndGame":
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
         Main.States.Login();
         break;
      case State.Register:
         Main.States.Register();
         break;
      case State.Recover:
         Main.States.Recover();
         break;
      case State.Consent:
         Main.States.Consent();
         break;
      case State.LoginDB:
         Main.States.LoginDB();
         break;
      case State.RegisterDB:
         Main.States.RegisterDB();
         break;
      case State.UpdatePassDB:
         Main.States.UpdatePassDB();
         break;
      case State.History:
         Main.States.History();
         break;
      case State.Ranking:
         User.States.Ranking();
         break;
      case State.Parameters:
         User.States.Parameters();
         break;
      case State.SaveMatchTxt:
         User.States.SaveMatchTxt();
         break;
      case State.Credits:
         User.States.Credits();
         break;
      case State.Manual:
         User.States.Manual();
         break;
      case State.Movement:
         User.States.Movement();
         break;
      case State.Game:
         GameUI.States.Game();
         break;
      case State.Playing:
         GameUI.States.Playing();
         break;
      case State.Pause:
         GameUI.States.Pause();
         break;
      case State.SavePerformanceTxt:
         GameUI.States.SavePerformanceTxt();
         break;
      case State.Connection:
         StartCoroutine(EndGame.States.Connection());
         break;
      case State.InsertSessionDB:
         StartCoroutine(EndGame.States.InsertSessionDB());
         break;
      case State.InsertMatchDB:
         StartCoroutine(EndGame.States.InsertMatchDB());
         break;
      case State.InsertPerformanceDB:
         StartCoroutine(EndGame.States.InsertPerformanceDB());
         break;
      case State.GetDataGame:
         EndGame.States.GetDataGame();
         break;
      case State.GameOver:
         EndGame.States.GameOver();
         break;
      case State.Congratulations:
         EndGame.States.Congratulations();
         break;
      }
   }

   /**********TRANSITIONS**********/
   public void bt_exit () {
      currentState = State.CloseApp;
      StateMachine();
   }
   public void bt_login2 () {
      LoginUserDB.instance.CleanAllDatas();
      currentState = State.Login;
      StateMachine();
   }
   public void bt_cancel1 () {
      LoginUserDB.instance.CleanAllDatas();
      currentState = State.Login;
      StateMachine();
   }
   public void success1 () {
      LoginUserDB.instance.CleanAllDatas();
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
      StartCoroutine(RegisterUserDB.instance.GetQuestions());
      RegisterUserDB.instance.CleanAllDatas();
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
      StartCoroutine(RecoverDB.instance.GetQuestions());
      RecoverDB.instance.CleanAllDatas();
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
      currentState = State.SaveMatchTxt;
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
