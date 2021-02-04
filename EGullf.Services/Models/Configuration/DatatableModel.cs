namespace EGullf.Services.Models.Configuration
{
    public class DatatableModel
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int? iDisplayLength { get; set; }
        public int iTotalRecords { get; set; }
        public int? iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public string sSortColumn { get; set; }

        public PagerModel ToPager()
        {
            PagerModel resp = new PagerModel();
            resp.Offset = iDisplayLength;
            resp.SortBy = sSortColumn;
            resp.SortDir = sSortDir_0;
            resp.Start = iDisplayStart;
            resp.Search = sSearch;
            return resp;
        }
    }
}
