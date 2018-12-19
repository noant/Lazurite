using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.LazuritePlugin
{
    [HumanFriendlyName("Медиа")]
    [LazuriteIcon(Icon.TvNews)]
    [SuitableValueTypes(typeof(InfoValueType), typeof(ButtonValueType), typeof(StateValueType))]
    [Category(Category.Multimedia)]
    public class MediaHostPlugin : IAction
    {
        private MediaPanelBase _panel;
        private MediaCommandBase _command;
        private ValueTypeBase _valueType;

        private string _currentVal = ToggleValueType.ValueOFF;

        public string CommandName { get; set; }
        public string ObjectName { get; set; }
        public bool IsToggle { get; set; }

        public string Caption
        {
            get => ObjectName + (!IsToggle ? " " + CommandName : string.Empty);
            set { }
        }

        public ValueTypeBase ValueType {
            get
            {
                // Статусы могут обновиться
                if (_command is StatesMediaCommand statesCommand)
                    (_valueType as StateValueType).AcceptedValues = statesCommand.States;

                return _valueType;
            }
            set => _valueType = value;
        }

        public bool IsSupportsEvent => true;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            if (IsToggle)
                return MediaObjects.MediaHostWindow.IsSourceActive(_panel) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
            else
                return _command.Current;
        }

        public void Initialize()
        {
            _panel = MediaObjects.MediaHostWindow.Sources.FirstOrDefault(x => x.ElementName == ObjectName);

            if (_panel == null)
                throw new ArgumentNullException(string.Format("Объект [{0}] не существует", ObjectName));

            if (!IsToggle)
            {
                _command = _panel.Commands.FirstOrDefault(x => x.Name == CommandName);
                if (_command == null)
                    throw new ArgumentNullException(string.Format("Команда [{0}] объекта [{1}] не существует", CommandName, ObjectName));
                _command.Changed += (o, e) => OnValueChanged(e.Value);
            }

            MediaObjects.MediaHostWindow.SourcesChanged += MediaHostWindow_SourcesChanged;

            if (IsToggle)
                ValueType = new ToggleValueType();
            else if (_command is StatesMediaCommand statesCommand)
                ValueType = new StateValueType() { AcceptedValues = statesCommand.States };
            else if (_command.AllowParam)
                ValueType = new InfoValueType();
            else
                ValueType = new ButtonValueType();
        }

        private void MediaHostWindow_SourcesChanged(object sender, Lazurite.Shared.EventsArgs<object> args)
        {
            if (IsToggle)
            {
                var cur = _currentVal == ToggleValueType.ValueON;
                if (MediaObjects.MediaHostWindow.IsSourceActive(_panel) != cur)
                    OnValueChanged(cur ? ToggleValueType.ValueOFF : ToggleValueType.ValueON);
            }
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (IsToggle)
            {
                var show = value == ToggleValueType.ValueON;
                if (!show)
                    MediaObjects.MediaHostWindow.CloseSource(_panel);
                else MediaObjects.MediaHostWindow.ActivateSourceAsQuery(_panel);
            }
            else
            {
                if (!MediaObjects.MediaHostWindow.IsSourceActive(_panel) && _command.ActivateWithPanelBase(value))
                    MediaObjects.MediaHostWindow.ActivateSourceAsQuery(_panel);
                if (_command.AllowParam)
                    _command.Execute(value);
                else _command.Execute();
            }
            OnValueChanged(value);
        }

        private void OnValueChanged(string val)
        {
            ValueChanged?.Invoke(this, val);
            _currentVal = val;
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            CommandView selected = null;
            if (_panel != null)
                selected = new CommandView(_panel, _command);
            var window = new MainWindow(valueType, selected);

            if (window.ShowDialog() ?? false && window.SelectedCommand != null)
            {
                selected = window.SelectedCommand;
                IsToggle = selected.IsToggle;

                CommandName = selected.Command?.Name;
                ObjectName = selected.Panel.ElementName;

                Initialize();

                return true;
            }
            else return false;
        }
    }
}
