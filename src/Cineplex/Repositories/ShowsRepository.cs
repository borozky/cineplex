using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cineplex.Models;
using Cineplex.Data;
using Microsoft.EntityFrameworkCore;

namespace Cineplex.Repositories
{
    public class ShowsRepository : IShowsRepository
    {
        private ApplicationDbContext _db;

        public ShowsRepository(ApplicationDbContext context)
        {
            _db = context;
        }
        public IEnumerable<Show> All()
        {
            // return shows that has yet to happen
            return _db.Shows
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Where(s => s.SessionTime > DateTime.Now)
                .ToList();
        }

        public Show Create(Show show)
        {
            var found = _db.Shows.FirstOrDefault(s => s.Id == show.Id);
            if (found == null)
            {
                _db.Shows.Add(show);
                _db.SaveChanges();
                return show;
            }
            return null;
        }

        public bool Delete(int ShowId)
        {
            var show = _db.Shows.FirstOrDefault(s => s.Id == ShowId);
            if (show != null)
            {
                _db.Shows.Remove(show);
                _db.SaveChanges();
            }
            return (_db.Shows.FirstOrDefault(s => s.Id == ShowId) == null);

        }

        public Show Find(int Id)
        {
            throw new NotImplementedException();
        }

        public Show Update(Show show)
        {
            throw new NotImplementedException();
        }
    }
}
