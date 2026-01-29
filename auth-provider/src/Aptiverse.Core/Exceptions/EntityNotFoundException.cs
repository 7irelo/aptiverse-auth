namespace Aptiverse.Core.Exceptions
{
    public class EntityNotFoundException(string entityName, object key)
        : Exception($"Entity '{entityName}' with key '{key}' was not found.")
    {
        public string EntityName { get; } = entityName;
        public object Key { get; } = key;
    }
}