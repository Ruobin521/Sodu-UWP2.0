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

        private static void StartAction()
        {
            if (isOperating)
            {
                return;
            }

            isOperating = true;

            while (Actions.Count > 0)
            {
                try
                {
                    Actions.FirstOrDefault()?.Invoke();
                    Actions.RemoveAt(0);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"DbHelper：StartAction->{e.Message}\n{e.StackTrace}");
                }
            }

            if (Actions.Count == 0)
            {
                isOperating = false;
            }
        }

        public static void AddDbOperator(Action action)
        {
            Actions.Add(action);

            Debug.WriteLine("---------------------------------" + Actions.Count + "---------------------------------");

            StartAction();

        }


    }
}
