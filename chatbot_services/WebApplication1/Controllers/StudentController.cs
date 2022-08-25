using ChatBotApp.Models;
using ChatBotApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ChatBotApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [HttpGet(Name = "GetAllStudent")]
        public List<Student> GetAllStudent()
        {
           var studentrepo = new studentrepo();
           var studentlist= studentrepo.GetAllStudent();

            return studentlist;
        }
    }

}



