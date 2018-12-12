namespace Quartic.AI.Test.Dialogs
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.Extensions;
    using Quartic.AI.Test.Models;
    using Quartic.AI.Test.SignalEngine;

    public class NewRuleViewModel : DialogViewModelBase
    {
        private bool _performValidationOnLoad;

        public NewRuleViewModel(bool performValidationOnLoad = true)
        {
            _performValidationOnLoad = performValidationOnLoad;
            this.Dialog = new NewRuleDialog { DataContext = this };
            this.Title = "New Rule";
            this.PrimaryButtonText = "Add";
        }

        private SignalRule _signalRule;
        public SignalRule SignalRule
        {
            get { return _signalRule; }
            set
            {
                _signalRule = value;
                this.RaisePropertyChanged();

                this.FillSignalProperties(value);
            }
        }

        private ElementProperties _elementProperties;
        public ElementProperties ElementProperties
        {
            get { return _elementProperties ?? (this._elementProperties = new ElementProperties()); }
            set
            {
                _elementProperties = value;
                this.RaisePropertyChanged();
            }
        }

        protected override bool CanExecutePrimaryCommand()
        {
            return this.IsDirty && this.ElementProperties.All(x => !x.HasErrors);
        }

        private void Clear(ValueDataType valueType, SignalRule signalRule)
        {
            PropertyInfo[] propertiesInfo = null;

            if (valueType.HasFlag(ValueDataType.Integer))
            {
                propertiesInfo = signalRule.GetType().GetIntegerWhereNotExemptProperties();
                if (propertiesInfo != null)
                {
                    foreach (PropertyInfo propertyInfo in propertiesInfo)
                    {
                        propertyInfo.SetValue(signalRule, null);
                    }
                }
            }

            if (valueType.HasFlag(ValueDataType.String))
            {
                propertiesInfo = signalRule.GetType().GetStringWhereNotExemptProperties();
                if (propertiesInfo != null)
                {
                    foreach (PropertyInfo propertyInfo in propertiesInfo)
                    {
                        propertyInfo.SetValue(signalRule, null);
                    }
                }
            }

            if (valueType.HasFlag(ValueDataType.Datetime))
            {
                propertiesInfo = signalRule.GetType().GetDateTimeWhereNotExemptProperties();
                if (propertiesInfo != null)
                {
                    foreach (PropertyInfo propertyInfo in propertiesInfo)
                    {
                        propertyInfo.SetValue(signalRule, null);
                    }
                }
            }
        }

        private void FillSignalProperties(SignalRule signalRule)
        {
            this.ElementProperties.ToList().ForEach(property => property.PropertyChanged -= this.ElementPropertyChanged);
            this.ElementProperties.Clear();

            if (signalRule != null)
            {
                ElementProperties properties = new ElementProperties();
                PropertyInfo[] propertiesInfo = null;

                switch (signalRule.ValueType)
                {
                    case ValueDataType.Integer:
                        propertiesInfo = signalRule.GetType().GetIntegerProperties();
                        this.Clear(ValueDataType.Datetime | ValueDataType.String, signalRule);
                        break;
                    case ValueDataType.String:
                        propertiesInfo = signalRule.GetType().GetStringProperties();
                        this.Clear(ValueDataType.Integer | ValueDataType.Datetime, signalRule);
                        break;
                    case ValueDataType.Datetime:
                        propertiesInfo = signalRule.GetType().GetDateTimeProperties();
                        this.Clear(ValueDataType.Integer | ValueDataType.String, signalRule);
                        break;
                    default:
                        propertiesInfo = signalRule.GetType().GetExemptProperties();
                        this.Clear(ValueDataType.Integer | ValueDataType.String | ValueDataType.Datetime, signalRule);
                        break;
                }

                foreach (PropertyInfo propertyInfo in propertiesInfo)
                {
                    ElementProperty elementProperty = null;

                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        elementProperty = this.GetComboBoxProperty(signalRule, propertyInfo);
                    }
                    else if (propertyInfo.PropertyType == typeof(double?))
                    {
                        elementProperty = this.GetDoubleProperty(signalRule, propertyInfo);
                    }
                    else if (propertyInfo.PropertyType == typeof(int?))
                    {
                        elementProperty = this.GetIntegerProperty(signalRule, propertyInfo);
                    }
                    else if (propertyInfo.PropertyType.IsArray)
                    {
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case "String[]":
                                elementProperty = this.GetArrayTextProperty(signalRule, propertyInfo);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        elementProperty = this.GetTextProperty(signalRule, propertyInfo);
                    }

                    properties.Add(elementProperty);
                }

                this.ElementProperties = properties;

                this.ElementProperties.ToList().ForEach(property =>
                {
                    if (!property.IsReadOnly)
                        property.PropertyChanged += this.ElementPropertyChanged;
                });
            }

            this.PrimaryCommand.RaiseCanExecuteChanged();
        }

        private ElementProperty GetComboBoxProperty(SignalRule signalRule, PropertyInfo propertyInfo)
        {
            ElementProperty elementProperty = new ComboBoxProperty
            {
                PerformValidationOnLoad = _performValidationOnLoad,
                SignalRule = signalRule,
                PropertyName = propertyInfo.Name,
                PropertyValue = propertyInfo.GetValue(signalRule)
            };

            foreach (var item in propertyInfo.PropertyType.GetEnumValues())
            {
                (elementProperty as ComboBoxProperty).Lookups.Add(item.ToString());
            }

            return elementProperty;
        }

        private ElementProperty GetDoubleProperty(SignalRule signalRule, PropertyInfo propertyInfo)
        {
            return new DoubleProperty
            {
                PerformValidationOnLoad = _performValidationOnLoad,
                SignalRule = signalRule,
                PropertyName = propertyInfo.Name,
                PropertyValue = propertyInfo.GetValue(signalRule)
            };
        }

        private ElementProperty GetIntegerProperty(SignalRule signalRule, PropertyInfo propertyInfo)
        {
            return new IntegerProperty
            {
                PerformValidationOnLoad = _performValidationOnLoad,
                SignalRule = signalRule,
                PropertyName = propertyInfo.Name,
                PropertyValue = propertyInfo.GetValue(signalRule)
            };
        }

        private ElementProperty GetTextProperty(SignalRule signalRule, PropertyInfo propertyInfo)
        {
            return new TextProperty
            {
                PerformValidationOnLoad = _performValidationOnLoad,
                SignalRule = signalRule,
                PropertyName = propertyInfo.Name,
                PropertyValue = propertyInfo.GetValue(signalRule)
            };
        }

        private ElementProperty GetArrayTextProperty(SignalRule signalRule, PropertyInfo propertyInfo)
        {
            object value = propertyInfo.GetValue(signalRule);

            return new TextProperty
            {
                PerformValidationOnLoad = _performValidationOnLoad,
                SignalRule = signalRule,
                PropertyName = propertyInfo.Name,
                PropertyValue = value != null ? string.Join(";", value as string[]) : null
            };
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (sender is ElementProperty elementProperty)
            {
                if (args.PropertyName == nameof(elementProperty.PropertyValue))
                {
                    if (elementProperty.PropertyName == nameof(elementProperty.SignalRule.ValueType))
                    {
                        this.FillSignalProperties(elementProperty.SignalRule);
                    }

                    this.ElementProperties.ValidateCrossFieldDependency(elementProperty.SignalRule);
                    this.IsDirty = true;
                }
            }

            this.PrimaryCommand.RaiseCanExecuteChanged();
        }
    }
}