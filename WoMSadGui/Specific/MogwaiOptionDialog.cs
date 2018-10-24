using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace WoMSadGui.Specific
{
    public class MogwaiOptionDialog : MogwaiDialog
    {
        private MogwaiRadioButton _selectedRadioButton;

        public string SelectedRadioButtonName => _selectedRadioButton?.Name;

        public MogwaiOptionDialog(string title, string text, int width, int height) : base(title, text, width, height)
        {

        }

        public MogwaiOptionDialog(string title, string text, Action<string> doAdventureAction, int width, int height) : base(title, text, width, height)
        {
            AddButton("ok", true);
            Button.Click += (btn, args) =>
            {
                var str = SelectedRadioButtonName;
                if (str?.Length > 0)
                {
                    Hide();
                    doAdventureAction(str);
                }
            };
            ButtonCancel.Click += (btn, args) =>
            {
                Hide();
            };
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
