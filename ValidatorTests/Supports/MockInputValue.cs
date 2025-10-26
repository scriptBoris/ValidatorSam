using ValidatorSam;

namespace ValidatorTests.Supports
{
    public class MockInputValue
    {
        private Validator? _validator;

        public object? Value { get; set; }
        public bool HasError { get; set; }
        public string? ErrorText { get; set; }
        public Validator? Validator
        {
            get => _validator;
            set
            {
                if (_validator != null)
                    _validator.ErrorChanged -= Value_ErrorChanged;

                if (value != null)
                    value.ErrorChanged += Value_ErrorChanged;

                _validator = value;
            }
        }

        private void Value_ErrorChanged(IValidator invoker, ValidatorErrorTextArgs args)
        {
            HasError = args.IsShow;
            ErrorText = args.ErrorText;
        }
    }
}
