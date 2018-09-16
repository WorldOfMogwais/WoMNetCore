using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using WoMSadGui.Consoles;

namespace WoMSadGui.Dialogs
{
    public class MogwaiOptionDialog : MogwaiDialog
    {
        private MogwaiRadioButton _selectedRadioButton;

        public string SelectedRadioButtonName => _selectedRadioButton?.Name;

        public MogwaiOptionDialog(string title, string text, int width, int height) : base(title, text, width, height)
        {

        }

        private void ClickedRadioButton(object sender, MouseEventArgs e)
        {
            _selectedRadioButton = sender as MogwaiRadioButton;
        }

        public void AddRadioButtons(string groupName, List<string[]> strArrayList)
        {
            for (var i = 0; i < strArrayList.Count; i++)
            {
                var radioButton2 = new MogwaiRadioButton(30, 1)
                {
                    Name = strArrayList[i][0],
                    Position = new Point(4, 4 + i),
                    GroupName = groupName,
                    Text = strArrayList[i][1]
                };
                radioButton2.MouseButtonClicked += ClickedRadioButton;
                Add(radioButton2);
            }
        }

    }
}
