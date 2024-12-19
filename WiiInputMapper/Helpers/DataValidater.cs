using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.Template
{
    public class DataValidater
    {
        /// <summary>
        /// This code checks if the pitch is valid;
        /// </summary>
        /// <param name="pitch">Pitch of device</param>
        /// <param name="roll">Roll of device</param>
        /// <param name="compareVal">Compare value when pitch is valid</param>
        /// <param name="compareC">What kind of comparison do we need to do?(< = >).</param>
        /// <returns>false if not valid. True if valid and calculation is also valid</returns>
        public static bool ValidatePitch(float pitch, float roll, float compareVal, char compareC)
        {
            if(roll < -50 || roll > 50)
            {
                Console.WriteLine(String.Format("Ignoring pitch value of {0} since the roll is {1}", pitch, roll));
                return false;
            }
            else
            {
                if (compareC == '>') return pitch > compareVal;
                if (compareC == '<') return pitch < compareVal;
                if (compareC == '=')
                {
                    if ((compareVal % 1) == 0) return Math.Round(pitch) == Math.Round(compareVal);
                    return Math.Round(pitch,1) == Math.Round(compareVal,1);
                }
                return false;
            } 
        }
    }
}
