namespace VirtualStoreApp.Services;

/**
 * Class to manage api responses
 */
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}
