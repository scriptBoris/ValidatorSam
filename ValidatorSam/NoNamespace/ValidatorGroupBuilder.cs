using System;
using System.Text;
using System.Collections.Generic;

namespace ValidatorSam
{
    /// <summary>
    /// Special builder class for easy generate ValidatorGroup
    /// </summary>
    public class ValidatorGroupBuilder
    {
        private readonly LinkedList<Validator> _validators = new LinkedList<Validator>();

        /// <summary>
        /// Adding a validator to a group
        /// </summary>
        /// <param name="validator"></param>
        public ValidatorGroupBuilder Include(Validator validator)
        {
            _validators.AddLast(validator);
            return this;
        }

        private ValidatorGroup FinishBuild()
        {
            var validatorGroup = new ValidatorGroup(_validators);
            return validatorGroup;
        }

        /// <summary>
        /// Allow to make autobuild
        /// </summary>
        public static implicit operator ValidatorGroup(ValidatorGroupBuilder builder)
        {
            return builder.FinishBuild();
        }
    }
}
