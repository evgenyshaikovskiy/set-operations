using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sets
{

    public class SetValueElem : ISetElement
    {

        public string Value;

        public ISetElement this[int index] { get => throw new InvalidOperationException("Cannot index a set value element"); }

        public bool Equals(ISetElement elem)
        {
            if (elem is SetValueElem) return Value == ((SetValueElem)elem).Value;
            /*else*/
            return false;
        }

        public ISetElement Duppl() => new SetValueElem(this.Value);

        public override string ToString() => Value;


        //Aliases for Duppl
        public ISetElement Dupplicate() => Duppl();


        public SetValueElem(string value)
        {
            Value = value;
        }

    }
}
