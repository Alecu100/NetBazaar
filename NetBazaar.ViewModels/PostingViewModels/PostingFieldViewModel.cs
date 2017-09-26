namespace NetBazaar.ViewModels.PostingViewModels
{
    public class PostingFieldViewModel
    {
        public long CategoryFieldId { get; set; }

        public string CategoryFieldName { get; set; }

        public string TextValue { get; set; }

        public int? IntegerValue { get; set; }

        public double? DecimalValue { get; set; }

        public bool? BooleanValue { get; set; }

        public int Type { get; set; }
    }
}