using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            this.logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ChatRequestDto request)
        {
            logger.LogInformation("Create Session and Enqueue started");

            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.FirstMessage))
                return BadRequest("Invalid chat request");

            var result = await _chatService.CreateAndEnqueue(request);

            logger.LogInformation("Create Session and Enqueue finished");

            return Ok(result);
        }

        [HttpPost("Poll/{id}")]
        public async Task<IActionResult> Poll(Guid id)
        {
            logger.LogInformation("Poll Session started with id {@sessionId}", id);

            await _chatService.PollAsync(id);

            logger.LogInformation("Poll Session by id finished");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("get all Sessions started");

            var chats = await _chatService.GetAllSessionsAsync();

            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            logger.LogInformation("get Session by id started");

            var chat = await _chatService.GetByIdAsync(id);
            if (chat == null)
                return NotFound("Chat session not found.");

            return Ok(chat);
        }
    }
}
