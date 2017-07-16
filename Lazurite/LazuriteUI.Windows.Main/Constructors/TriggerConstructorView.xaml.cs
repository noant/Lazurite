using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class TriggerConstructorView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>(); 
        private TriggerView _constructorView;
        private Lazurite.MainDomain.TriggerBase _originalTrigger;
        private Lazurite.MainDomain.TriggerBase _clonedTrigger;

        public event Action Applied;
        public event Action Modified;
        public bool IsModified { get; private set; }

        public TriggerConstructorView()
        {
            InitializeComponent();
            this.buttonsView.ApplyClicked += () => Apply();
            this.buttonsView.ResetClicked += () => Revert();
        }

        public void SetTrigger(Lazurite.MainDomain.TriggerBase trigger)
        {
            var loadView = new MessageView();
            loadView.Icon = Icons.Icon.Hourglass;
            loadView.ContentText = "Формирование окна...";
            loadView.ShowInNewWindow();
            if (trigger != null)
            {
                _originalTrigger = trigger;
                _clonedTrigger = (Lazurite.MainDomain.TriggerBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalTrigger);
                _clonedTrigger.Initialize(_repository);
                this.contentPresenter.Content  = new TriggerView(_clonedTrigger);
                buttonsView.SetTrigger(_clonedTrigger);
                buttonsView.Modified += () => IsModified = true;
                _constructorView.Modified += () => Modified?.Invoke();
                _constructorView.Modified += () => buttonsView.TriggerModified();
                _constructorView.Modified += () => IsModified = true;
                _constructorView.Failed += () => buttonsView.Failed();
                _constructorView.Succeed += () => buttonsView.Success();
                EmptyTriggerModeOff();
            }
            else
            {
                EmptyTriggerModeOn();
            }
            IsModified = false;
            loadView.Close();
        }

        private void EmptyTriggerModeOn()
        {
            tbTriggerEmpty.Visibility = Visibility.Visible;
            buttonsViewHolder.Visibility = Visibility.Collapsed;
            this.contentPresenter.Content = null;
        }

        private void EmptyTriggerModeOff()
        {
            tbTriggerEmpty.Visibility = Visibility.Collapsed;
            buttonsViewHolder.Visibility = Visibility.Visible;
        }

        public Lazurite.MainDomain.TriggerBase GetTrigger()
        {
            return _originalTrigger;
        }

        public void Apply(Action callback = null)
        {
            ApplyInternal();
            callback?.Invoke();
        }

        private void ApplyInternal()
        {
            _originalTrigger.Stop();
            _repository.SaveTrigger(_clonedTrigger);
            _clonedTrigger.Initialize(_repository);
            SetTrigger(_clonedTrigger);
            Applied?.Invoke();
            IsModified = false;
        }

        public void Revert()
        {
            _clonedTrigger = (Lazurite.MainDomain.TriggerBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalTrigger);
            _clonedTrigger.Initialize(_repository);
            buttonsView.Revert(_clonedTrigger);
            _constructorView.Revert(_clonedTrigger);
            IsModified = false;
        }
    }
}