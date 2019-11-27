using Proyecto26;
using FullSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    public Text scoreText;
    public static int playerScore;
    public static string playerName;

    public InputField getScoreText;

    public InputField emailText;
    public InputField usernameText;
    public InputField passwordText;

    User user = new User();

    private System.Random random = new System.Random();

    public static string localId;
    private string idToken;

    private string getLocalId;

    private string dataBaseURL = "https://fir-project-e825d.firebaseio.com/users";
    private string AuthKey = "AIzaSyBZ7Ch8_VvYZXTz_sJDa31rVAkhAMZjXFw";

    public fsSerializer serializer = new fsSerializer();


    void Start()
    {
        playerScore = random.Next(0, 100);
        scoreText.text = "Score:" + playerScore;
    }

    public void OnSubmit()
    {
        PostToDatabase();
    }

    public void OnRetrieve()
    {
        GetLocalId();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + user.userScore;
    }
    private void PostToDatabase(bool emptyScore = false)
    {
        User user = new User();

        if(emptyScore)
        {
            user.userScore = 0;

        }
        RestClient.Put(dataBaseURL + "/" + localId +".json", user);
    }

    private void RetrieveFromDatabase()
    {
    
        RestClient.Get<User>(dataBaseURL + getLocalId + ".json").Then(response => {
             user = response;
            UpdateScore();
        });
        
    }

    private void SignUpUser(string email,string username,string password)
    {
        string userData = "{\"email\":\""+ email +"\",\"password\":\""+ password +"\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            playerName = username;
            PostToDatabase(true);
        }).Catch(error => 
        {
            Debug.Log(error);
        });
    }

    public void SignUpUserButton()
    {
        SignUpUser(emailText.text,usernameText.text,passwordText.text);
    }

    private void SignInUser(string email,string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            getUsername();
        }).Catch(error =>
        {
            Debug.Log(error);
        });

    }

    public void SignInUserButton()
    {
        SignInUser(emailText.text,passwordText.text);
    }

    private void getUsername()
    {
        RestClient.Get<User>(dataBaseURL + "/" + localId + ".json").Then(response => {
           playerName = response.userName;
        });

    }

    private void GetLocalId()
    {
        RestClient.Get(dataBaseURL + ".json").Then(response => {

            var username = getScoreText.text;

            fsData userData = fsJsonParser.Parse(response.Text);

            Dictionary<string,User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach(var user in users.Values)
            {
                if(user.userName == username)
                {
                    getLocalId = user.localId;
                    RetrieveFromDatabase();
                    break;
                }
            }
        });
    }

    void Update()
    {
        
    }
}
