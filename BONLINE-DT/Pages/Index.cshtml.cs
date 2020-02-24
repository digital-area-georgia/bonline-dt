using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BONLINE_DT.Data;
using BONLINE_DT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Dynamic.Core;


namespace BONLINE_DT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDbContext _context;


        public IndexModel(ILogger<IndexModel> logger, IDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }
        
        public IList<DocumentModel> Documents { get; set; }
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }
        
        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = await _context.Documents.CountDocumentsAsync(new BsonDocument());
            IQueryable<DocumentModel> documentsQuery = _context.Documents.AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                documentsQuery = Queryable.Where(documentsQuery, s =>
                    s.Document.EntryComment.ToUpper().Contains(searchText) ||
                    s.Document.EntryCommentEn.ToUpper().Contains(searchText)
                );
            }

            var dateFilter = DataTablesRequest.Columns[1].Search.Value?.ToUpper();
            var creditFilter = DataTablesRequest.Columns[2].Search.Value?.ToUpper();
            var commentFilter = DataTablesRequest.Columns[3].Search.Value?.ToUpper();

            if (!string.IsNullOrWhiteSpace(dateFilter) && DateTime.TryParseExact(dateFilter, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dateParsed))
            {
                documentsQuery = Queryable.Where(documentsQuery,
                    d => d.Document.PostDate >= dateParsed &&
                         d.Document.PostDate <= dateParsed.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            if (!string.IsNullOrWhiteSpace(creditFilter))
            {
                documentsQuery = Queryable.Where(documentsQuery,
                    s => s.Document.Credit == Convert.ToDouble(creditFilter));
            }

            if (!string.IsNullOrWhiteSpace(commentFilter))
            {
                documentsQuery = Queryable.Where(documentsQuery,
                    s => s.Document.EntryComment.ToUpper().Contains(commentFilter) ||
                         s.Document.EntryCommentEn.ToUpper().Contains(commentFilter));
            }

            var recordsFiltered = documentsQuery.Count();

            var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
            var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

            documentsQuery = documentsQuery.OrderBy($"{sortColumnName} {sortDirection}") as IOrderedMongoQueryable<DocumentModel>;
            var skip = DataTablesRequest.Start;
            var take = DataTablesRequest.Length;
            documentsQuery = Queryable.Skip(documentsQuery, skip);
            if (take != -1)
            {
                documentsQuery = Queryable.Take(documentsQuery, take);
            }
            var data = await documentsQuery.ToDynamicListAsync();

            return new JsonResult(new
            {
                Draw = DataTablesRequest.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            });
        }
        
        public DocumentModel Document { get; set; }

        public async Task<IActionResult> OnGetModalViewAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Document = await _context.Documents.FindOneAndUpdateAsync(Builders<DocumentModel>.Filter.Eq(d => d.Id, id),
                Builders<DocumentModel>.Update.Set(d => d.IsRead, true));

            if (Document == null)
            {
                return NotFound();
            }
            return Partial("_ModalView", this.ViewData.Model);
        }
    }
}