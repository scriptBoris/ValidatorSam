using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorTests.Supports
{
    public class MockObject
    {
        public MockObject()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}
