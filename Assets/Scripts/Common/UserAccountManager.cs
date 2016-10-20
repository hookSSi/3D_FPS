using UnityEngine;
using System.Collections;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour 
{
    public static UserAccountManager singleton;

    void Awake()
    {
        if(singleton != null)
        {
            Destroy(this.gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(this);
    }

    public static string LoggedIn_Username {get; protected set;} //stores username once logged in
    public static string LoggedIn_Password = ""; //stores password once logged in

    public static string LoggedIn_Data { get; protected set; }

    public static bool IsLoggedIn { get; protected set; }

    public string loggedInSceneName = "Lobby";
    public string loggedOutSceneName = "Login";

    public void LogOut()
    {
        LoggedIn_Username = "";
        LoggedIn_Password = "";

        IsLoggedIn = false;

        Debug.Log("User logged out!");
    }

    public void LogIn(string p_username, string p_password)
    {
        LoggedIn_Username = p_username;
        LoggedIn_Password = p_password;

        IsLoggedIn = true;

        Debug.Log("User logged in as" + LoggedIn_Username);

        SceneManager.LoadScene(loggedInSceneName);
    }


    public void SendData(string p_data)
    { //called when the 'Send Data' button on the data part is pressed
        if (IsLoggedIn == true)
        {
            //ready to send request
            StartCoroutine(sendSendDataRequest(LoggedIn_Username, LoggedIn_Password, p_data)); //calls function to send: send data request
        }
    }

    IEnumerator sendSendDataRequest(string username, string password, string data)
    {
        IEnumerator eee = DC.SetUserData(username, password, data);
        while (eee.MoveNext())
        {
            yield return eee.Current;
        }
        WWW returneddd = eee.Current as WWW;
        if (returneddd.text == "ContainsUnsupportedSymbol")
        {
            //One of the parameters contained a - symbol
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        if (returneddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
            Debug.Log("Data Upload Error: Contains Unsupported Symbol '-'");
        }
    }

    public void GetData()
    { //called when the 'Get Data' button on the data part is pressed

        if (IsLoggedIn)
        {
            //ready to send request
            StartCoroutine(sendGetDataRequest(LoggedIn_Username, LoggedIn_Password)); //calls function to send get data request
        }
    }

    IEnumerator sendGetDataRequest(string username, string password)
    {
        string data = "ERROR";

        IEnumerator eeee = DC.GetUserData(username, password);
        while (eeee.MoveNext())
        {
            yield return eeee.Current;
        }
        WWW returnedddd = eeee.Current as WWW;
        if (returnedddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        else
        {
            if (returnedddd.text == "ContainsUnsupportedSymbol")
            {
                //One of the parameters contained a - symbol
                Debug.Log("Get Data Error: Contains Unsupported Symbol '-'");
            }
            else
            {
                //Data received in returned.text variable
                string DataRecieved = returnedddd.text;
                data = DataRecieved;
            }
        }

        LoggedIn_Data = data;
    }
}
