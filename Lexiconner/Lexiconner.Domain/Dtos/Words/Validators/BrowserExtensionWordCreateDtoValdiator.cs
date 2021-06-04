using FluentValidation;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class BrowserExtensionWordCreateDtoValdiator : AbstractValidator<BrowserExtensionWordCreateDto>
    {
        public BrowserExtensionWordCreateDtoValdiator()
        {
            RuleFor(x => x.Word).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Meaning).NotEmpty().MaximumLength(200);
            RuleFor(x => x.WordLanguageCode).NotEmpty();
            RuleFor(x => x.MeaningLanguageCode).NotEmpty();
        }
    }
}
