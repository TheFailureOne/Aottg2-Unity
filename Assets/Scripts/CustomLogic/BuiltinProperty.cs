﻿using System;

namespace CustomLogic
{
    internal class BuiltinProperty
    {
        private readonly Func<object, object> _getter;
        private readonly Action<object, object> _setter;
        
        public readonly bool IsReadOnly;

        public BuiltinProperty(Func<object, object> getter, Action<object, object> setter)
        {
            _getter = getter;
            _setter = setter;
            IsReadOnly = _setter == null;
        }

        public object GetValue(object instance) => _getter?.Invoke(instance);
        public void SetValue(object instance, object value) => _setter?.Invoke(instance, value);
    }
}