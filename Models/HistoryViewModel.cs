using System;
using System.Collections.Generic;

namespace checkpanel.Models
{
    public class HistoryViewModel
    {
        public int TotalItems;
        public int ItemsPerPage = 10;
        public int CurrentPage = 1;
        public int TotalPages;

        public List<HistoryModel> Records;
    }
}
