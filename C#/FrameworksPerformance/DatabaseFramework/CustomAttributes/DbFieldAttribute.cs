using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFramework.CustomAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    internal class DbFieldAttribute : Attribute
    {
    }
}
