﻿using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels.Users;

public class CreateUserViewModel
{
    [Display(Name = "Имя")] public string UserName { get; set; }

    [EmailAddress(ErrorMessage = "Некорректный адрес")]
    public string Email { get; set; }

    [Display(Name = "Пароль")] public string Password { get; set; }

    [Display(Name = "Дата регистрации")]
    [DataType(DataType.Date)]
    public DateTime RegistrationDate { get; set; }

    [Display(Name = "Роль")] public string UserRole { get; set; }

    public CreateUserViewModel()
    {
        UserRole = "User";
        RegistrationDate = DateTime.Now.Date;
    }
}