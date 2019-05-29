// <copyright file="GUIsimple.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// WTXGUIsimple, a demo application for HBM Weighing-API  
//
// The MIT License (MIT)
//
// Copyright (C) Hottinger Baldwin Messtechnik GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

namespace GUIsimple
{
    using System;
    using System.Windows.Forms;
    using Hbm.Weighing.API;
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
