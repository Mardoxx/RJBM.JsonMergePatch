using Microsoft.AspNetCore.Mvc;
using RJBM.JsonMergePatch.Web.Models;

namespace RJBM.JsonMergePatch.Web.Controllers
{
    public class DemoController : Controller
    {
        [HttpPatch]
        public IActionResult TestPatch([FromBody] JsonMergePatchDocument<UserUpdateModel> patch)
        {
            User user = new User
            {
                Id = 1,
                Age = 20,
                GivenName = "First",
                Surname = "Last"
            };

            patch?.ApplyTo(user);

            return Ok(user);
        }
    }
}
