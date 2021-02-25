using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterLoginScript : MonoBehaviour
{
    public CanvasGroup registrationForm;
    public CanvasGroup loginButtonGroup;

    private GameObject userNameError;
    private GameObject emailError;
    private GameObject passwordError;
    private GameObject confirmPasswordError;

    private readonly DataManager dataManager = new DataManager();

    public void ValidateUserInfo()
    {
        ClearErrorMessages();
        // Retrieve user input
        var userName = GameObject.Find("DisplayName/InputField/Text").GetComponent<Text>().text;
        var email = GameObject.Find("EmailInput/InputField/Text").GetComponent<Text>().text;
        var password = GameObject.Find("PasswordInput/InputField/Text").GetComponent<Text>().text;
        var confirmPassword = GameObject.Find("ConfirmPasswordInput/InputField/Text").GetComponent<Text>().text;

        // Clear old error messages
        ClearErrorMessages();

        // Validate new input / display error messages if any
        bool inputValid = ValidateInput(userName, email, password, confirmPassword);

        // If no errors found, save user to the database
        if (inputValid)
        {
            var userId = Guid.NewGuid();
            dataManager.AddUserSQL(userId.ToString(), email, userName, password, "2000/01/01");

            // CLOSE THE REGISTRATION FORM
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
            confirmPasswordError.GetComponent<Text>().text = "Password must be the same.";
            valid = false;
        }

        return valid;
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
        registrationForm.gameObject.SetActive(false);
    }

    public void OpenRegistrationForm()
    {
        registrationForm.gameObject.SetActive(true);
        loginButtonGroup.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
    }
}
