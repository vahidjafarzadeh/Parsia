using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Parsia.Core.UseCase
{
    public class UseCaseDto
    {
        [Display(Name = "شناسه")]
        public long EntityId { get; set; }
    }
}
