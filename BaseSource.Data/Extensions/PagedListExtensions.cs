using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BaseSource.Data.EF
{
    public static class PagedListExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(
            this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var counts = await source.ToListAsync();
            var counts2 = source.ToList();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return new StaticPagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}
