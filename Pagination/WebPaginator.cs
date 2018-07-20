using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace Pagination
{
    public static class WebPaginator
    {
        /// <summary>
        /// Returns a Tuple with a list and a JSon containing the pagination header for the http response.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Tuple</returns>
        public static (IQueryable<T> data, string paginationHeader)Paginate<T>(this IQueryable<T> data, int pageNumber, int pageSize)
        {
            var count = data.Count();
            var paginationHeader = JsonConvert.SerializeObject(new
            {
                count,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling((decimal)count / pageSize)
            });

            var paginatedData = data.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            return (paginatedData, paginationHeader);
        }
    }
}
