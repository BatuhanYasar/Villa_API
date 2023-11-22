using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models
{
    public class VillaNumber
    {
        // VillaNo özelliği, bu tablodaki birincil anahtar sütununu ifade eder ve bu değer genellikle kullanıcı tarafından belirlenir veya elle atanır. Bu sütunun veritabanı tarafından otomatik olarak oluşturulmayacağını belirtmek için [DatabaseGenerate (DatabaseGeneratedOption.None)] kullanılır.

        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }

        public string SpecialDetails { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime  UpdatedDate { get; set;}
    }
}
