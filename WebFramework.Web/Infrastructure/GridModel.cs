using System.Linq;

namespace Web.Infrastructure
{
    public class GridModel<T>
    {
        public IQueryable<T> Items { get; set; }
        public int TotalNumber { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }

    }
}