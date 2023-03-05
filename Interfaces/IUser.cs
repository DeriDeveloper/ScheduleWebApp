namespace ScheduleWebApp.Interfaces
{
    public interface IUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Types.Enums.TypePerson TypePerson { get; set; }
        public Types.Enums.TypePrivilege TypePrivilege { get; set; }


        //student
        public Types.GroupInfo GroupInfo { get; set; }
        public int Subgroup { get; set; }



        //teacher
        public long TeacherId { get; set; }
        public string TeacherFullName { get; set; }
    }
}
