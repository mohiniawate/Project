using ChatBotApp.Models;
using ChatBotApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApp.Controllers
{
    [Route("api/[controller]")]

    [ApiController]

    public class LoginController : ControllerBase
    {
        [HttpPost(Name = "Login")]
        public LoginResponse Login(Login login)
        {   
            var loginResponse =new LoginResponse();
            var studentrepo = new studentrepo();
            var student = studentrepo.getstudent(login.Mobileno,login.Password);

            if(student != null && student.StudentName != null)
            {
                loginResponse.Message = "Login Successfull";
                loginResponse.MessageId = 1;
                loginResponse.student = student;
                
            }
            else
            {
                loginResponse.Message = "Invalid Login";
                loginResponse.MessageId =2 ;
                loginResponse.student = new Student();

            }

            return loginResponse;
        }

    }
}
