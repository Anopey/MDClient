﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ClientCode
{
    public static class Utils
    {
        public static byte[] GetBytes(string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }
    }
}
