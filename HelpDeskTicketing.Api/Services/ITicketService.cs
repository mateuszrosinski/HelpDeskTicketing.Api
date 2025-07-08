using HelpDeskTicketing.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskTicketing.Api.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsForUserAsync(string userId);
        Task<Ticket> GetTicketByIdForUserAsync(int ticketId, string userId);
        Task<Ticket> CreateTicketAsync(CreateTicketDto ticketDto, string userId);
        Task<bool> UpdateTicketAsync(int ticketId, UpdateTicketDto ticketDto, string userId);
        Task<bool> DeleteTicketAsync(int ticketId, string userId);
    }
}
