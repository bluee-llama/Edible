using System.Collections.Generic;

namespace Edible.Domain.Entities;

public partial class Chef : Usuario
{
    public virtual IList<Receta> Receta { get; set; } = new List<Receta>();
}
