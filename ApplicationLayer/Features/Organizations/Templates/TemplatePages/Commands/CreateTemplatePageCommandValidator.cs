using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Commands
{
    public class CreateTemplatePageCommandValidator : AbstractValidator<CreateTemplatePageCommand>
    {
        public CreateTemplatePageCommandValidator()
        {
            RuleFor(x => x.TemplateVersionId)
                .GreaterThan(0).WithMessage("Template version ID must be greater than 0.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("Width must be greater than 0.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0.");
        }
    }
}

