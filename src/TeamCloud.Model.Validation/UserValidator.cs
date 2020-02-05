/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using FluentValidation;
using TeamCloud.Model.Data;

namespace TeamCloud.Model.Validation
{
    public sealed class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(obj => obj.Id).MustBeGuid();
            RuleFor(obj => obj.Role).NotEmpty();
            //RuleFor(obj => obj.Tags).NotEmpty();
        }
    }
}
