using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterLoginScript : MonoBehaviour
{
    public CanvasGroup registrationForm;
    public CanvasGroup loginButtonGroup;

    private GameObject userNameObject;
    private GameObject emailObject;
    private GameObject passwordObject;
    private GameObject confirmPasswordObject;
    
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
            if (timerCounter >= 50)
            {
                successMessage.GetComponent<Text>().text = "";
                displayRegistrationSuccessMessage = false;
                timerCounter = 0;
                CloseRegistrationForm();
            }
        }
    }

    public void RegisterUser()
    {
        ClearErrorMessages();

        InitializeInputObjects();        

        // Retrieve user input
        var userName = userNameObject.GetComponent<InputField>().text;
        var email = emailObject.GetComponent<InputField>().text;
        var password = passwordObject.GetComponent<InputField>().text;
        var confirmPassword = confirmPasswordObject.GetComponent<InputField>().text;

        // Clear old error messages
        ClearErrorMessages();

        // Validate new input / display error messages if any
        bool inputValid = ValidateInput(userName, email, password, confirmPassword);

        // If no errors found, save user to the database
        if (inputValid)
        {
            var userId = Guid.NewGuid();
            dataManager.AddUserSQL(userId.ToString(), email, userName, password, "2000/01/01");

            // Display success message and close the form in 3 secs
            successMessage = GameObject.Find("SuccessMessage");
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
        else if (dataManager.DisplayNameExistsSQL(userName))
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

        if (confirmPassword != password)
        {
            confirmPasswordError.GetComponent<Text>().text = "Passwords must match.";
            valid = false;
        }

        return valid;
    }

    private void InitializeInputObjects()
    {
        userNameObject = GameObject.Find("DisplayName/InputField");
        emailObject = GameObject.Find("EmailInput/InputField");
        passwordObject = GameObject.Find("PasswordInput/InputField");
        confirmPasswordObject = GameObject.Find("ConfirmPasswordInput/InputField");
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

    }

    private void ClearErrorMessages()
    {
        // instantiate ErrorMessage objects if not yet 
        if (userNameError == null)
        {
            userNameError = GameObject.Find("DisplayName/ErrorMessage");
            emailError = GameObject.Find("EmailInput/ErrorMessage");
            passwordError = GameObject.Find("PasswordInput/ErrorMessage");
            confirmPasswordError = GameObject.Find("ConfirmPasswordInput/ErrorMessage");
        }

        userNameError.GetComponent<Text>().text = string.Empty;
        emailError.GetComponent<Text>().text = string.Empty;
        passwordError.GetComponent<Text>().text = string.Empty;
        confirmPasswordError.GetComponent<Text>().text = string.Empty;
    }

    public void CloseRegistrationForm()
    {
        ClearInputObjects();
        registrationForm.gameObject.SetActive(false);
    }

    public void OpenRegistrationForm()
    {
        registrationForm.gameObject.SetActive(true);
        loginButtonGroup.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
    }
}
