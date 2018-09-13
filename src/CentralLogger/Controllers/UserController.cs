using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralLogger.Hubs;
using CentralLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CentralLogger.Controllers {
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly EmailService email;
        private readonly CentralLoggerContext db;
        private readonly IHubContext<LogHub> hubContext;

        private readonly UserService userService;



        public UserController(CentralLoggerContext db, IHubContext<LogHub> hubContext, EmailService email, UserService userService) {
            this.db = db;
            this.hubContext = hubContext;
            this.email = email;
            this.userService = userService;
        }
        [HttpPost]
        public async Task<ActionResult> LoginRequest([FromBody]GetLoginRequest request, [FromServices] UserService userService) {

            var IsAuthorized = await userService.IsAuthorized(request.User, request.Pass);
            if (IsAuthorized) {
                if (request.User != null) {
                    //  base64 UTF8 (request.User:request.pass)
                    var account = $"{request.User}:{request.Pass}";
                    var accountBytes = System.Text.Encoding.UTF8.GetBytes(account);

                    var result = new { accessToken = Convert.ToBase64String(accountBytes) };
                    return Ok(result);
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        public ActionResult AddUser([FromBody]GetUsers data) {
            var userlist = db.Users.Where(x => x.User == data.Users).Select(x => x.User).FirstOrDefault();
            if (userlist != data.Users && data.Users != null) {
                userService.AddUser(data.Users, data.Password);
                return Ok();
            } else {
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult DeleteUser(string User) {
            var del = db.Users.Where(data => data.User == User).FirstOrDefault();
            if (del != null && User != "admin") {
                db.Users.Remove(del);
                db.SaveChanges();
                return Ok();
            } else
                return BadRequest();
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> ShowAllUser() {
            try {
                var showUsers = db.Users.Select(data => data.User).ToArray();
                return Ok(showUsers);
            } catch (Exception ex) {
                return StatusCode(500, ex);
            }
        }

    }
}