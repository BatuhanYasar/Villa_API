using System.Net;

namespace MagicVilla_API.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages {  get; set; }

        public object Result { get; set; }
        
        // Bu tür bir özellik genellikle bir sınıfta bir işlem sonucunu veya durumu temsil etmek için kullanılır. Örneğin, bir metot bir işlemi gerçekleştirir ve sonucunu bu "Result" özelliğine atar, böylece çağrılan taraf bu özelliği kullanarak işlemin sonucuna veya durumuna erişebilir.
    }
}
