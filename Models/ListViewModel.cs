using System;
using System.Collections.Generic;

namespace checkpanel.Models
{
    public class ListViewModel
    {
        public ListViewModel()
        {
            this.ListSet = new List<(ItemModel, PunchModel, DateTime)>();
        }

        public List<(ItemModel, PunchModel, DateTime)> ListSet;
    }
}
