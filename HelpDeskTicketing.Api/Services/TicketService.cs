// Plik: Services/TicketService.cs
using AutoMapper; // <-- Ważny using!
using HelpDeskTicketing.Api.Data;
using HelpDeskTicketing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskTicketing.Api.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; // <-- ZMIANA

        public TicketService(ApplicationDbContext context, IMapper mapper) // <-- ZMIANA
        {
            _context = context;
            _mapper = mapper; // <-- ZMIANA
        }

        public async Task<Ticket> CreateTicketAsync(CreateTicketDto ticketDto, string userId)
        {
            // Używamy AutoMappera do stworzenia obiektu Ticket
            var newTicket = _mapper.Map<Ticket>(ticketDto);

            // Uzupełniamy pola, których nie ma w DTO
            newTicket.OwnerId = userId;
            newTicket.Status = TicketStatus.New;
            newTicket.Priority = TicketPriority.Medium;
            newTicket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();
            return newTicket;
        }

        public async Task<bool> UpdateTicketAsync(int ticketId, UpdateTicketDto ticketDto, string userId)
        {
            var ticketInDb = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.OwnerId == userId);
            if (ticketInDb == null) return false;

            // AutoMapper aktualizuje istniejący obiekt ticketInDb danymi z ticketDto
            _mapper.Map(ticketDto, ticketInDb);

            ticketInDb.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Metody GetAll, GetById i Delete na razie pozostają bez zmian,
        // bo nie używają mapowania. Zmienimy je, gdy będziemy zwracać DTO.

        public async Task<IEnumerable<Ticket>> GetAllTicketsForUserAsync(string userId)
        {
            var tickets = await _context.Tickets
                // .Include(t => t.Owner) // Możesz to na razie wykomentować, żeby uprościć diagnozę
                .Where(t => t.OwnerId == userId)
                .ToListAsync();

            return tickets; // <-- USTAW PUNKT PRZERWANIA TUTAJ
        }

        public async Task<Ticket> GetTicketByIdForUserAsync(int ticketId, string userId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.OwnerId == userId);
        }

        public async Task<bool> DeleteTicketAsync(int ticketId, string userId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.OwnerId == userId);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}