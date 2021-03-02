using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterLoginScript : MonoBehaviour
{
    public CanvasGroup registrationForm;
    public CanvasGroup loginForm;
    public CanvasGroup loginButtonGroup;
    public GameObject loginRegistrationGroup;

    #region Registration Scripts
    private GameObject userNameObject;
    private GameObject emailObject;
    private GameObject passwordObject;
    private GameObject confirmPasswordObject;
    private Toggle overEighteenOld;
    
    private GameObject userNameError;
    private GameObject emailError;
    private GameObject passwordError;
    private GameObject confirmPasswordError;

    private GameObject successMessage;

    private readonly DataManager dataManager = new DataManager();

    private bool displayRegistrationSuccessMessage;
    private int timerCounter;

    public void Update()
    {
        if (displayRegistrationSuccessMessage)
        {
            timerCounter++;
            if (timerCounter >= 150)
            {
                successMessage.GetComponent<Text>().text = "";
                displayRegistrationSuccessMessage = false;
                GameObject.Find("LoginTrigger").GetComponent<LoginTriggerScript>().ExitRegistrationLogin();
                timerCounter = 0;
            }
        }
    }

    public void RegisterUser()
    {
        ClearMessages();

        InitializeInputObjects();        

        // Retrieve user input
        var userName = userNameObject.GetComponent<InputField>().text;
        var email = emailObject.GetComponent<InputField>().text;
        var password = passwordObject.GetComponent<InputField>().text;
        var confirmPassword = confirmPasswordObject.GetComponent<InputField>().text;

        // Clear old error messages
        ClearMessages();

        // Validate new input / display error messages if any
        bool inputValid = ValidateInput(userName, email, password, confirmPassword);

        // If no errors found, save user to the database
        if (inputValid)
        {
            dataManager.AddUser(email, userName, password);

            // TODO: Complete the login process!!!

            // Display success message and close the form in 3 secs
            successMessage.GetComponent<Text>().text = "Registration completed successfully";
            timerCounter = 0;
            displayRegistrationSuccessMessage = true;
        }
    }

    private bool ValidateInput(string userName, string email, string password, string confirmPassword)
    {
        bool valid = true;

        // Validate user name
        Regex userNameRegex = new Regex(@"^[\w\s-]{3,}$");
        if (string.IsNullOrEmpty(userName))
        {
            userNameError.GetComponent<Text>().text = "This field is required.";
            valid = false;
        }
        else if (userName.Length < 3)
        {
            userNameError.GetComponent<Text>().text = "Display name must be at least 3-character long.";
            valid = false;
        }
        else if (!userNameRegex.IsMatch(userName))
        {
            userNameError.GetComponent<Text>().text = "Only letters, digits, dashes, underscores, and spaces allowed.";
            valid = false;
        }
        else if (dataManager.DisplayNameExists(userName))
        {
            userNameError.GetComponent<Text>().text = "This display name is already taken.";
            valid = false;
        }

        // Validate email address
        Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (string.IsNullOrEmpty(email))
        {
            emailError.GetComponent<Text>().text = "This field is required.";
            valid = false;
        }
        else if (!emailRegex.IsMatch(email))
        {
            emailError.GetComponent<Text>().text = "Please enter a valid email address.";
            valid = false;
        }

        // Validate password
        Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$");
        if (string.IsNullOrEmpty(password))
        {
            passwordError.GetComponent<Text>().text = "This field is required.";
            valid = false;
        }
        else if (password.Length < 6)
        {
            passwordError.GetComponent<Text>().text = "Password must be at least 6-character long.";
            valid = false;
        }
        else if (!passwordRegex.IsMatch(password))
        {
            passwordError.GetComponent<Text>().text = "Must contain at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character.";
            valid = false;
        }

        // Validate passeword confirmation
        if (confirmPassword != password)
        {
            confirmPasswordError.GetComponent<Text>().text = "Passwords must match.";
            valid = false;
        }

        // Check if ove 18 years old
        if (!overEighteenOld.isOn)
        {
            successMessage.GetComponent<Text>().text = "You must be over 18 years old to register.";
            valid = false;
        }

        return valid;
    }

    private void InitializeInputObjects()
    {
        userNameObject = GameObject.Find("RegisterDisplayName/InputField");
        emailObject = GameObject.Find("RegisterEmailInput/InputField");
        passwordObject = GameObject.Find("RegisterPasswordInput/InputField");
        confirmPasswordObject = GameObject.Find("ConfirmPasswordInput/InputField");
        overEighteenOld = GameObject.Find("OverEighteenToggle").GetComponent<Toggle>();
        successMessage = GameObject.Find("RegisterSuccessMessage");
    }

    private void ClearInputObjects()
    {
        if (userNameObject == null)
        {
            InitializeInputObjects();
        }
        userNameObject.GetComponent<InputField>().text = string.Empty;
        emailObject.GetComponent<InputField>().text = string.Empty;
        passwordObject.GetComponent<InputField>().text = string.Empty;
        confirmPasswordObject.GetComponent<InputField>().text = string.Empty;
        overEighteenOld.isOn = false;
    }

    private void ClearMessages()
    {
        // instantiate ErrorMessage objects if not yet 
        if (userNameError == null)
        {
            userNameError = GameObject.Find("RegisterDisplayName/ErrorMessage");
            emailError = GameObject.Find("RegisterEmailInput/ErrorMessage");
            passwordError = GameObject.Find("RegisterPasswordInput/ErrorMessage");
            confirmPasswordError = GameObject.Find("ConfirmPasswordInput/ErrorMessage");
            successMessage = GameObject.Find("RegisterSuccessMessage");
        }

        userNameError.GetComponent<Text>().text = string.Empty;
        emailError.GetComponent<Text>().text = string.Empty;
        passwordError.GetComponent<Text>().text = string.Empty;
        confirmPasswordError.GetComponent<Text>().text = string.Empty;
        successMessage.GetComponent<Text>().text = string.Empty;
    }    

    public void OpenRegistrationForm()
    {
        registrationForm.gameObject.SetActive(true);
        ClearInputObjects();
        ClearMessages();
        loginButtonGroup.gameObject.SetActive(false);
    }
    #endregion

    #region Login Scripts
    private GameObject loginUserNameObject;
    private GameObject loginPasswordObject;
    private GameObject loginMessageObject;
    public void LogIn()
    {
        var userName = loginUserNameObject.GetComponent<InputField>().text;
        var password = loginPasswordObject.GetComponent<InputField>().text;
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            loginMessageObject.GetComponent<Text>().text = "Both display name and password are required.";
        }
        else if (!dataManager.LoginUser(userName, password))
        {
            loginMessageObject.GetComponent<Text>().text = "Invalid display name or password.";
        }
        else
        {
            // TODO: Complete the login process!!!
            GameObject.Find("LoginTrigger").GetComponent<LoginTriggerScript>().ExitRegistrationLogin();
        }
    }

    public void InitializeLoginInputs()
    {
        if (loginPasswordObject == null)
        {
            loginUserNameObject = GameObject.Find("LoginDisplayName/InputField");
            loginPasswordObject = GameObject.Find("LoginPasswordInput/InputField");
            loginMessageObject = GameObject.Find("LoginSuccessMessage");
        }
        loginUserNameObject.GetComponent<InputField>().text = string.Empty;
        loginPasswordObject.GetComponent<InputField>().text = string.Empty;
        loginMessageObject.GetComponent<Text>().text = string.Empty;
    }   

    public void OpenLoginForm()
    {        
        loginForm.gameObject.SetActive(true);
        InitializeLoginInputs();
        loginButtonGroup.gameObject.SetActive(false);
    }
    #endregion

    public void CloseRegistrationLoginForm()
    {
        GameObject.Find("LoginTrigger").GetComponent<LoginTriggerScript>().ExitRegistrationLogin();
    }
}
