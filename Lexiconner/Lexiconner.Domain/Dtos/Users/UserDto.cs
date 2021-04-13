using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Users
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<ApplicationUserEntityLearningLanguage> LearningLanguages { get; set; }
    }
}
