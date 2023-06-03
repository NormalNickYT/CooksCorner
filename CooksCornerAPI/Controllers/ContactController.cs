namespace CooksCornerAPI.Controllers
{
    public class ContactController
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ContactController : ControllerBase
        {
            private readonly IContactServices _IContactService;
            private readonly IEmailService _emailService;


            public ContactController(IContactServices IContactService, IEmailService emailService)
            {
                _IContactService = IContactService;
                _emailService = emailService;
            }

            [HttpGet]
            public async Task<ActionResult<List<Contact>>> GetAllContacts()
            {

                return await _IContactService.GetAllContacts();

            }

            [HttpGet("{id}")]
            public async Task<ActionResult<List<Contact>>> GetSingleContact(int id)
            {
                var result = await _IContactService.GetSingleContact(id);
                if (result is null)
                {
                    return NotFound("Contact not found");
                }
                return Ok(result);

            }


            [HttpPost]
            public async Task<ActionResult<Contact>> AddContact(Contact contact)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _emailService.SendEmailAsync(contact.Email, contact.Subject, contact.Message);
                var emailSent = true;
                if (!emailSent)
                {
                    ModelState.AddModelError("", "An error occurred while sending the email. Please try again later.");
                    return BadRequest(ModelState);
                }
                var result = await _IContactService.AddContact(contact);
                return Ok(result);
            }
        }
    }
}
