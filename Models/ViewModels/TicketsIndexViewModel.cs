using System.Collections.Generic;
using System.Web.Mvc;

namespace HelpDesk.Web.Models.ViewModels
{
    public class TicketsIndexViewModel
    {
        public IList<TicketListItemViewModel> Tickets { get; set; }

        public IList<SelectListItem> Estados { get; set; }

        public string Buscar { get; set; }

        public string Estado { get; set; }
    }
}
