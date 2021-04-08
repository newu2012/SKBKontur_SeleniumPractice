using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace HomePractice {
    /*
     * Позитивные тесты:
     *  Письмо отправлено на верную почту
     *  Отправка при женском имени попуга
     *  После изменения email очистилось поле email и исчезла кнопка для изменения
     * Негативные тесты:
     *  Отправка пустого email
     *  Отправка нерабочего email
     *  Отправка очень длинного email
     *  Вставка SQL-инъекции в поле email
     *  Вставка скрипта в поле email
    */ 
    
    [TestFixture]
    public class ParrotNamePickTests {
        private ChromeDriver _driver;

        private const string       Url       = "https://qa-course.kontur.host/selenium-practice/";
        private const string       TestCorrectEmail = "kontur_course_hi@mail.ru";
        private const string       TestEmptyEmail = "";
        private const string       TestTooLongEmail = "kontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hikontur_course_hi@mail.ru";
        private const string       TestWrongFormatEmail = "kontur_course_hi@mail@sobaka.ru";
        private const string       TestWrongFormat2Email = "kontur_course_hi@mail_nesobaka.ru\"Hehe";
        private const string       TestSqlInjectionEmail = "a@mail.ru'); DROP TABLE Emails";
        private const string       TestScriptEmail = "a@a.a<script>alert(XXX)</script>";
        
        private static readonly By EmailInputLocator         = By.Name("email");
        private static readonly By BoyGenderRadioButton      = By.Id("boy");
        private static readonly By GirlGenderRadioButton     = By.Id("girl");
        private static readonly By SubmitButtonLocator       = By.Id("sendMe");
        private static readonly By YourEmailLocator          = By.ClassName("your-email");
        private static readonly By AnotherEmailButtonLocator = By.Id("anotherEmail");
        private static readonly By FormErrorLocator          = By.ClassName("form-error");
        

        [SetUp]
        public void SetUp() {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
        }
        
        [TearDown]
        public void TearDown() {
            _driver.Quit();
        }
        
        [Test]
        public void ParrotNamePick_SubmitEmail_Submits() {
            _driver.Navigate().GoToUrl(Url);
            var emailInput = _driver.FindElement(EmailInputLocator);
            var submitButton = _driver.FindElement(SubmitButtonLocator);

            emailInput.SendKeys(TestCorrectEmail);
            submitButton.Click();
            var emailResult = _driver.FindElement(YourEmailLocator);
            
            Assert.AreEqual(emailResult.Text, TestCorrectEmail, "Сделали заявку не на тот email");
        }
        
        [Test]
        public void ParrotNamePick_GirlGender_Submits() {
            _driver.Navigate().GoToUrl(Url);
            var emailInput = _driver.FindElement(EmailInputLocator);
            var submitButton = _driver.FindElement(SubmitButtonLocator);
            var girlRadioButton = _driver.FindElement(GirlGenderRadioButton);

            girlRadioButton.Click();
            emailInput.SendKeys(TestCorrectEmail);
            submitButton.Click();
            
            var emailResult = _driver.FindElement(YourEmailLocator);
            Assert.AreEqual(emailResult.Text, TestCorrectEmail, "Не отправляется форма при попуге-девочке");
        }
        
        [Test]
        public void ParrotNamePick_MultipleGenderChanges_Correct() {
            _driver.Navigate().GoToUrl(Url);
            var emailInput = _driver.FindElement(EmailInputLocator);
            var submitButton = _driver.FindElement(SubmitButtonLocator);
            var girlRadioButton = _driver.FindElement(GirlGenderRadioButton);
            var boyRadioButton = _driver.FindElement(BoyGenderRadioButton);

            boyRadioButton.Click();
            girlRadioButton.Click();
            boyRadioButton.Click();
            girlRadioButton.Click();
            emailInput.SendKeys(TestCorrectEmail);
            submitButton.Click();
            var emailResult = _driver.FindElement(YourEmailLocator);
            
            Assert.AreEqual(emailResult.Text, TestCorrectEmail, "Не отправляется форма при множественном переключении пола попуга");
        }
        
        [Test]
        public void ParrotNamePick_ClickAnotherEmail_EmailInputIsEmpty() {
            _driver.Navigate().GoToUrl(Url);
            
            var emailInput = _driver.FindElement(EmailInputLocator);
            var writeEmailButton = _driver.FindElement(SubmitButtonLocator);

            emailInput.SendKeys(TestCorrectEmail);
            writeEmailButton.Click();

            var anotherEmailButton = _driver.FindElement(AnotherEmailButtonLocator);
            anotherEmailButton.Click();
            
            Assert.AreEqual(string.Empty, emailInput.Text, "После клика по указанию другого email'а поле email не очистилось");
            Assert.IsFalse(anotherEmailButton.Displayed, "Не исчезла ссылка для ввода другого email'а");
        }
        
        // Негативные тесты

        [Test]
        public void ParrotNamePick_EmptyEmail_IsNotSent() {
            _driver.Navigate().GoToUrl(Url);
            
            var emailInput = _driver.FindElement(EmailInputLocator);
            var submitButton = _driver.FindElement(SubmitButtonLocator);

            emailInput.SendKeys(TestEmptyEmail);
            submitButton.Click();
            var formError = _driver.FindElement(FormErrorLocator);
            
            Assert.IsTrue(formError.Text.Contains("Введите email"), "Отправляется форма на пустой email");
        }
        
        [Test]
        [TestCase(TestTooLongEmail)]
        [TestCase(TestWrongFormatEmail)]
        [TestCase(TestWrongFormat2Email)]
        [TestCase(TestSqlInjectionEmail)]
        [TestCase(TestScriptEmail)]
        public void ParrotNamePick_IncorrectEmail_IsNotSent(string email) {
            _driver.Navigate().GoToUrl(Url);
            
            var emailInput = _driver.FindElement(EmailInputLocator);
            var submitButton = _driver.FindElement(SubmitButtonLocator);

            emailInput.SendKeys(email);
            submitButton.Click();
            var formError = _driver.FindElement(FormErrorLocator);
            
            Assert.IsTrue(formError.Text.Contains("Некорректный email"), "Отправляется форма на неверный email");
        }
    }
    
}