using System.Collections.Generic;

namespace dialogbot.Models
{ 
    public class Professor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MobileNo { get; set; }

        public string EmailId { get; set; }

        public string Department { get; set; }

        public string Subject { get; set; }


    }

    public class ProfessorsData
    {
        public List<Professor> professorList{ get; set; }
    }
}
