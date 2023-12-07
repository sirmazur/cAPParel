namespace cAPParel.BlazorApp.Helpers
{
    public class OperationResult
    {
        public bool IsSuccesss { get; set; }
        public string? Message { get; set; }

        public OperationResult(bool issuccess, string? message=null) 
        {
            IsSuccesss = issuccess;
            Message = message;
        }
    }
}
