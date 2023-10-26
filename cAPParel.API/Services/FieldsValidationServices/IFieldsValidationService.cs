namespace cAPParel.API.Services.FieldsValidationServices
{
    public interface IFieldsValidationService
    {
        bool TypeHasProperties<T>(string? fields);
    }
}