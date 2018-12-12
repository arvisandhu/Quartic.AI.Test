namespace Quartic.AI.Test.SignalEngine
{
    using System;
    using System.Xml.Linq;
    using Quartic.AI.Test.Attributes;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.Essentials;

    public class SignalRule : ObservableObject
    {
        [DoNotIncludeInReflection]
        public XElement Element { get; set; }

        private string _signalID;
        [Exempt]
        [Integer]
        [String]
        [DateTime]
        public string SignalID
        {
            get { return _signalID; }
            set
            {
                _signalID = value;
                this.UpdateElement(nameof(this.SignalID), value);
                this.RaisePropertyChanged();
            }
        }

        private ValueDataType _valueType;
        [Exempt]
        [Integer]
        [String]
        [DateTime]
        public ValueDataType ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                this.UpdateElement(nameof(this.ValueType), value);
                this.RaisePropertyChanged();
            }
        }

        private TrueFalse _isActive;
        [Exempt]
        [Integer]
        [String]
        [DateTime]
        public TrueFalse IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                this.UpdateElement(nameof(this.IsActive), value);
                this.RaisePropertyChanged();
            }
        }

        private TrueFalse _allowNull;
        [Exempt]
        [Integer]
        [String]
        [DateTime]
        public TrueFalse AllowNull
        {
            get { return _allowNull; }
            set
            {
                _allowNull = value;
                this.UpdateElement(nameof(this.AllowNull), value);
                this.RaisePropertyChanged();
            }
        }

        private double? _minValue;
        [Integer]
        public double? MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                this.UpdateElement(nameof(this.MinValue), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private double? _maxValue;
        [Integer]
        public double? MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                this.UpdateElement(nameof(this.MaxValue), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private string[] _allowedValues;
        [String]
        public string[] AllowedValues
        {
            get { return _allowedValues; }
            set
            {
                _allowedValues = value;
                this.UpdateElement(nameof(this.AllowedValues), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private string[] _notAllowedValues;
        [String]
        public string[] NotAllowedValues
        {
            get { return _notAllowedValues; }
            set
            {
                _notAllowedValues = value;
                this.UpdateElement(nameof(this.NotAllowedValues), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private int? _minLength;
        [String]
        public int? MinLength
        {
            get { return _minLength; }
            set
            {
                _minLength = value;
                this.UpdateElement(nameof(this.MinLength), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private int? _maxLength;
        [String]
        public int? MaxLength
        {
            get { return _maxLength; }
            set
            {
                _maxLength = value;
                this.UpdateElement(nameof(this.MaxLength), value, value == null);
                this.RaisePropertyChanged();
            }
        }

        private TrueFalse _allowFutureDate;
        [DateTime]
        public TrueFalse AllowFutureDate
        {
            get { return _allowFutureDate; }
            set
            {
                _allowFutureDate = value;
                this.UpdateElement(nameof(this.AllowFutureDate), value, this.ValueType != ValueDataType.Datetime);
                this.RaisePropertyChanged();
            }
        }

        private string _dateFormat;
        public string DateFormat
        {
            get { return _dateFormat; }
            set
            {
                _dateFormat = value;
                this.UpdateElement(nameof(this.DateFormat), value, string.IsNullOrWhiteSpace(value));
                this.RaisePropertyChanged();
            }
        }

        private string _minDate;
        [DateTime]
        public string MinDate
        {
            get { return _minDate; }
            set
            {
                _minDate = value;
                this.UpdateElement(nameof(this.MinDate), value, string.IsNullOrWhiteSpace(value));
                this.RaisePropertyChanged();
            }
        }

        private string _maxDate;
        [DateTime]
        public string MaxDate
        {
            get { return _maxDate; }
            set
            {
                _maxDate = value;
                this.UpdateElement(nameof(this.MaxDate), value, string.IsNullOrWhiteSpace(value));
                this.RaisePropertyChanged();
            }
        }

        public void UpdateElement(string attributeName, object attributeValue, bool remove = false)
        {
            if (this.Element != null)
            {
                string value = string.Empty;

                if (attributeValue != null)
                {
                    value = attributeValue.ToString();

                    if (attributeValue.GetType().Name == "String[]")
                        value = string.Join(";", attributeValue as string[]);
                }

                XAttribute attribute = this.Element.Attribute(attributeName);

                if (attribute != null)
                {
                    if (remove)
                        attribute.Remove();
                    else
                        this.Element.Attribute(attributeName).Value = value;
                }
                else if (!remove)
                {
                    attribute = new XAttribute(attributeName, value);
                    this.Element.Add(attribute);
                }

                this.RaisePropertyChanged(nameof(this.Element));
            }
        }

        public SignalRule DeepCopy()
        {
            return new SignalRule
            {
                AllowedValues = this.AllowedValues,
                AllowFutureDate = this.AllowFutureDate,
                AllowNull = this.AllowNull,
                DateFormat = this.DateFormat,
                Element = this.Element,
                IsActive = this.IsActive,
                MaxDate = this.MaxDate,
                MaxLength = this.MaxLength,
                MaxValue = this.MaxValue,
                MinDate = this.MinDate,
                MinLength = this.MinLength,
                MinValue = this.MinValue,
                NotAllowedValues = this.NotAllowedValues,
                SignalID = this.SignalID,
                ValueType = this.ValueType
            };
        }
    }
}