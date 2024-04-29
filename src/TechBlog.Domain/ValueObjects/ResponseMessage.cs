using System.ComponentModel;

namespace TechBlog.Domain.ValueObjects
{
    public enum ResponseMessage
    {
        Default,
        Forbidden,
        NotFound,
        ValidationError,
        UnexpectedError,
        InvalidApiKey,
        InvalidEmail,
        InvalidEmailBody,
        InvalidEmailSubject,
        UserIsNotAuthenticated,
        UserAlreadyExists,
        UserDontExists,
        ErrorCreatingUser,
        InvalidName,
        InvalidPassword,
        InvalidUserType,
        InvalidInformation,
        InvalidCredentials,
        InvalidTitle,
        InvalidDescription,
        InvalidBody,
        InvalidTags,
        InvalidId,
        UserMustBeAJournalist,        
        UserIsNotTheOwner,
        InvalidCurrentPassword,
        InvalidNewPassword,
        InvalidNewPasswordConfirmation,
        PasswordsMustBeTheSame,
        [Description("The strategy is not implemented")]
        StrategyIsNotImplemented,
        [Description("The strategy has more than one implementation")]
        StrategyHasMoreThanOneImplementation
    }
}
