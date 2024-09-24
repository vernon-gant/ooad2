// Learning simplified example from real world project. Let's say we have a system with different
// types of resources - equipment, places, people and so on. And when importing a resource we want to validate them.
// We do not want derived classes to override our main validation logic, so we do not mark this method as virtual what would allow derived classes to override it
// However for example we still make the class open because we want to work with different types of resources and depending on the resource type we may have different validatio
// rules, so we have an abstract method which must be implemented by derived classes to provide concrete resource type/category which our validation factory will use
// to deliver correct validator.
public abstract class ImportCoordinator
{
    private readonly ValidatorFactory _validatorFactory;

    public async Task<ValidationResult> ValidateImportAsync(ResourceImport resourceImport)
    {
        ImportValidator validator = _validatorFactory.CreateAsync(GetResourceCategory());

        validator.ValidateResourceHeaders(resourceImport.ResourceHeaders);
        validator.ValidateCustomFieldHeaders(resourceImport.CustomFieldHeaders);

        List<ValidationError> validationErrors = resourceImport.Resources
            .Select(resource => validator.ValidateResource(resource))
            .Where(result => !result.IsSuccess)
            .Select(result => result.ValidationErrors)
            .ToList();

        return validationErrors.Count == 0 ? ValidationResult.Success() : ValidationResult.WithErrors(validationErrors);
    }

    public abstract ResourceCategory GetResourceCategory();
}