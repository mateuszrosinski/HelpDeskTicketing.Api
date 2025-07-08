// Plik: Mapping/MappingProfile.cs
using AutoMapper;
using HelpDeskTicketing.Api.Models;

namespace HelpDeskTicketing.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapowanie z DTO do modelu bazodanowego
            CreateMap<CreateTicketDto, Ticket>();
            CreateMap<UpdateTicketDto, Ticket>();

            // Mapowanie z modelu bazodanowego na DTO (do zwracania danych)
            // W przyszłości stworzymy TicketDto, na razie to przygotowanie
            // CreateMap<Ticket, TicketDto>(); 
        }
    }
}