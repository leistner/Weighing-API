using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Hbm.Weighing.API;

namespace GUIplc
{
    /// <summary>
    /// A form to select IO input and output functions listed in a checkedlistbox
    /// </summary>
    public partial class FunctionIO : Form
    {

        #region ==================== constants & fields ==================== 

        public EventHandler<IOFunctionEventArgs> ReadButtonClicked_IOFunctions;
        public EventHandler<IOFunctionEventArgs> WriteButtonClicked_IOFunctions;

        #endregion

        #region =============== constructors & destructors =================

        /// <summary>
        /// Constructor of FunctionIO. Sets the eventHandler of the checked lists to ItemCheck_methods, adds initial values.
        /// </summary>
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

        #endregion

        #region ==================== events & delegates ====================

        /// <summary>
        /// Gets the input and output functions. Used in instances of the enums 'OutputFunction', 'InputFunction'. 
        /// Triggers the event to commit the values (output, input functions) to the eventHandler in GuiSimpleForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the input and output functions. Used in instances of the enums 'OutputFunction', 'InputFunction'. 
        /// Triggers the event to commit the values (output, input functions) to the eventHandler in GuiSimpleForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the input function of checkList 1 to the event argument InputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="InputArg"></param>
        private void checkedListInputIO1_ItemCheck(object sender, ItemCheckEventArgs InputArg)
        {
            if (checkedListInputIO1.CheckedItems.Count >= 1 && InputArg.CurrentValue != CheckState.Checked)
            {
                InputArg.NewValue = InputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        /// <summary>
        /// Sets the input function of checkList 2 to the event argument InputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="InputArg"></param>
        private void checkedListInputIO2_ItemCheck(object sender, ItemCheckEventArgs InputArg)
        {
            if (checkedListInputIO2.CheckedItems.Count >= 1 && InputArg.CurrentValue != CheckState.Checked)
            {
                InputArg.NewValue = InputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        /// <summary>
        /// Sets the output function of checkList 1 to the event argument OutputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutputArg"></param>
        private void checkedListOutputIO1_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO1.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        /// <summary>
        /// Sets the output function of checkList 2 to the event argument OutputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutputArg"></param>
        private void checkedListOutputIO2_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO2.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        /// <summary>
        /// Sets the output function of checkList 3 to the event argument OutputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutputArg"></param>
        private void checkedListOutputIO3_ItemCheck(object sender, ItemCheckEventArgs OutputArg)
        {
            if (checkedListOutputIO3.CheckedItems.Count >= 1 && OutputArg.CurrentValue != CheckState.Checked)
            {
                OutputArg.NewValue = OutputArg.CurrentValue;
                MessageBox.Show("You can only check one item in one box");
            }
        }

        /// <summary>
        /// Sets the output function of checkList 4 to the event argument OutputArg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutputArg"></param>
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

        #endregion

    }
}
