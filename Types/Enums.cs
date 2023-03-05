using System;

namespace ScheduleWebApp.Types
{
    public class Enums
    {
        public enum DayOfWeekEng
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        public enum DayOfWeekRusShort : int
        {
            ПН = 0,
            ВТ = 1,
            СР = 2,
            ЧТ = 3,
            ПТ = 4,
            СБ = 5,
            ВС = 6
        }



        public enum CellScheduleType : int
        {
            common,
            numerator,
            denominator,
        }

        public enum CommandOperationBD : long
        {
            None,
            Add,
            Remove
        }

        public enum NotifyMessage : long
        {
            good,
            warning,
            bad
        }
        public enum TypeScheduleFrom : int
        {
            Student,
            Teacher
        }
        public enum TypePerson : int
        {
            student,
            teacher,
            notdefined
        }

        [Flags]
        public enum TypePrivilege : int
        {
            Ordinary,
            ScheduleEditor,
            Developer
        }

        public enum TypeDocument : int
        {
            Docx,
            Xlsx
        }

        public enum TypeSqlOperaion : int
        {
            None,
            Select,
            Update,
            Insert,
            Delete,

        }

        public enum Theme : int
        {
            LIGHT,
            DARK,
        }

        public enum TypeMonth : int
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum TypeChangesToCallScheduleForLocation : int
        {
            All = 0,
            Gagarino = 1,
            Shevchenko = 2
        }
    }
}
