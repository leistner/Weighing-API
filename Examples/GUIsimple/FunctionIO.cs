using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GUIsimple;
using Hbm.Weighing.API;

namespace WTXModbus
{
    /// <summary>
    /// A form to select IO input and output functions listed in a checkedlistbox
    /// </summary>
    public partial class FunctionIO : Form
    {
        public EventHandler<IOFunctionEventArgs> ReadButtonClicked_IOFunctions;
        public EventHandler<IOFunctionEventArgs> WriteButtonClicked_IOFunctions;

        public FunctionIO()
        {
            InitializeComponent();

            checkedListInputIO1.ItemCheck += checkedListInputIO1_ItemCheck;
            checkedListInputIO2.ItemCheck += checkedListInputIO2_ItemCheck;

            checkedListOutputIO1.ItemCheck += checkedListOutputIO1_ItemCheck;
            checkedListOutputIO2.ItemCheck += checkedListOutputIO2_ItemCheck;
            checkedListOutputIO3.ItemCheck += checkedListOutputIO3_ItemCheck;
            checkedListOutputIO4.ItemCheck += checkedListOutputIO4_ItemCheck;

            foreach (InputFunction e in Enum.GetValues(typeof(InputFunction)))
            {
                checkedListInputIO1.Items.Add(e.ToString(), false);
                checkedListInputIO2.Items.Add(e.ToString(), false);
            }
            foreach (OutputFunction e in Enum.GetValues(typeof(OutputFunction)))
            {
                checkedListOutputIO1.Items.Add(e.ToString(), false);
                checkedListOutputIO2.Items.Add(e.ToString(), false);
                checkedListOutputIO3.Items.Add(e.ToString(), false);
                checkedListOutputIO4.Items.Add(e.ToString(), false);
            }
        }      
        // Button Read : 
        private void button1_Click(object sender, EventArgs e)
        {
            OutputFunction Out1 = (OutputFunction)checkedListOutputIO1.SelectedIndex;
            OutputFunction Out2 = (OutputFunction)checkedListOutputIO2.SelectedIndex;
            OutputFunction Out3 = (OutputFunction)checkedListOutputIO3.SelectedIndex;
            OutputFunction Out4 = (OutputFunction)checkedListOutputIO4.SelectedIndex;

            InputFunction In1 = (InputFunction)checkedListInputIO1.SelectedIndex;
            InputFunction In2 = (InputFunction)checkedListInputIO2.SelectedIndex;
            
            ReadButtonClicked_IOFunctions.Invoke(this, new IOFunctionEventArgs(Out1,Out2,Out3,Out4,In1,In2));

            this.Close();
        }
        // Button Write :
        private void button2_Click(object sender, EventArgs e)
        {
            OutputFunction Out1 = (OutputFunction)checkedListOutputIO1.SelectedIndex;
            OutputFunction Out2 = (OutputFunction)checkedListOutputIO2.SelectedIndex;
            OutputFunction Out3 = (OutputFunction)checkedListOutputIO3.SelectedIndex;
            OutputFunction Out4 = (OutputFunction)checkedListOutputIO4.SelectedIndex;

            InputFunction In1 = (InputFunction)checkedListInputIO1.SelectedIndex;
            InputFunction In2 = (InputFunction)checkedListInputIO2.SelectedIndex;

            WriteButtonClicked_IOFunctions.Invoke(this, new IOFunctionEventArgs(Out1, Out2, Out3, Out4, In1, In2));

            this.Close();
        }

        private void checkedListInputIO1_ItemCheck(object sender, ItemCheckEventArgs InputArg)
        {
            if (checkedListInputIO1.CheckedItems.Count >= 1 && InputArg.CurrentValue != CheckState.Checked)
            {
                InputArg.NewValue = InputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }
        private void checkedListInputIO2_ItemCheck(object sender, ItemCheckEventArgs InputArg)
        {
            if (checkedListInputIO2.CheckedItems.Count >= 1 && InputArg.CurrentValue != CheckState.Checked)
            {
                InputArg.NewValue = InputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }
        private void checkedListOutputIO1_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO1.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }
        private void checkedListOutputIO2_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO2.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }
        private void checkedListOutputIO3_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO3.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }
        private void checkedListOutputIO4_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO4.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        private void FunctionIO_Load(object sender, EventArgs e)
        { 
        }
        private void checkedListInputIO_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void checkedListOutputIO2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListOutputIO1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
