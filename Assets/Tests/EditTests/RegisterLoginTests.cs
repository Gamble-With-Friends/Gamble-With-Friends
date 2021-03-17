using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RegisterLoginTests
    {
        RegisterLoginScript registrationScript = new RegisterLoginScript();
        
        [Test]
        public void ValidateInput_DisplayNameEmpty()
        {
            string userName = "";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";
            bool overEighteen = true;

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, overEighteen);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_DisplayNameTooShort()
        {
            string userName = "aa";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_DisplayNameInvalid()
        {
            string userName = "new_u$er";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_EmptyEmail()
        {
            string userName = "new_user";
            string email = "";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_InvalidEmail()
        {
            string userName = "new_user";
            string email = "fake@email";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_EmptyPassword()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe!2";
            string confirmPassword = "Qwe!2";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_PasswordTooShort()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe!2";
            string confirmPassword = "Qwe!2";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_PasswordComplexityProblem()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe123";
            string confirmPassword = "Qwe123";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_ConfirmPasswordEmpty()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_ConfirmPasswordInvalid()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "Qwe123";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.False(valid);
        }

        [Test]
        public void ValidateInput_Valid()
        {
            string userName = "new_user";
            string email = "fake@email.com";
            string password = "Qwe!23";
            string confirmPassword = "Qwe!23";

            bool valid = registrationScript.ValidateInput(userName, email, password, confirmPassword, true);

            Assert.True(valid);
        }
    }
}
