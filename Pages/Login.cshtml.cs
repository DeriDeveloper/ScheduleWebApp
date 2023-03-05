using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScheduleWebApp.Services;
using ScheduleWebApp.Types;

namespace ScheduleWebApp.Pages
{
    public class LoginModel : PageModel
    {
        public List<DeriLibrary.Web.Types.NotifyMessage> NotifyMessages { get; set; } = new List<DeriLibrary.Web.Types.NotifyMessage>();

        public LibrarySchedule.Models.User? UserInfo { get; set; }



        public IActionResult OnGet()
        {
            UserInfo = LibrarySchedule.Services.BackgroundWorker.CheckUserForAuthorization(HttpContext.Request.Cookies["user_id"]);



            if (UserInfo != null)
            {
                return Redirect(BackgroundWorker.GetUrlRedirectSchedule(UserInfo.Id));
            }

            return Page();
        }

        public IActionResult OnPostAuthorizationStudent(string login, string password)
        {
            //string connectionRemoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var passwordCripty = DeriLibrary.EncriptionAndDecription.EncriptionText(password, login);


            LibrarySchedule.Models.StudentAccount student = LibrarySchedule.Services.DateBase.Worker.GetStudent(login, passwordCripty);

            if (student != null)
            {
                if (student.Id != 0)
                {
                    BackgroundWorker.UpdateCookieDefault(HttpContext.Response.Cookies, userId: student.Id);
                    var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Good, $"С возвращением, {student.Name}");
                    NotifyMessages.Add(notifyMessage);


                    return Redirect($"/Schedule?group_id={student.GroupId}");
                }
                else
                {
                    var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Неверный логин или пароль");
                    NotifyMessages.Add(notifyMessage);

                }
            }
            else
            {
                //не понятно почему не найден student
            }

            return Page();
        }

        public IActionResult OnPostAuthorizationTeacher(string login, string password)
        {
            var passwordCripty = DeriLibrary.EncriptionAndDecription.EncriptionText(password, login);


            var teacher = LibrarySchedule.Services.BackgroundWorker.GetTeacherAccountByLoginAndPassword(login, passwordCripty);
            if (teacher?.Id != 0)
            {
                HttpContext.Response.Cookies.Append("user_id", teacher.Id.ToString());
                var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Good, $"С возвращением, {teacher.Name}");
                NotifyMessages.Add(notifyMessage);


                return Redirect($"/Schedule?teacher_id={teacher.Id}");
            }
            else
            {
                var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Неверный логин или пароль");
                NotifyMessages.Add(notifyMessage);

            }

            return Page();
        }


        public IActionResult OnPostRegistrationStudent(string login, string password, string password_replay, int group_id, int subgroup)
        {
            if (LibrarySchedule.Services.BackgroundWorker.IsLoginFreeStudent(login))
            {
                if (password.Equals(password_replay))
                {
                    // регестрируем нового студента

                    var passwordCripty = DeriLibrary.EncriptionAndDecription.EncriptionText(password, login);

                    var student = LibrarySchedule.Services.BackgroundWorker.AddNewStudent(login, passwordCripty, group_id, subgroup);

                    if (student != null)
                    {
                        if (student.Id != 0)
                        {
                            HttpContext.Response.Cookies.Append("user_id", student.Id.ToString());
                            var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Good, $"Привет, новый пользователь");
                            NotifyMessages.Add(notifyMessage);
                            return Redirect($"/Schedule?group_id={student.Id}");

                        }
                    }
                    else
                    {
                        //произошла ошибка на этапе регестрации, попробуйте еще раз
                        var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Произошла неизвестная ошибка, попробуйтее еще раз");
                        NotifyMessages.Add(notifyMessage);
                    }
                }
                else
                {
                    var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Пароли не совпадают");
                    NotifyMessages.Add(notifyMessage);
                }

            }
            else
            {
                var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Студент с таким логином уже занят");
                NotifyMessages.Add(notifyMessage);
            }


            return Page();
        }

        public IActionResult OnPostRegistrationTeacher(string login, string password, string password_replay, long teacher_id)
        {
            /*if (Services.BackgroundWorker.IsLoginFree(login))
            {
                if (password.Equals(password_replay))
                {
                    // регестрируем нового преподавателя

                    var passwordCripty = DeriLibrary.EncriptionAndDecription.EncriptionText(password, login);


                    if (Services.BackgroundWorker.AddNewTeacher(login, passwordCripty, teacher_id))
                    {
                        var teacher = LibrarySchedule.Services.BackgroundWorker.GetTeacherAccountByLoginAndPassword(login, passwordCripty);
                        if (teacher.Id != 0)
                        {
                            HttpContext.Response.Cookies.Append("user_id", teacher.Id.ToString());
                            var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Good, $"Привет, новый пользователь");
                            NotifyMessages.Add(notifyMessage);
                            return Redirect($"/Schedule?teacher_id={teacher.Id}");
                        }
                    }
                    else
                    {
                        //произошла ошибка на этапе регестрации, попробуйте еще раз
                        var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Произошла неизвестная ошибка, попробуйтее еще раз");
                        NotifyMessages.Add(notifyMessage);
                    }
                }
                else
                {
                    var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Пароли не совпадают");
                    NotifyMessages.Add(notifyMessage);
                }

            }
            else
            {
                var notifyMessage = new DeriLibrary.Web.Types.NotifyMessage(DeriLibrary.Web.Types.NotifyMessage.TypeNotify.Bad, "Преподаватель с таким логином уже занят");
                NotifyMessages.Add(notifyMessage);
            }

            */
            return Page();
        }
    }
}
