using LibrarySchedule.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWebApp.Services
{
    public class HtmlWorker
    {
        /*public static string GetHtmlContainerDayOfWeeksCellsSchedule(Dictionary<DateTime, List<LibrarySchedule.Models.CellSchedule>> dayOfWeeksCellsSchedule, ViewDataDictionary viewData)
        {
            string themeName = (string)viewData["ThemeName"] != null ? (string)viewData["ThemeName"] : "light";
          
            
            StringBuilder sb = new StringBuilder();

            int dayOfWeekIndex = 0;

             var dateNow = DateTime.Now;

            foreach (var keyValuePair in dayOfWeeksCellsSchedule)
            {
                var date = keyValuePair.Key;
                var cellsSchedule = keyValuePair.Value;

                sb.AppendLine($"<div class=\"schedule-cells\" id=\"ScheduleDay_{dayOfWeekIndex}\" style=\"{(dateNow == date ? "display: block;" : "display: none;")}\">");

                sb.AppendLine(GetHtmlContainerDayOfWeekCellsSchedule( cellsSchedule, viewData));

                sb.AppendLine("</div>");

                dayOfWeekIndex++;
            }

            return sb.ToString();
        }*/

        public static string GetHtmlContainerDayOfWeekCellsSchedule(LibrarySchedule.Models.CellSchedule[]? cellsSchedule, ViewDataDictionary viewData)
        {
            string themeName = (string)viewData["ThemeName"] != null ? (string)viewData["ThemeName"] : "light";


            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<div class=\"schedule-cells\">");

            if (cellsSchedule != null)
            {
                foreach (var cellSchedule in cellsSchedule)
                {
                    sb.AppendLine(GetHtmlContainerCellSchedule(cellSchedule, themeName));
                }
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public static string GetHtmlContainerCellSchedule(LibrarySchedule.Models.CellSchedule cellSchedule, string themeName)
        {
            StringBuilder sb = new StringBuilder();

            /*
             * <div class="cell-main-container-v2 cell-main-container-v2-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light") @((long)Model.CurrentDayOfWeekRus == ScheduleDayIndex ? (Services.BackgroundWorker.IsTimeIncludedInTimeRange(DateTime.Now, cellSchedules.TimesPair.TimeStart, cellSchedules.TimesPair.TimeEnd) ? "cell-main-container-active": "" ) : "")">
                    @if (Program.MainConfigJson.DeveloperSettings.CellSchedule.ShowCellId)
                    {
                        <p class="text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")">Id Cell: @cellSchedules.Id</p>
                    }
                    @if (Program.MainConfigJson.DeveloperSettings.CellSchedule.ShowCellType)
                    {
                        <p class="text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")">Type Cell: @cellSchedules.TypeCell.ToString()</p>
                    }
                    <div class="cell-main-container-special-info-v2-main-container">
                        @if (cellSchedules.IsChange)
                        {
                            <div class="cell-main-container-special-info-v2-container">
                                <img src="/images/rotate24.png" width="25" alt="Изменение" title="Изменение"/>
                            </div>
                        }
                        @if (cellSchedules.TimesPair.IsChange)
                        {
                            <div class="cell-main-container-special-info-v2-container">
                                <img src="/images/clock.png" width="25" alt="Время изменено" title="Время изменено"/>
                            </div>
                        }
                    </div>
                    <div class="cell-main-container-top-container-v2">
                        <div>
                            <p  style="line-break: anywhere;"  class="text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light") @(cellSchedules.TimesPair.IsChange ? "text-color-time-change": "")" title="@cellSchedules.TimesPair.TimeStart.ToString("HH:mm") - @cellSchedules.TimesPair.TimeEnd.ToString("HH:mm")">@cellSchedules.TimesPair.TimeStart.ToString("HH:mm") - @cellSchedules.TimesPair.TimeEnd.ToString("HH:mm")</p>
                        </div>
                        <div>
                            <p  style="line-break: anywhere;" class="text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")" title="@(string.Join(", ", cellSchedules.Audiences))">@(string.Join(", ", cellSchedules.Audiences))</p>
                        </div>
                    </div>
                    <div class="cell-main-container-bootom-container-v2">
                        <div>
                            <p style="line-break: anywhere;" class="text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")" title="@(string.Join(" / ", cellSchedules.AcademicSubjects))">@(cellSchedules.NumberPair). @(string.Join(" / ", cellSchedules.AcademicSubjects))</p>
                        </div>
                        <div style="margin-top: 6px; display: flex; flex-direction: column;">
                            @if (Model.TypePerson == Types.Enums.TypePerson.student)
                            {
                                @foreach (var Teacher in cellSchedules.Teachers)
                                {
                                    // <a href="/Schedule?teacher_id=@Teacher.Id" style="line-break: anywhere;" class="link-text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")" title="@Teacher.FullName">@Teacher.FullName</a>
                                }
                            }
                            else if (Model.TypePerson == Types.Enums.TypePerson.teacher)
                            {
                                /2*if (cellSchedules.GroupId > 0)
                                {
                                    <a href="/Schedule?group_id=@cellSchedules.GroupId" style="line-break: anywhere;" class="link-text-theme-@(ViewData["ThemeName"]  != null ? ViewData["ThemeName"] : "light")" title="@(Services.WorkerDB.GetNameGroupById(cellSchedules.GroupId))">@(Services.WorkerDB.GetNameGroupById(cellSchedules.GroupId))</a>
                                }*2/
                            }
                        </div>
                    </div>
                </div>
             */


            sb.AppendLine($"<div class=\"cell-main-container-v2 cell-main-container-v2-theme-{themeName}\">");
            
            sb.AppendLine("<div class=\"cell-main-container-special-info-v2-main-container\">");

            //тут пусто для знаков изменения и тд

            sb.AppendLine($"</div>");

            sb.AppendLine("<div class=\"cell-main-container-top-container-v2\">");

            sb.AppendLine("<div>");

            sb.AppendLine($"<p style=\"line-break: anywhere;\"  class=\"text-theme-{themeName} title=\"{cellSchedule.TimesPair.TimeStart.ToString("HH:mm")} - {cellSchedule.TimesPair.TimeEnd.ToString("HH:mm")}\">{cellSchedule.TimesPair.TimeStart.ToString("HH:mm")} - {cellSchedule.TimesPair.TimeEnd.ToString("HH:mm")}</p>");
            
            sb.AppendLine("</div>");

            sb.AppendLine("<div style=\"display: flex;\">");

            foreach (var audience in cellSchedule.Audiences)
            {
                sb.AppendLine($"<p class=\"text-theme-{themeName}\" style=\"margin-left:5px;\" title=\"{audience.Name}\">{audience.Name},</p>");
            }

            sb.AppendLine("</div>");

            sb.AppendLine($"</div>");
            
            
            sb.AppendLine("<div class=\"cell-main-container-bootom-container-v2\">");

            sb.AppendLine("<div style=\"display:flex;\">");
            
            sb.AppendLine($"<p class=\"text-theme-{themeName}\" title=\"{cellSchedule.NumberPair}\">{cellSchedule.NumberPair}.</p>");

            sb.AppendLine("<div style=\"margin-left: 10px;\">");

            sb.AppendLine("<div>");
            
            foreach (var academicSubject in cellSchedule.AcademicSubjects)
            {
                sb.AppendLine($"<p style=\"line-break: auto;\" class=\"text-theme-{themeName}\" title=\"{academicSubject.Name}\">{academicSubject.Name}</p>");
            }

            sb.AppendLine("</div>");

            

            sb.AppendLine("<div style=\"margin-top: 6px; display: flex; flex-direction: column;\">");
            

            foreach (var teacher in cellSchedule.Teachers)
            {
                sb.AppendLine($"<a  href=\"/Schedule?teacher_id={teacher.Id}\" style=\"line-break: auto;\" class=\"text-theme-{themeName}\" title=\"{teacher.GetFullName()}\">{teacher.GetNameInitials()}</a>");
            }

            sb.AppendLine("</div>");

            sb.AppendLine("</div>");

            sb.AppendLine("</div>");

            sb.AppendLine($"</div>");

            
            
            
            
            
            
            sb.AppendLine($"</div>");


            return sb.ToString();
        }

    }
}
