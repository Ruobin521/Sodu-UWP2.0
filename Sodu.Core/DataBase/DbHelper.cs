using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.DataBase
{
    public class DbHelper
    {

        private static List<Action> Actions = new List<Action>();

        private static bool isOperating = false;

        public static void AddDbOperator(Action action)
        {
            Actions.Add(action);

            Debug.WriteLine("---------------------------------" + Actions.Count + "---------------------------------");
            if (isOperating)
            {
                return;
            }

            isOperating = true;

            while (Actions.Count > 0)
            {
                Actions.FirstOrDefault()?.Invoke();
                Actions.RemoveAt(0);
            }

            if (Actions.Count == 0)
            {
                isOperating = false;
            }
        }


    }
}
