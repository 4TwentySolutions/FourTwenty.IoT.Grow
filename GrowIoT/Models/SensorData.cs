
namespace GrowIoT.Models
{
    public class BaseModuleResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ModuleResponse<T> : BaseModuleResponse
    {
        public T Data { get; set; }
    }
}
