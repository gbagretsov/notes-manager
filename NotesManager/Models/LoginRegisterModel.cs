using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesManager.Models
{
    public class LoginModel
    {
        [Required (ErrorMessage = "Необходимо ввести логин")]
        [Display (Name = "Логин")]
        public string Nickname { get; set; }

        [Required (ErrorMessage = "Необходимо ввести пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required (ErrorMessage = "Необходимо задать логин")]
        [Display(Name = "Логин (имя или ник)")]
        public string Nickname { get; set; }

        [Required (ErrorMessage = "Необходимо задать пароль")]
        [RegularExpression("^.{6,}$",
            ErrorMessage = "Длина пароля должна быть не менее 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required (ErrorMessage = "Необходимо повторно ввести пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        [Required (ErrorMessage = "Необходимо ввести адрес электронной почты")]
        [RegularExpression(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$", 
            ErrorMessage = "Введён некорректный адрес электронной почты")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Display(Name = "Уведомлять о событиях")]
        public bool Notify { get; set; }
    }

    public class SettingsModel
    {
        [Display(Name = "Логин (имя или ник)")]
        public string Nickname { get; set; }
       
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль (введите для смены пароля)")]
        public string OldPassword { get; set; }

        [RegularExpression("^.{6,}$",
            ErrorMessage = "Длина пароля должна быть не менее 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$",
            ErrorMessage = "Введён некорректный адрес электронной почты")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Display(Name = "Уведомлять о событиях")]
        public bool Notify { get; set; }
    }

}