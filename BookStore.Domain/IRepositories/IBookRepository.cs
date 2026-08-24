using FixHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Domain.IRepositories
{
    public interface IBookRepository: IRepository<Book>
    {
        // Specific methods for Book repository
        /// <summary>
        /// Get books by author
        /// </summary>
        //Task<List<Book>> GetByAuthorAsync(string author);
    }
}
