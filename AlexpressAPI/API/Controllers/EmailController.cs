using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController:ControllerBase
    {
        //private readonly IMessageRepository _repo;

        //public EmailController(IMessageRepository repo) 
        //{
        //    _repo = repo;
        //}

        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult> SendSummaryEmail([FromBody] SendEmailDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    await _repo.SendSummaryEmail(dto);
        //    return Ok(new { Message = "We have sent details to your email about your order" });
        //}

        //[Authorize]
        //[HttpPost("support")]
        //public ActionResult SendSupportEmail([FromBody]SupportDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    _repo.SendSupportEmail(dto);
        //    return Ok(new {Message="Support email sent successfully"});
        //}
    }
}
