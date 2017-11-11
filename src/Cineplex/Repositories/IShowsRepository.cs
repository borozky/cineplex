using Cineplex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.Repositories
{
    public interface IShowsRepository
    {
        IEnumerable<Show> All();
        Show Find(int Id);
        Show Create(Show show);
        Show Update(Show show);
        bool Delete(int ShowId);

    }
}
