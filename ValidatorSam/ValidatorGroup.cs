using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// A class that groups several validators together.
    /// For example, it can be used to make a button active only when all validators are valid.
    /// </summary>
    public class ValidatorGroup : INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
    {
        private readonly List<Validator> _validators;

        /// <summary>
        /// Implementation of INotifyDataErrorInfo interface
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Implementation of INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Empty validator group
        /// </summary>
        public ValidatorGroup()
        {
            _validators = new List<Validator>();
        }

        /// <summary>
        /// Include exists validators
        /// </summary>
        /// <param name="validators"></param>
        public ValidatorGroup(IEnumerable<Validator> validators)
        {
            _validators = new List<Validator>(validators);

            foreach (var item in _validators)
            {
                item.ValidationChanged += Item_ValidationChanged;
            }

            int errors = 0;
            int valids = 0;
            int total = 0;
            foreach (var item in _validators)
            {
                if (!item.IsEnabled)
                    continue;

                if (item.IsValid)
                    valids++;
                else
                    errors++;

                total++;
            }

            HasErrors = errors > 0;
            IsValid = valids == total && total > 0;
        }

        private void Item_ValidationChanged(Validator invoker, bool isValid)
        {
            int errors = 0;
            int valids = 0;
            int total = 0;
            foreach (var item in _validators)
            {
                if (!item.IsEnabled)
                    continue;

                if (item.IsValid)
                    valids++;
                else
                    errors++;

                total++;
            }

            HasErrors = errors > 0;
            IsValid = valids == total && total > 0;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
        }

        /// <summary>
        /// Indicator of validation errors or lack thereof
        /// </summary>
        public bool HasErrors { get; private set; }

        /// <summary>
        /// Reports that all enabled validators are free of errors.
        /// </summary>
        public bool IsValid { get; private set; }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            var match = _validators.FirstOrDefault(x => x.Name == propertyName);
            if (match != null)
            {
                return new string[] { match.TextError ?? "" };
            }
            else
            {
                return new string[] { "" };
            }
        }

        /// <summary>
        /// Unsubscribe from validator events
        /// </summary>
        public void Dispose()
        {
            foreach (var item in _validators)
            {
                item.ValidationChanged -= Item_ValidationChanged;
            }
            _validators.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Auto-create singleton instance with using Fody code postprocessing
        /// </summary>
        public static ValidatorGroupBuilder Build()
        {
            var builder = new ValidatorGroupBuilder();
            return builder;
        }
    }
}
#nullable disable