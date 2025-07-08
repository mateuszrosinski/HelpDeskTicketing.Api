using HelpDeskTicketing.Api.Data;
using HelpDeskTicketing.Api.Models;
using HelpDeskTicketing.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskTicketing.Api.Services;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService; // <-- ZMIANA

    public TicketsController(ITicketService ticketService) // <-- ZMIANA
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tickets = await _ticketService.GetAllTicketsForUserAsync(userId);
        return Ok(tickets); // W przyszłości zmapujemy to na DTO
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ticket = await _ticketService.GetTicketByIdForUserAsync(id, userId);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> PostTicket(CreateTicketDto createTicketDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var newTicket = await _ticketService.CreateTicketAsync(createTicketDto, userId);
        return CreatedAtAction(nameof(GetTicket), new { id = newTicket.Id }, newTicket);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTicket(int id, UpdateTicketDto updateTicketDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = await _ticketService.UpdateTicketAsync(id, updateTicketDto, userId);
        if (!success) return NotFound(); // NotFound, aby nie ujawniać istnienia zasobu
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = await _ticketService.DeleteTicketAsync(id, userId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}