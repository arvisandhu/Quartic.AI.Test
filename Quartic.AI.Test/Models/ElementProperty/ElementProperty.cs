namespace Quartic.AI.Test.Models
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Quartic.AI.Test.Attributes;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.Essentials;
    using Quartic.AI.Test.SignalEngine;

    public class ElementProperty : ObservableObject, INotifyDataErrorInfo
    {
        private string _requiredFieldErrorMessage = "{0} is required.";
        private string _invalidValueErrorMessage = "{0} has invalid value.";
        private string _arrayValueRequiredErrorMessage = "{0} is required. It's a array field, Use ';' delimiter for multiple values.";
        private string _minMaxValueErrorMessage = "Min value can't be greater than Max value.";
        private string _dateErrorMessage = "Input date in YYYY-MM-DD format.";

        public ElementProperty()
        {
            this.IsMandatory = true;
        }

        public bool PerformValidationOnLoad { get; set; }

        public SignalRule SignalRule { get; set; }

        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                _propertyName = value;
                this.RaisePropertyChanged();
            }
        }

        private dynamic _propertyValue;
        public dynamic PropertyValue
        {
            get { return _propertyValue; }
            set
            {
                _propertyValue = value;
                if (this.PerformValidationOnLoad)
                    this.Validate();

                this.PerformValidationOnLoad = true;
                this.RaisePropertyChanged();
            }
        }

        private bool _isMandatory;
        public bool IsMandatory
        {
            get { return _isMandatory; }
            set { _isMandatory = value; }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                this._isReadOnly = value;
                this.RaisePropertyChanged();
            }
        }

        public bool HasDataError { get; set; }

        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();
        public Dictionary<string, ICollection<string>> ValidationErrors
        {
            get { return _validationErrors; }
        }

        public bool HasErrors { get { return this.ValidationErrors.Count > 0; } }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void RaiseErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !this.ValidationErrors.ContainsKey(propertyName))
                return null;

            return this.ValidationErrors[propertyName];
        }

        public void Validate([CallerMemberName] string propertyName = null)
        {
            if (this.ValidationErrors.ContainsKey(propertyName))
                this.ValidationErrors.Remove(propertyName);

            if (this.SignalRule != null)
            {
                PropertyInfo propertyInfo = this.SignalRule.GetType().GetProperty(this.PropertyName);
                if (propertyInfo != null)
                {
                    bool hasError = false;
                    string errorMessage = string.Format(_requiredFieldErrorMessage, this.PropertyName);

                    dynamic value = null;
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case nameof(TrueFalse):
                                if (Enum.IsDefined(typeof(TrueFalse), this.PropertyValue))
                                {
                                    if (Enum.TryParse(this.PropertyValue.ToString(), out TrueFalse trueFalse))
                                    {
                                        value = trueFalse;
                                    }
                                }
                                else if (this.IsMandatory)
                                {
                                    hasError = true;
                                }

                                break;
                            case nameof(ValueDataType):
                                if (Enum.IsDefined(typeof(ValueDataType), this.PropertyValue))
                                {
                                    if (Enum.TryParse(this.PropertyValue.ToString(), out ValueDataType valueType))
                                    {
                                        value = valueType;
                                    }
                                }
                                else if (this.IsMandatory)
                                {
                                    hasError = true;
                                }

                                break;
                        }
                    }
                    else if (propertyInfo.PropertyType.IsArray)
                    {
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case "String[]":
                                {
                                    if (this.PropertyValue != null)
                                    {
                                        value = (this.PropertyValue as string).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    }

                                    if ((value == null || (value != null && (value as string[]).Length == 0)) && this.IsMandatory)
                                    {
                                        hasError = true;
                                        errorMessage = string.Format(_arrayValueRequiredErrorMessage, this.PropertyName);
                                    }
                                }

                                break;
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(int?))
                    {
                        if (this.PropertyValue != null)
                        {
                            if (int.TryParse(this.PropertyValue.ToString(), out int result))
                                value = result;
                        }

                        if (value == null)
                        {
                            if (this.PropertyValue != null && !string.IsNullOrEmpty(this.PropertyValue as string))
                            {
                                hasError = true;
                                errorMessage = string.Format(_invalidValueErrorMessage, this.PropertyName);
                            }
                            else if (this.IsMandatory)
                            {
                                hasError = true;
                            }
                        }
                        else if (this.HasDataError)
                        {
                            hasError = true;
                            errorMessage = string.Format(_minMaxValueErrorMessage, this.PropertyName);
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(double?))
                    {
                        if (this.PropertyValue != null)
                        {
                            if (double.TryParse(this.PropertyValue.ToString(), out double result))
                                value = result;
                        }

                        if (value == null)
                        {
                            if (this.PropertyValue != null && !string.IsNullOrEmpty(this.PropertyValue.ToString()))
                            {
                                hasError = true;
                                errorMessage = string.Format(_invalidValueErrorMessage, this.PropertyName);
                            }
                            else if (this.IsMandatory)
                            {
                                hasError = true;
                            }
                        }
                        else if (this.HasDataError)
                        {
                            hasError = true;
                            errorMessage = string.Format(_minMaxValueErrorMessage, this.PropertyName);
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        if (this.PropertyValue != null)
                        {
                            value = this.PropertyValue as string;
                        }

                        if (string.IsNullOrEmpty(value) && this.IsMandatory)
                        {
                            hasError = true;
                        }
                        else if (!string.IsNullOrEmpty(value))
                        {
                            if (propertyInfo.GetCustomAttribute(typeof(DateTimeAttribute)) != null)
                            {
                                if (propertyInfo.GetCustomAttribute(typeof(ExemptAttribute)) == null)
                                {
                                    if (!DateTime.TryParse(value.ToString(), out DateTime dateTime))
                                    {
                                        hasError = true;
                                        errorMessage = string.Format(_dateErrorMessage, this.PropertyName);
                                    }
                                    else
                                    {
                                        string stringValue = value.ToString();
                                        if (stringValue.Count(x => x == '-') != 2)
                                        {
                                            hasError = true;
                                            errorMessage = string.Format(_dateErrorMessage, this.PropertyName);
                                        }
                                    }

                                    if (!hasError && this.HasDataError)
                                    {
                                        hasError = true;
                                        errorMessage = string.Format(_minMaxValueErrorMessage, this.PropertyName);
                                    }
                                }
                            }
                        }
                    }

                    if (hasError)
                    {
                        if (!this.ValidationErrors.ContainsKey(propertyName))
                        {
                            this.ValidationErrors.Add(propertyName, new List<string>());
                            this.ValidationErrors[propertyName].Add(errorMessage);
                        }
                    }

                    propertyInfo.SetValue(this.SignalRule, value);
                    this.RaiseErrorsChanged(propertyName);

                    this.RaisePropertyChanged(nameof(this.HasErrors));
                    this.RaisePropertyChanged(nameof(this.ValidationErrors));
                }
            }
        }
    }
}