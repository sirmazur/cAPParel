namespace cAPParel.API.Filters
{
    public interface IFilter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }        
    }
}
