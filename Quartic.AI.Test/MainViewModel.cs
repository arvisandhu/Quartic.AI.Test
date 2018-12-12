namespace Quartic.AI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Quartic.AI.Test.Dialogs;
    using Quartic.AI.Test.Essentials;
    using Quartic.AI.Test.Services;
    using Quartic.AI.Test.SignalEngine;

    public class MainViewModel : ObservableObject
    {
        private readonly DialogContainerService _dialogContainerService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly RuleManagementViewModel _ruleManagement;
        private readonly MessageDialogViewModel _messageDialog;

        public MainViewModel()
        {
            _dialogContainerService = new DialogContainerService();
            _jsonSerializer = new JsonSerializer();

            _ruleManagement = new RuleManagementViewModel();
            _messageDialog = new MessageDialogViewModel();
            _validateCommand = new RelayCommand(this.ValidateCommandHandler, this.CanExecuteValidateCommand);
            _manageRulesCommand = new RelayCommand(this.ManageRulesCommandHandler);
        }

        private string _dataStream;
        public string DataStream
        {
            get { return _dataStream; }
            set
            {
                _dataStream = value;
                this.RaisePropertyChanged();

                this.ValidateCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly RelayCommand _validateCommand;
        public RelayCommand ValidateCommand
        {
            get { return _validateCommand; }
        }

        private ObservableCollection<JsonSignal> _failedSignals;

        public ObservableCollection<JsonSignal> FailedSignals
        {
            get { return _failedSignals; }
            set
            {
                _failedSignals = value;
                this.RaisePropertyChanged();
            }
        }

        private async void ValidateCommandHandler()
        {
            if (this.FailedSignals == null)
                this.FailedSignals = new ObservableCollection<JsonSignal>();

            this.FailedSignals.Clear();

            JsonSignal[] jsonSignals = _jsonSerializer.Deserialize(this.DataStream);

            if (jsonSignals == null)
            {
                _messageDialog.Caption = "Invalid Json Data";
                _messageDialog.Message = "Error while parsing input string. Couldn't parse the string.";
                _dialogContainerService.ShowDialog(_messageDialog);
                return;
            }

            // refresh the rules
            await _ruleManagement.Prepare();

            foreach (JsonSignal jsonSignal in this.RunRules(jsonSignals))
            {
                this.FailedSignals.Add(jsonSignal);
            }
        }

        private IEnumerable<JsonSignal> RunRules(JsonSignal[] jsonSignals)
        {
            foreach (JsonSignal jsonSignal in jsonSignals)
            {
                if (!_ruleManagement.Validate(jsonSignal))
                    yield return jsonSignal;
            }
        }

        private bool CanExecuteValidateCommand()
        {
            return !string.IsNullOrWhiteSpace(this.DataStream);
        }

        private readonly RelayCommand _manageRulesCommand;
        public RelayCommand ManageRulesCommand
        {
            get { return _manageRulesCommand; }
        }

        private void ManageRulesCommandHandler()
        {
            _dialogContainerService.ShowDialog(_ruleManagement);
        }
    }
}