﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UAI.Case.Exceptions.Security
{
    public class UserActivedException : Exception
    {
        public UserActivedException(String message) : base  (message)
        {

        }
    }
}
