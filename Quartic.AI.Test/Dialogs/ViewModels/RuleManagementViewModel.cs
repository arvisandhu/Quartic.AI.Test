namespace Quartic.AI.Test.Dialogs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Xml.Linq;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.Essentials;
    using Quartic.AI.Test.Extensions;
    using Quartic.AI.Test.Services;
    using Quartic.AI.Test.SignalEngine;

    public class RuleManagementViewModel : DialogViewModelBase
    {
        private readonly DialogContainerService _dialogContainerService;
        private readonly MessageDialogViewModel _messageDialog;
        private XDocument _document;

        public RuleManagementViewModel()
        {
            _dialogContainerService = new DialogContainerService();
            _messageDialog = new MessageDialogViewModel();

            this.Dialog = new RuleManagementDialog { DataContext = this };
            this.Title = "Rule Management";

            _newRuleCommand = new RelayCommand(this.NewRuleCommandHandler);
            _editRuleCommand = new RelayCommand<SignalRule>(this.EditRuleCommandHandler, this.CanExecuteEditRuleCommand);
            _deleteRuleCommand = new RelayCommand<SignalRule>(this.DeleteRuleCommandHandler, this.CanExecuteEditRuleCommand);
            _deleteSelectedRulesCommand = new RelayCommand<object>(this.DeleteSelectedRulesCommandHandler, this.CanExecuteDeleteSelectedRulesCommand);
        }

        private ObservableCollection<SignalRule> _signalRules;
        public ObservableCollection<SignalRule> SignalRules
        {
            get { return _signalRules ?? (_signalRules = new ObservableCollection<SignalRule>()); }
            set
            {
                _signalRules = value;
                this.RaisePropertyChanged();
            }
        }

        private SignalRule _selectedSignalRule;
        public SignalRule SelectedSignalRule
        {
            get { return _selectedSignalRule; }
            set
            {
                _selectedSignalRule = value;
                this.RaisePropertyChanged();

                this.EditRuleCommand.RaiseCanExecuteChanged();
                this.DeleteRuleCommand.RaiseCanExecuteChanged();
                this.DeleteSelectedRulesCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly RelayCommand _newRuleCommand;
        public RelayCommand NewRuleCommand
        {
            get { return _newRuleCommand; }
        }

        private void NewRuleCommandHandler()
        {
            XElement element = null;
            SignalRule signalRule = null;

            element = XElementExtensions.NewSignalRule();
            signalRule = element.ToTypeSignalRule();

            if (element != null && signalRule != null)
            {
                NewRuleViewModel newRuleViewModel = new NewRuleViewModel { SignalRule = signalRule };

                // here we have to open a new rule dialog.
                _dialogContainerService.ShowDialog(newRuleViewModel);

                if (newRuleViewModel.Result == DialogResult.Success)
                {
                    if (this.SignalRules != null && _document != null)
                    {
                        _document.Root.Add(element, Environment.NewLine);
                        this.SignalRules.Add(signalRule);
                        this.IsDirty = true;
                        this.PrimaryCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private readonly RelayCommand<SignalRule> _editRuleCommand;
        public RelayCommand<SignalRule> EditRuleCommand
        {
            get { return _editRuleCommand; }
        }

        private void EditRuleCommandHandler(SignalRule signalRule)
        {
            if (signalRule != null)
            {
                SignalRule preservedSignalRule = signalRule.DeepCopy();

                NewRuleViewModel editRuleViewModel = new NewRuleViewModel(performValidationOnLoad: false) { SignalRule = signalRule };
                editRuleViewModel.Title = "Edit Rule";
                editRuleViewModel.PrimaryButtonText = "Save";

                // here we have to open a new rule dialog.
                _dialogContainerService.ShowDialog(editRuleViewModel);

                if (editRuleViewModel.Result == DialogResult.Success)
                {
                    this.IsDirty = true;
                    this.PrimaryCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    signalRule.AllowedValues = preservedSignalRule.AllowedValues;
                    signalRule.AllowFutureDate = preservedSignalRule.AllowFutureDate;
                    signalRule.AllowNull = preservedSignalRule.AllowNull;
                    signalRule.DateFormat = preservedSignalRule.DateFormat;
                    signalRule.Element = preservedSignalRule.Element;
                    signalRule.IsActive = preservedSignalRule.IsActive;
                    signalRule.MaxDate = preservedSignalRule.MaxDate;
                    signalRule.MaxLength = preservedSignalRule.MaxLength;
                    signalRule.MaxValue = preservedSignalRule.MaxValue;
                    signalRule.MinDate = preservedSignalRule.MinDate;
                    signalRule.MinLength = preservedSignalRule.MinLength;
                    signalRule.MinValue = preservedSignalRule.MinValue;
                    signalRule.NotAllowedValues = preservedSignalRule.NotAllowedValues;
                    signalRule.SignalID = preservedSignalRule.SignalID;
                    signalRule.ValueType = preservedSignalRule.ValueType;
                }
            }
        }

        private bool CanExecuteEditRuleCommand(SignalRule signalRule)
        {
            return this.SelectedSignalRule != null;
        }

        private readonly RelayCommand<SignalRule> _deleteRuleCommand;
        public RelayCommand<SignalRule> DeleteRuleCommand
        {
            get { return _deleteRuleCommand; }
        }

        private void DeleteRuleCommandHandler(SignalRule signalRule)
        {
            if (signalRule != null)
            {
                if (this.SignalRules.Contains(signalRule))
                {
                    signalRule.Element.Remove();
                    this.SignalRules.Remove(signalRule);
                    this.IsDirty = true;
                    this.PrimaryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private readonly RelayCommand<object> _deleteSelectedRulesCommand;
        public RelayCommand<object> DeleteSelectedRulesCommand
        {
            get { return _deleteSelectedRulesCommand; }
        }

        private void DeleteSelectedRulesCommandHandler(object signalRulesObject)
        {
            if (signalRulesObject != null)
            {
                if (signalRulesObject is string value)
                {
                    if (value.Equals("ALL"))
                    {
                        this.SignalRules.ToList().ForEach(signalRule => signalRule.Element.Remove());
                        this.SignalRules.Clear();
                        this.IsDirty = true;
                        this.PrimaryCommand.RaiseCanExecuteChanged();
                    }
                }
                else
                {
                    IList items = (IList)signalRulesObject;
                    List<SignalRule> signalRules = items.Cast<SignalRule>().ToList();

                    for (int index = 0; index < signalRules.Count(); index++)
                    {
                        SignalRule signalRule = signalRules[index];
                        this.DeleteRuleCommandHandler(signalRule);
                    }
                }
            }
        }

        private bool CanExecuteDeleteSelectedRulesCommand(object args)
        {
            if (args != null)
            {
                if (args is string value)
                {
                    if (value.Equals("ALL"))
                        return this.SignalRules != null && this.SignalRules.Count > 0;
                }
            }

            return this.SelectedSignalRule != null;
        }

        public override async Task Prepare()
        {
            this.SignalRules = await Task.Run(() => this.LoadRules());

            this.EditRuleCommand.RaiseCanExecuteChanged();
            this.DeleteRuleCommand.RaiseCanExecuteChanged();
            this.DeleteSelectedRulesCommand.RaiseCanExecuteChanged();
        }

        protected override void PrimaryCommandHandler()
        {
            if (_document != null)
            {
                string signalDefinitionFilePath = ConfigurationService.GetValue("SignalDefinitions");
                if (!string.IsNullOrWhiteSpace(signalDefinitionFilePath))
                {
                    if (!File.Exists(signalDefinitionFilePath))
                        return;
                }

                try
                {
                    if (_document.Declaration == null)
                    {
                        File.WriteAllText(signalDefinitionFilePath, _document.ToString(SaveOptions.None));
                    }
                    else
                    {
                        File.WriteAllText(signalDefinitionFilePath, _document.Declaration.ToString().Trim() + Environment.NewLine + _document.ToString(SaveOptions.None).Trim());
                    }

                    base.PrimaryCommandHandler();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        protected override bool CanExecutePrimaryCommand()
        {
            return this.IsDirty;
        }

        private ObservableCollection<SignalRule> LoadRules()
        {
            _document = null;

            string signalDefinitionFilePath = ConfigurationService.GetValue("SignalDefinitions");
            signalDefinitionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, signalDefinitionFilePath);

            if (!string.IsNullOrWhiteSpace(signalDefinitionFilePath))
                if (File.Exists(signalDefinitionFilePath))
                    _document = XDocument.Load(signalDefinitionFilePath, LoadOptions.None);

            if (_document == null)
            {
                _messageDialog.Caption = "Config Rule File";
                _messageDialog.Message = $"Couldn't locate configuration file at {signalDefinitionFilePath}. You need this file to manage the rules. Clicking \"OK\" will terminate the application.";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dialogContainerService.ShowDialog(_messageDialog);
                    Application.Current.Shutdown();
                });
            }

            if (_document != null)
            {
                if (_document.Root != null)
                {
                    // get all signal nodes
                    List<XElement> signals = _document.Root.Elements("Signal").ToList();

                    // convert them to class instance
                    List<SignalRule> signalRules = signals.ToListOfTypeSignalRule();
                    return new ObservableCollection<SignalRule>(signalRules);
                }
            }

            return null;
        }

        public bool Validate(JsonSignal jsonSignal)
        {
            if (jsonSignal != null)
            {
                SignalRule signalRule = this.SignalRules.FirstOrDefault(rule
                    => rule.IsActive == TrueFalse.True
                    && rule.SignalID.Equals(jsonSignal.SignalID, StringComparison.OrdinalIgnoreCase)
                    && rule.ValueType.ToString().Equals(jsonSignal.ValueType, StringComparison.OrdinalIgnoreCase));

                if (signalRule != null)
                {
                    string jsonValue = null;
                    if (jsonSignal.Value != null)
                    {
                        jsonValue = jsonSignal.Value.ToString();
                    }

                    if (signalRule.AllowNull == TrueFalse.False)
                    {
                        if (string.IsNullOrEmpty(jsonValue))
                        {
                            return false;
                        }
                    }

                    switch (signalRule.ValueType)
                    {
                        case ValueDataType.Integer:
                            {
                                double doubleValue;
                                if (!double.TryParse(jsonValue, out doubleValue))
                                    return false;

                                if (signalRule.MinValue.HasValue)
                                    if (doubleValue < signalRule.MinValue.Value)
                                        return false;

                                if (signalRule.MaxValue.HasValue)
                                    if (doubleValue > signalRule.MaxValue.Value)
                                        return false;
                            }

                            break;
                        case ValueDataType.String:
                            {
                                // preference given to allowed values.. if there is no entry in allowed values then will look for not allowed values
                                if (signalRule.AllowedValues != null && signalRule.AllowedValues.Length > 0)
                                {
                                    if (!signalRule.AllowedValues.Any(x => x.Equals(jsonValue)))
                                    {
                                        return false;
                                    }
                                }

                                if ((signalRule.AllowedValues == null || (signalRule.AllowedValues != null && signalRule.AllowedValues.Length == 0)) &&
                                    signalRule.NotAllowedValues != null && signalRule.NotAllowedValues.Length > 0)
                                {
                                    if (signalRule.NotAllowedValues.Any(x => x.Equals(jsonValue)))
                                    {
                                        return false;
                                    }
                                }

                                if (signalRule.MinLength.HasValue)
                                {
                                    if (jsonValue.Length < signalRule.MinLength.Value)
                                    {
                                        return false;
                                    }
                                }

                                if (signalRule.MaxLength.HasValue)
                                {
                                    if (jsonValue.Length > signalRule.MaxLength.Value)
                                    {
                                        return false;
                                    }
                                }
                            }

                            break;
                        case ValueDataType.Datetime:
                            {
                                DateTime dateTime;
                                if (!DateTime.TryParse(jsonValue, out dateTime))
                                    return false;

                                if (signalRule.AllowFutureDate == TrueFalse.False)
                                {
                                    if (dateTime.Date > DateTime.Today)
                                        return false;
                                }

                                if (!string.IsNullOrWhiteSpace(signalRule.MinDate))
                                {
                                    DateTime minDate;
                                    if (DateTime.TryParse(signalRule.MinDate, out minDate))
                                    {
                                        if (dateTime.Date < minDate.Date)
                                            return false;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(signalRule.MaxDate))
                                {
                                    DateTime maxDate;
                                    if (DateTime.TryParse(signalRule.MaxDate, out maxDate))
                                    {
                                        if (dateTime.Date > maxDate.Date)
                                            return false;
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            return true;
        }
    }
}