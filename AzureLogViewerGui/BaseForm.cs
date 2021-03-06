﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzureLogViewerGui
{
    public class BaseForm : Form
    {
        private Dictionary<Control, bool> controlState = new Dictionary<Control, bool>();
        private int busyCount = 0;
        private bool isBusy = false;

        public BaseForm()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (value)
                    busyCount++;
                else
                    busyCount--;

                if (isBusy && busyCount == 0)
                    DisableBusy();
                else if (!isBusy && busyCount == 1)
                    EnableBusy();
            }
        }

        private void EnableBusy()
        {
            isBusy = true;
            controlState.Clear();
            RecursiveDisable(this);
            Cursor.Current = Cursors.WaitCursor;
        }

        private void RecursiveDisable(Control c)
        {
            foreach (Control inner in c.Controls)
                RecursiveDisable(inner);
            SaveControlStateAndDisable(c);
        }

        private void DisableBusy()
        {
            isBusy = false;
            RecursiveRestore(this);
            Cursor.Current = Cursors.Default;
        }

        private void RecursiveRestore(Control c)
        {
            foreach (Control inner in c.Controls)
                RecursiveRestore(inner);
            RestoreControlState(c);
        }

        #region ControlState

        private void SaveControlStateAndDisable(Control control)
        {
            controlState.Add(control, control.Enabled);
            control.Enabled = false;
            if (control is DataGridView)
            {
                var dgv = (DataGridView)control;
                dgv.ForeColor = Color.Gray;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            }
        }
        
        private void RestoreControlState(Control control)
        {
            if (controlState.ContainsKey(control))
            {
                control.Enabled = controlState[control];
                if (control is DataGridView)
                {
                    var dgv = (DataGridView)control;
                    dgv.ForeColor = Control.DefaultForeColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Control.DefaultForeColor;
                }
            }
        }

        #endregion

    }
}
