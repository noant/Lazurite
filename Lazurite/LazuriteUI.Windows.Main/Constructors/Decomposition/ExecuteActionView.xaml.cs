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
using Lazurite.CoreActions;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ExecuteActionView.xaml
    /// </summary>
    public partial class ExecuteActionView : UserControl, IConstructorElement
    {
        public ExecuteActionView(ExecuteAction action)
        {
            InitializeComponent();
            this.action1View.Refresh(action.MasterActionHolder);
            this.action2View.Refresh(action.InputValue);
            this.action1View.Modified += (element) => Modified?.Invoke(this);
            this.action2View.Modified += (element) => Modified?.Invoke(this);

            this.buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            this.buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
        }

        public ActionHolder ActionHolder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool EditMode
        {
            get;
            set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
