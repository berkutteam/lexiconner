using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserFilms
{
    public class UserFilmsRequestDto : PaginationRequestDto
    {
        public string Search { get; set; }
    }
}
