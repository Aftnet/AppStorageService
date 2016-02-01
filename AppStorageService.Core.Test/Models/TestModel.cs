﻿using System.Collections.Generic;
using System.Linq;

namespace AppStorageService.Core.Test.Models
{
    public class TestModel
    {
        private static int Ctr = 0;

        public int IntProperty { get; set; }
        public string StringProperty { get; set; }

        public bool Equals(TestModel other)
        {
            if (IntProperty != other.IntProperty) return false;
            if (StringProperty != other.StringProperty) return false;

            return true;
        }

        public static bool EnumEquals(IEnumerable<TestModel> reference, IEnumerable<TestModel> value)
        {
            if (reference.Count() != value.Count()) return false;

            foreach (var i in reference.Zip(value, (d, e) => new { refEl = d, valEl = e }))
            {
                if (!i.refEl.Equals(i.valEl)) return false;
            }

            return true;
        }

        public static TestModel Generate()
        {
            Ctr++;
            return new TestModel
            {
                IntProperty = Ctr,
                StringProperty = string.Format("TestProp {0}", Ctr)
            };
        }
    }
}
