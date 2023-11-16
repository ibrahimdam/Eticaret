using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Entities
{
    public class ProductCategory
    {
        // Bu Class'ın amacı; bir product bir den fazla category'e sahip olabilir, vir category de birden fazla product'a sahip olabilir. Yani burada Çoktan Çoğa (Many-To-Many Relation) bir ilişki söz konusu. Bunu da ilişki kurulacak Class'ların dışında üçüncü bir Class tanımlayarak oluşturabiliyoruz.
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
