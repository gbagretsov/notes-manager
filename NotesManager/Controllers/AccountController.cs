using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NotesManager.Models;

namespace NotesManager.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Поиск пользователя в БД
                User user = null;
                using (UsersNotesContext uc = new UsersNotesContext())
                    user = uc.Users.Where(u => u.Nickname == model.Nickname && u.Notapassword == model.Password).FirstOrDefault();
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Nickname, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Пользователь с таким логином и паролем не найден");
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (UsersNotesContext uc = new UsersNotesContext())
                {
                    user = uc.Users.Where(u => u.Nickname == model.Nickname).FirstOrDefault();
                }
                if (user == null)
                {
                    // Создаем нового пользователя
                    using (UsersNotesContext uc = new UsersNotesContext())
                    {
                        uc.Users.Add(new User { 
                            Nickname = model.Nickname, Notapassword = model.Password, Email = model.Email, Notify = model.Notify 
                        });
                        uc.SaveChanges();

                        user = uc.Users.Where(u => u.Nickname == model.Nickname && u.Notapassword == model.Password).FirstOrDefault();
                    }
                    // Если пользователь удачно добавлен в БД
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Nickname, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }

        public ActionResult Settings()
        {
            if (User.Identity.IsAuthenticated)
            {
                UsersNotesContext uc = new UsersNotesContext();
                User user = uc.Users.Where(u => u.Nickname == User.Identity.Name).Single();
                SettingsModel sm = new SettingsModel { Email = user.Email, Notify = user.Notify };
                return View(sm);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsModel model)
        {
            bool error = false;

            using (UsersNotesContext uc = new UsersNotesContext())
            {
                User curUser = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    // Изменяем ник
                    if (model.Nickname != null)
                        if (uc.Users.Where(u => u.Nickname == model.Nickname).FirstOrDefault() == null)
                        {
                            curUser.Nickname = model.Nickname;
                            FormsAuthentication.SetAuthCookie(model.Nickname, true);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                            error = true;
                        }

                    // Изменяем пароль
                    if (model.Password != null)
                    {
                        if (model.OldPassword == curUser.Notapassword)
                            if (model.ConfirmPassword == model.Password)
                                curUser.Notapassword = model.Password;
                            else
                            {
                                ModelState.AddModelError("", "Пароли не совпадают");
                                error = true;
                            }
                        else
                        {
                            ModelState.AddModelError("", "Старый пароль введён неправильно");
                            error = true;
                        }
                    }

                    // Изменяем электронную почту
                    if (model.Email != null)
                        curUser.Email = model.Email;

                    // Изменяем галочку
                    curUser.Notify = model.Notify;

                    uc.SaveChanges();
                }
                else
                    error = true;                        
            }

            if (error)
                return View(model);  
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}