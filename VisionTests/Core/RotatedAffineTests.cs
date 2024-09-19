using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vision.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Core.Tests
{
    [TestClass( )]
    public class RotatedAffineTests
    {
        [TestMethod( )]
        public void Math_TransferTest( )
        {
            RotatedAffine.Math_Transfer( 2 , 1 , Math.PI/2, 1 , 1 , out var x , out var y );
        }
    }
}