using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class CalculationForm : Form
    {
        private class TagValuePair
        {
            public string DisplayText { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }
        private List<ComboBox> tagIdComboBoxes;
        private List<ComboBox> operatorComboBoxes;
        private TagDefinitions tag;
        private readonly ExtendedPlc myPlc;

        public CalculationForm(ExtendedPlc myPlc, TagDefinitions tag)
        {
            this.tag = tag;
            this.myPlc = myPlc;
            InitializeComponent();
            if (tag.Name is null)
            {
                tag.Name = string.Empty;
            }
            else
            {
                this.Text = tag.Name + " beräkningar";
            }
            tagIdComboBoxes = new List<ComboBox>();
            operatorComboBoxes = new List<ComboBox>();
            if (tag.Calculation != null && !string.IsNullOrWhiteSpace(tag.Calculation))
            {
                // Parse the calculation string and auto-generate ComboBoxes
                var calculationTrimmed = tag.Calculation.Trim();
                string[] calculationParts = calculationTrimmed.Split(' ');
                for (int i = 0; i < calculationParts.Length; i++)
                {
                    if (IsNumber(calculationParts[i]))
                    {
                        AddNewElement("tag");
                    }
                    else
                    {
                        AddNewElement("operator");
                    }
                }
                SetSelectedValuesAndItems(calculationParts);
                GenerateCalculation();
            }
            else
            {
                AddNewElement("tag");
            }
        }


        private void SetSelectedValuesAndItems(string[] calculationParts)
        {
            int tagIndex = 0;
            int operatorIndex = 0;
            for (int i = 0; i < calculationParts.Length; i++)
            {
                if (IsNumber(calculationParts[i]))
                {
                    tagIdComboBoxes[tagIndex].SelectedValue = int.Parse(calculationParts[i]);
                    tagIndex++;
                }
                else
                {
                    operatorComboBoxes[operatorIndex].SelectedItem = calculationParts[i];
                    operatorIndex++;
                }
            }
        }

        private bool IsNumber(string v)
        {
            if (int.TryParse(v, out int result))
            {
                return true;
            }
            return false;
        }

        private void btnAddComboBoxes_Click(object sender, EventArgs e)
        {
            if (tag.Calculation != null && !string.IsNullOrWhiteSpace(tag.Calculation))
            {
                AddNewElement("operator");
            }
            else
            {
                AddNewElement("tag");
            }
        }



        private void AddNewElement(string type)
        {
            int yOffset = Math.Max(tagIdComboBoxes.Count, operatorComboBoxes.Count) * 30;

            if (type == "tag")
            {
                var tagIdComboBox = new ComboBox
                {
                    Name = $"cmbTagId{tagIdComboBoxes.Count + 1}",
                    Location = new System.Drawing.Point(10, yOffset),
                    Size = new System.Drawing.Size(250, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                pnlComboBoxes.Controls.Add(tagIdComboBox);
                InitializeTagIdComboBox(tagIdComboBox);
                tagIdComboBoxes.Add(tagIdComboBox);
            }
            else if (type == "operator")
            {
                var operatorComboBox = new ComboBox
                {
                    Name = $"cmbOperator{operatorComboBoxes.Count + 1}",
                    Location = new System.Drawing.Point(270, yOffset),
                    Size = new System.Drawing.Size(50, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                pnlComboBoxes.Controls.Add(operatorComboBox);
                InitializeOperatorComboBox(operatorComboBox);
                operatorComboBoxes.Add(operatorComboBox);
            }
        }

        private void btnAddOperatorComboBox_Click(object sender, EventArgs e)
        {
            AddNewElement("operator");
        }

        private void btnAddTagComboBox_Click(object sender, EventArgs e)
        {
            AddNewElement("tag");
        }



        private void InitializeTagIdComboBox(ComboBox comboBox)
        {

            TagsToLog.GetAllTagsFromSql();
            var excludedTags = new List<TagDefinitions> { tag };

            List<TagValuePair> tagValuePairs = new List<TagValuePair>();

            foreach (var tagValue in TagsToLog.AllLogValues.Where(x => x.PlcName == myPlc.PlcName &&
                        IsNumeric(x.VarType)
                     && myPlc.Active).Except(excludedTags))
            {
                TagValuePair tagValuePair = new TagValuePair
                {
                    DisplayText = tagValue.Name,
                    Value = tagValue.Id
                };
                tagValuePairs.Add(tagValuePair);
            }
            comboBox.DataSource = tagValuePairs;
            comboBox.DisplayMember = "DisplayText";
            comboBox.ValueMember = "Value";
        }

        private bool IsNumeric(VarType varType)
        {
            switch (varType)
            {
                case VarType.Int:
                    return true;
                case VarType.DInt:
                    return true;
                case VarType.Byte:
                    return true;
                case VarType.DWord:
                    return true;
                case VarType.Real:
                    return true;
                case VarType.LReal:
                    return true;
                default:
                    return false;
            }
        }

        private void InitializeOperatorComboBox(ComboBox comboBox)
        {
            comboBox.Items.Add("+");
            comboBox.Items.Add("-");
            comboBox.Items.Add("*");
            comboBox.Items.Add("/");
            comboBox.Items.Add("(");
            comboBox.Items.Add(")");
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateCalculation();
        }

        private void GenerateCalculation()
        {
            tag.Calculation = string.Empty;
            var displayExpression = string.Empty;

            int maxCount = Math.Max(tagIdComboBoxes.Count, operatorComboBoxes.Count);
            int operatorIndex = 0;
            int tagIndex = 0;

            while (operatorIndex < operatorComboBoxes.Count || tagIndex < tagIdComboBoxes.Count)
            {
                if (operatorIndex < operatorComboBoxes.Count)
                {
                    var selectedOperator = operatorComboBoxes[operatorIndex].SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(selectedOperator) && selectedOperator == "(")
                    {
                        tag.Calculation += $" {selectedOperator}";
                        displayExpression += $" {selectedOperator}";
                        operatorIndex++;
                    }
                }

                if (tagIndex < tagIdComboBoxes.Count)
                {
                    var selectedTagName = tagIdComboBoxes[tagIndex].SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(selectedTagName))
                    {
                        var selectedTagId = tagIdComboBoxes[tagIndex].SelectedValue;
                        tag.Calculation += $" {selectedTagId}";
                        displayExpression += $" {selectedTagName}";
                        tagIndex++;

                        // Add the closing parenthesis if the next operator is not "+", "-", "*", or "/".
                        if (operatorIndex < operatorComboBoxes.Count)
                        {
                            var nextOperator = operatorComboBoxes[operatorIndex].SelectedItem?.ToString();
                            if (!string.IsNullOrEmpty(nextOperator) && nextOperator == ")")
                            {
                                tag.Calculation += $" {nextOperator}";
                                displayExpression += $" {nextOperator}";
                                operatorIndex++;
                            }
                        }
                    }
                }

                if (operatorIndex < operatorComboBoxes.Count)
                {
                    var selectedOperator = operatorComboBoxes[operatorIndex].SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(selectedOperator) && "+-*/".Contains(selectedOperator))
                    {
                        tag.Calculation += $" {selectedOperator}";
                        displayExpression += $" {selectedOperator}";
                        operatorIndex++;
                    }
                }
            }

            lblCalculation.Text = tag.Calculation.Trim();
            lblDisplayCalculation.Text = displayExpression.Trim();
        }

        private void btnRemoveComboBoxes_Click(object sender, EventArgs e)
        {
            RemoveLastComboBoxes();
        }

        private void RemoveLastComboBoxes()
        {
            if (operatorComboBoxes.Count > 0 && tagIdComboBoxes.Count > 0)
            {
                var lastOperatorComboBox = operatorComboBoxes.Last();
                var lastTagIdComboBox = tagIdComboBoxes.Last();

                if (lastOperatorComboBox.Location.Y > lastTagIdComboBox.Location.Y ||
                    (lastOperatorComboBox.Location.Y == lastTagIdComboBox.Location.Y))
                {
                    // Remove the last operator ComboBox.
                    operatorComboBoxes.RemoveAt(operatorComboBoxes.Count - 1);
                    pnlComboBoxes.Controls.Remove(lastOperatorComboBox);
                    lastOperatorComboBox.Dispose();
                }
                else
                {
                    // Remove the last tag ID ComboBox.
                    tagIdComboBoxes.RemoveAt(tagIdComboBoxes.Count - 1);
                    pnlComboBoxes.Controls.Remove(lastTagIdComboBox);
                    lastTagIdComboBox.Dispose();
                }
            }
            else if (operatorComboBoxes.Count > 0)
            {
                // Remove the last operator ComboBox.
                var lastOperatorComboBox = operatorComboBoxes.Last();
                operatorComboBoxes.RemoveAt(operatorComboBoxes.Count - 1);
                pnlComboBoxes.Controls.Remove(lastOperatorComboBox);
                lastOperatorComboBox.Dispose();
            }
            else if (tagIdComboBoxes.Count > 0)
            {
                // Remove the last tag ID ComboBox.
                var lastTagIdComboBox = tagIdComboBoxes.Last();
                tagIdComboBoxes.RemoveAt(tagIdComboBoxes.Count - 1);
                pnlComboBoxes.Controls.Remove(lastTagIdComboBox);
                lastTagIdComboBox.Dispose();
            }
        }
    }
}
