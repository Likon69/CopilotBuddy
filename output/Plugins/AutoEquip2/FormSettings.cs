using System;
using System.Windows.Forms;

namespace Styx.Bot.Plugins.AutoEquip2
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            pgSettings.SelectedObject = AutoEquipSettings.Instance;
        }

        private void pgSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (pgSettings.SelectedObject != null && pgSettings.SelectedObject is AutoEquipSettings)
                ((AutoEquipSettings)pgSettings.SelectedObject).Save();
        }
    }
}
