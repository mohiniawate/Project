using ChatBotApp.Models;
using ChatBotApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ChatBotApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ProfessorController : ControllerBase
    {
        [HttpGet]

        public List<Professor> GetAllProfessor()
        {
            var Professorrepo = new Professorrepo();
            var Professorlist = Professorrepo.GetAllProfessor();

            return Professorlist;
        }

        [HttpGet("{department}")]
        public List<Professor> GetProfessorByDepartment(string department)
        {
            var ProfessorResponse = new List<Professor>();
            var Professorrepo = new Professorrepo();
            var ProfessorList = Professorrepo.getProfessorByDepartment(department);

            if (ProfessorList != null && ProfessorList.Any())
            {
                ProfessorResponse = ProfessorList;

            }
            else
            {

                ProfessorResponse = new List<Professor>();

            }

            return ProfessorResponse;
        }

    }

   


}



