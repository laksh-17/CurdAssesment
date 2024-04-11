using CurdAssesment.Db;
using CurdAssesment.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CurdAssesment.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]


    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ContactDbContext ContactDbContext;
        private IConfiguration _config;
        public ContactController(ContactDbContext exampleDbContext, IConfiguration configuration, ILogger<ContactController> logger)

        {
            this.ContactDbContext = exampleDbContext;
            _config = configuration;

            _logger = logger;   

        }

             private string GenrateToken()
        {
            _logger.LogInformation("Token is generated");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"],null,
                expires:DateTime.UtcNow.AddMinutes(10),signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        
        [HttpGet("authentication")]
        public IActionResult auth()
        {

            _logger.LogInformation("Authentication for token");
            IActionResult response = Unauthorized();
                var token = GenrateToken();
                response = Ok(new {token = token});
            return response;
        }

   

        [HttpGet("userdetails")]
        public async Task<IActionResult> GetContact()
        {
            _logger.LogInformation("get call for data");
            var contact = await ContactDbContext.Contacts.ToListAsync();
            return Ok(contact);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContact([FromBody]Contact cont)
        {
            _logger.LogInformation("post call update the new data");
            await ContactDbContext.Contacts.AddAsync(cont);
            await ContactDbContext.SaveChangesAsync();
            return Ok(cont);
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateContacts(int id ,[FromBody] Contact cont)
        {
            _logger.LogInformation("put call for update the existing data");
            var contact = await ContactDbContext.Contacts.FirstOrDefaultAsync(a => a.Id == id);
            if(contact != null)
            {
               contact.FirstName= cont.FirstName;
                contact.LastName= cont.LastName;
                contact.Email= cont.Email;
                contact.Address = cont.Address;
                contact.PostalCode = cont.PostalCode;
                contact.Country = cont.Country;
                contact.City = cont.City;
                contact.State = cont.State;
                contact.PhoneNumber = cont.PhoneNumber;
                await ContactDbContext.SaveChangesAsync();
                return Ok(cont);
            }
            else
            {
                return NotFound("contact not found");
            }
           
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteContacts(int id)
        {
            _logger.LogInformation("delete the existing data from database");
            var contact = await ContactDbContext.Contacts.FirstOrDefaultAsync(a => a.Id == id);
            if (contact != null)
            {
                ContactDbContext.Contacts.Remove(contact);
                await ContactDbContext.SaveChangesAsync();
                return Ok(contact);
            }
            else
            {
                return NotFound("contact not found");
            }

        }
    
}

}
