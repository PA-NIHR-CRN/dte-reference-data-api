using System.Collections.Generic;

namespace Application.Models
{
    public class PaginationListModel<T> where T : class
    {
        public string PaginationToken { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}