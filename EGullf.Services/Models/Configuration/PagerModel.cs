namespace EGullf.Services.Models.Configuration
{
    public class PagerModel
    {
        private int? _Start;
        private int? _Offset;
        private string _SortBy;
        private string _SortDir;
        private int? _TotalRecords;
        private string _Search;

        public PagerModel()
        {

        }

        public PagerModel(int? start, int? offset, string sortby, string sortdir,string search="")
        {
            Start = start;
            Offset = offset;
            SortBy = sortby;
            SortDir = sortdir;
            Search = search;
        }

        public int? Start
        {
            get
            {
                if (_Start == null)
                    return 0;
                else return _Start;
            }
            set
            {
                _Start = value;
            }
        }

        public int? Offset
        {
            get
            {
                if (_Offset == null)
                    return 0;
                else return _Offset;
            }
            set
            {
                _Offset = value;
            }
        }

        public string SortBy
        {
            get
            {
                if (_SortBy == null)
                    return "";
                else return _SortBy;
            }
            set
            {
                _SortBy = value;
            }
        }

        public string SortDir
        {
            get
            {
                if (string.IsNullOrEmpty(_SortBy))
                    return "asc";
                else return _SortDir;
            }
            set
            {
                _SortDir = value;
            }
        }
        public int? TotalRecords {
            get
            {
                if (_TotalRecords == null)
                    return 0;
                else return _TotalRecords;
            }
            set
            {
                _TotalRecords = value;
            }
        }

        public string Search {
            get {
                if (_Search == null)
                    return "";
                else return _Search;
            }
            set {
                _Search = value;
            }
        }
    }
}
